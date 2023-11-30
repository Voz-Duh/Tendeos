using Prototype.Graphics;
using System;
using XnaGame.Utils;
using XnaGame.Utils.Graphics;
using XnaGame.Utils.Input;

namespace XnaGame.UI
{
    public class GUIElement
    {
        private static IGUICamera Camera { get; set; }

        public Action<FRectangle> DrawAction = (p) => { };
        public Action<FRectangle> UpdateAction = (p) => { };
        public Action ResetAction = () => { };

        public FRectangle rectangle;
        public bool MouseOn { get; private set; }

        public GUIElement Parent { get; private set; }
        public FVector2 Anchor { get; }

        public GUIElement(GUIElement parent, FVector2 anchor)
        {
            Anchor = anchor;
            parent?.Add(this);
        }

        public GUIElement(IGUICamera camera)
        {
            Camera = camera;
            rectangle = new FRectangle(0, 0, 0, 0);
        }

        public void Add(GUIElement gameObject)
        {
            gameObject.Parent = this;
            DrawAction += gameObject.BaseDraw;
            UpdateAction += gameObject.BaseUpdate;
            ResetAction += gameObject.Reset;
        }

        public void Remove(GUIElement gameObject)
        {
            DrawAction -= gameObject.BaseDraw;
            UpdateAction -= gameObject.BaseUpdate;
            ResetAction -= gameObject.Reset;
        }

        public void Remove() => Parent.Remove(this);

        public FVector2 GetAnchoredPosition(FVector2 point, FRectangle rectangle) => rectangle.Location + (rectangle.Size - this.rectangle.Size) * Anchor + point;

        public void Draw() => BaseDraw(new FRectangle(FVector2.Zero, Camera.WorldViewport));
        public void BaseDraw(FRectangle rectangle)
        {
            FRectangle rect = this.rectangle.Size == FVector2.Zero ? rectangle : new FRectangle(GetAnchoredPosition(this.rectangle.Location, rectangle), this.rectangle.Size);
            Draw(rect);

            DrawAction(rect);
        }

        public virtual void Draw(FRectangle rectangle)
        {

        }

        public void Update() => BaseUpdate(new FRectangle(FVector2.Zero, Camera.WorldViewport));
        public void BaseUpdate(FRectangle rectangle)
        {
            FRectangle rect = this.rectangle.Size == FVector2.Zero ? rectangle : new FRectangle(GetAnchoredPosition(this.rectangle.Location, rectangle), this.rectangle.Size);
            Update(rect);

            if (MouseOn)
            {
                Delegate[] list = UpdateAction.GetInvocationList();
                for (int i = list.Length - 1; i >= 0; i--)
                    if (list[i].Target is GUIElement target)
                    {
                        list[i].DynamicInvoke(rect);
                        if (target.MouseOn) break;
                    }
            }
        }

        public void Reset()
        {
            MouseOn = false;
            ResetAction();
        }

        public virtual void Update(FRectangle rectangle)
        {
            if (MouseOn = (Parent?.MouseOn ?? true) && rectangle.Contains(Mouse.GUIPosition))
                Mouse.OnGUI = true;
        }

        #region Draw

        public void DrawRectWindow(Sprite[] texture, FRectangle rectangle)
        {
            float t00w = texture[0].Rect.Width;
            float t22w = texture[8].Rect.Width;
            float t00h = texture[0].Rect.Height;
            float t22h = texture[8].Rect.Height;

            SDraw.Rect(texture[0], rectangle.Location, null, 0, 0, Origin.Zero, Origin.Zero);
            SDraw.Rect(texture[1], new FRectangle(rectangle.X + t00w, rectangle.Y, rectangle.Width - t00w - t22w, t00h), 0);
            SDraw.Rect(texture[2], new FVector2(rectangle.Right - t22w, rectangle.Y), null, 0, 0, Origin.Zero, Origin.Zero);

            SDraw.Rect(texture[3], new FRectangle(rectangle.X, rectangle.Y + t00h, t00w, rectangle.Height - t00h - t22h), 0);
            SDraw.Rect(texture[4], new FRectangle(rectangle.X + t00w, rectangle.Y + t00h, rectangle.Width - t00w - t22w, rectangle.Height - t00h - t22h), 0);
            SDraw.Rect(texture[5], new FRectangle(rectangle.Right - t22w, rectangle.Y + t00h, t22w, rectangle.Height - t00h - t22h), 0);

            SDraw.Rect(texture[6], new FVector2(rectangle.X, rectangle.Top - t22h), null, 0, 0, Origin.Zero, Origin.Zero);
            SDraw.Rect(texture[7], new FRectangle(rectangle.X + t00w, rectangle.Top - t22h, rectangle.Width - t00w - t22w, t22h), 0);
            SDraw.Rect(texture[8], new FVector2(rectangle.Right - t22w, rectangle.Top - t22h), null, 0, 0, Origin.Zero, Origin.Zero);
        }

        #endregion
    }
}