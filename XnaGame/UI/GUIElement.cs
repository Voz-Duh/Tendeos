using Prototype.Graphics;
using System;
using XnaGame.Utils;
using XnaGame.Utils.Input;

namespace XnaGame.UI
{
    public class GUIElement
    {
        public const float baseWidth = 100, baseHeight = 60;
        public static FVector2 BaseSize => new FVector2(baseWidth, baseHeight);

        private static IGUICamera Camera { get; set; }
        public static FVector2 ScreenSize => Camera.Origin / Camera.Zoom;


        public Action<FVector2> DrawAction = (p) => { };
        public Action<FVector2> UpdateAction = (p) => { };

        public FRectangle rectangle;
        public bool MouseOn { get; private set; }

        public GUIElement Parent { get; init; }

        public GUIElement(GUIElement parent)
        {
            Parent = parent;
            parent.Add(this);
        }

        public GUIElement(IGUICamera camera)
        {
            Camera = camera;
            rectangle = new FRectangle(0, 0, 0, 0);
        }

        public void Add(GUIElement gameObject)
        {
            DrawAction += gameObject.BaseDraw;
            UpdateAction += gameObject.BaseUpdate;
        }

        public void Remove(GUIElement gameObject)
        {
            DrawAction -= gameObject.BaseDraw;
            UpdateAction -= gameObject.BaseUpdate;
        }

        public void Remove() => Parent.Remove(this);

        public void BaseDraw(FVector2 point)
        {
            FVector2 screenSize = ScreenSize;
            Draw(new FRectangle((rectangle.Location + point) / BaseSize * screenSize, rectangle.Size / BaseSize * screenSize));
        }

        public virtual void Draw(FRectangle rectangle)
        {
            FVector2 screenSize = ScreenSize;
            DrawAction(rectangle.Location * BaseSize / screenSize);
        }

        public void BaseUpdate(FVector2 point)
        {
            FVector2 screenSize = ScreenSize;
            FRectangle rectangle = new FRectangle((this.rectangle.Location + point) / BaseSize * screenSize, this.rectangle.Size / BaseSize * screenSize);
            Update(rectangle);

            if (MouseOn)
            {
                Delegate[] list = UpdateAction.GetInvocationList();
                bool breaked = false;
                for (int i = list.Length - 1; i >= 0; i--)
                    if (list[i].Target is GUIElement target)
                    {
                        target.MouseOn = false;
                        if (!breaked)
                        {
                            list[i].DynamicInvoke(this.rectangle.Location + point);
                            if (target.MouseOn) breaked = true;
                        }
                    }
            }
        }

        public virtual void Update(FRectangle rectangle)
        {
            if (rectangle.Size != FVector2.Zero)
            {
                if (MouseOn = Parent.MouseOn && rectangle.Contains(Mouse.GUIPosition))
                    Mouse.OnGUI = true;
            }
            else MouseOn = true;
        }

        #region Draw

        public void DrawRectWindow(Sprite[] texture, FRectangle rectangle)
        {
            float t00w = texture[0].Rect.Width;
            float t22w = texture[8].Rect.Width;
            float t00h = texture[0].Rect.Height;
            float t22h = texture[8].Rect.Height;

            SDraw.Rect(texture[0], rectangle.Location, null, 0, 0, SDraw.Origin.Zero, SDraw.Origin.Zero);
            SDraw.Rect(texture[1], new FRectangle(rectangle.X + t00w, rectangle.Y, rectangle.Width - t00w - t22w, t00h), 0);
            SDraw.Rect(texture[2], new FVector2(rectangle.Right - t22w, rectangle.Y), null, 0, 0, SDraw.Origin.Zero, SDraw.Origin.Zero);

            SDraw.Rect(texture[3], new FRectangle(rectangle.X, rectangle.Y + t00h, t00w, rectangle.Height - t00h - t22h), 0);
            SDraw.Rect(texture[4], new FRectangle(rectangle.X + t00w, rectangle.Y + t00h, rectangle.Width - t00w - t22w, rectangle.Height - t00h - t22h), 0);
            SDraw.Rect(texture[5], new FRectangle(rectangle.Right - t22w, rectangle.Y + t00h, t22w, rectangle.Height - t00h - t22h), 0);

            SDraw.Rect(texture[6], new FVector2(rectangle.X, rectangle.Top - t22h), null, 0, 0, SDraw.Origin.Zero, SDraw.Origin.Zero);
            SDraw.Rect(texture[7], new FRectangle(rectangle.X + t00w, rectangle.Top - t22h, rectangle.Width - t00w - t22w, t22h), 0);
            SDraw.Rect(texture[8], new FVector2(rectangle.Right - t22w, rectangle.Top - t22h), null, 0, 0, SDraw.Origin.Zero, SDraw.Origin.Zero);
        }

        #endregion
    }
}
