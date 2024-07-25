using System;
using System.Collections.Generic;
using Tendeos.Utils;
using Tendeos.Utils.Graphics;
using Tendeos.Utils.Input;

namespace Tendeos.UI
{
    public delegate IEnumerator<GUIElement> GuiElementAddDelegate();

    public class GUIElement
    {
        private static IGUICamera camera;
        public static GUIElement Selected;
        private static GUIElement lastSelected;
        private static bool selectedCurrently;

        private Action<SpriteBatch, FRectangle> drawAction = (a, b) => { };
        private Action<FRectangle> updateAction = (a) => { };
        private Action resetAction = () => { };

        public FRectangle Rectangle;
        public bool MouseOn { get; protected set; }

        public GUIElement Parent { get; private set; }
        public Vec2 Anchor;

        public GUIElement(Vec2 anchor, FRectangle rectangle, GUIElement[] childs)
        {
            Rectangle = rectangle;
            Anchor = anchor;
            
            if (childs == null) return;
            foreach (GUIElement child in childs) Add(child);
        }

        public GUIElement(IGUICamera camera)
        {
            GUIElement.camera = camera;
            Rectangle = new FRectangle(0, 0, 0, 0);
        }

        public GUIElement Add(GUIElement element)
        {
            element.Parent = this;
            drawAction += element.BaseDraw;
            updateAction += element.BaseUpdate;
            resetAction += element.Reset;
            element.OnAdd();
            return this;
        }

        public GUIElement Add(params GUIElement[] elements)
        {
            foreach (var element in elements)
            {
                element.Parent = this;
                drawAction += element.BaseDraw;
                updateAction += element.BaseUpdate;
                resetAction += element.Reset;
                element.OnAdd();
            }

            return this;
        }

        public GUIElement Add(IEnumerator<GUIElement> elements)
        {
            GUIElement element;
            while (elements.MoveNext())
            {
                element = elements.Current;
                element.Parent = this;
                drawAction += element.BaseDraw;
                updateAction += element.BaseUpdate;
                resetAction += element.Reset;
                element.OnAdd();
            }

            return this;
        }

        public virtual void OnAdd()
        {
        }

        public GUIElement Remove(GUIElement element)
        {
            drawAction -= element.BaseDraw;
            updateAction -= element.BaseUpdate;
            resetAction -= element.Reset;
            element.OnRemove();
            return this;
        }

        public virtual void OnRemove()
        {
        }

        public Vec2 GetAnchoredPosition(Vec2 point, FRectangle rectangle) =>
            rectangle.Location + (rectangle.Size - Rectangle.Size) * Anchor + point;

        public void Draw(SpriteBatch spriteBatch) =>
            BaseDraw(spriteBatch, new FRectangle(Vec2.Zero, camera.WorldViewport));

        public void BaseDraw(SpriteBatch spriteBatch, FRectangle rectangle)
        {
            FRectangle rect = Rectangle.Size == Vec2.Zero
                ? rectangle
                : new FRectangle(GetAnchoredPosition(Rectangle.Location, rectangle), Rectangle.Size);
            Draw(spriteBatch, rect);

            drawAction(spriteBatch, rect);
        }

        public virtual void Draw(SpriteBatch spriteBatch, FRectangle rectangle)
        {
        }

        public void Update() => BaseUpdate(new FRectangle(Vec2.Zero, camera.WorldViewport));

        public void BaseUpdate(FRectangle rectangle)
        {
            FRectangle rect = Rectangle.Size == Vec2.Zero
                ? rectangle
                : new FRectangle(GetAnchoredPosition(Rectangle.Location, rectangle), Rectangle.Size);
            Update(rect);

            if (MouseOn)
            {
                Delegate[] list = updateAction.GetInvocationList();
                for (int i = list.Length - 1; i >= 0; i--)
                    if (list[i].Target is GUIElement target)
                    {
                        list[i].Method.Invoke(list[i].Target, new object[] {rect});
                        if (target.MouseOn) break;
                    }
            }
        }

        public void Reset()
        {
            MouseOn = false;
            resetAction();
        }

        public virtual void Update(FRectangle rectangle)
        {
            if (Rectangle.Size == Vec2.Zero)
            {
                Mouse.OnGUI = false;
                MouseOn = true;
            }
            else if (MouseOn = (Parent?.MouseOn ?? true) && rectangle.Contains(Mouse.GUIPosition))
                Mouse.OnGUI = true;
        }

        public virtual void Unselect()
        {
        }

        public virtual void Select()
        {
        }

        public void Clear()
        {
            updateAction = a => { };
            drawAction = (a, b) => { };
            resetAction = () => { };
        }

        public static void Select(GUIElement element)
        {
            if (Selected != element) element?.Select();
            Selected = element;
            selectedCurrently = true;
        }

        public static void Deselect()
        {
            if (Mouse.LeftPressed)
            {
                if (selectedCurrently)
                {
                    if (lastSelected != Selected) lastSelected?.Unselect();
                    selectedCurrently = false;
                }
                else
                {
                    Selected?.Unselect();
                    Selected = null;
                }

                lastSelected = Selected;
            }
        }

        #region Draw

        public static void DrawRectWindow(SpriteBatch spriteBatch, Sprite[] texture, FRectangle rectangle)
        {
            float t00w = texture[0].Rect.Width;
            float t22w = texture[8].Rect.Width;
            float t00h = texture[0].Rect.Height;
            float t22h = texture[8].Rect.Height;

            spriteBatch.Rect(texture[0], rectangle.Location, 1, 0, 0, 0);
            spriteBatch.Rect(texture[1],
                new FRectangle(rectangle.X + t00w, rectangle.Y, rectangle.Width - t00w - t22w, t00h));
            spriteBatch.Rect(texture[2], new Vec2(rectangle.Right - t22w, rectangle.Y), 1, 0, 0, 0);

            spriteBatch.Rect(texture[3],
                new FRectangle(rectangle.X, rectangle.Y + t00h, t00w, rectangle.Height - t00h - t22h));
            spriteBatch.Rect(texture[4],
                new FRectangle(rectangle.X + t00w, rectangle.Y + t00h, rectangle.Width - t00w - t22w,
                    rectangle.Height - t00h - t22h));
            spriteBatch.Rect(texture[5],
                new FRectangle(rectangle.Right - t22w, rectangle.Y + t00h, t22w, rectangle.Height - t00h - t22h));

            spriteBatch.Rect(texture[6], new Vec2(rectangle.X, rectangle.Bottom - t22h), 1, 0, 0, 0);
            spriteBatch.Rect(texture[7],
                new FRectangle(rectangle.X + t00w, rectangle.Bottom - t22h, rectangle.Width - t00w - t22w, t22h));
            spriteBatch.Rect(texture[8], new Vec2(rectangle.Right - t22w, rectangle.Bottom - t22h), 1, 0, 0, 0);
        }

        public void DrawRectWindow(SpriteBatch spriteBatch, int from, Sprite[] texture, FRectangle rectangle)
        {
            float t00w = texture[from].Rect.Width;
            float t22w = texture[from + 8].Rect.Width;
            float t00h = texture[from].Rect.Height;
            float t22h = texture[from + 8].Rect.Height;

            spriteBatch.Rect(texture[from], rectangle.Location, 1, 0, 0, 0);
            spriteBatch.Rect(texture[from + 1],
                new FRectangle(rectangle.X + t00w, rectangle.Y, rectangle.Width - t00w - t22w, t00h));
            spriteBatch.Rect(texture[from + 2], new Vec2(rectangle.Right - t22w, rectangle.Y), 1, 0, 0, 0);

            spriteBatch.Rect(texture[from + 3],
                new FRectangle(rectangle.X, rectangle.Y + t00h, t00w, rectangle.Height - t00h - t22h));
            spriteBatch.Rect(texture[from + 4],
                new FRectangle(rectangle.X + t00w, rectangle.Y + t00h, rectangle.Width - t00w - t22w,
                    rectangle.Height - t00h - t22h));
            spriteBatch.Rect(texture[from + 5],
                new FRectangle(rectangle.Right - t22w, rectangle.Y + t00h, t22w, rectangle.Height - t00h - t22h));

            spriteBatch.Rect(texture[from + 6], new Vec2(rectangle.X, rectangle.Bottom - t22h), 1, 0, 0, 0);
            spriteBatch.Rect(texture[from + 7],
                new FRectangle(rectangle.X + t00w, rectangle.Bottom - t22h, rectangle.Width - t00w - t22w, t22h));
            spriteBatch.Rect(texture[from + 8], new Vec2(rectangle.Right - t22w, rectangle.Bottom - t22h), 1, 0, 0, 0);
        }

        #endregion
    }
}