using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Prototype.Graphics;
using XnaGame.Utils.Input;

namespace XnaGame.Utils
{
    public class Camera : IMouseCamera, IGUICamera
    {
        public FVector2 Position { get; set; }
        public float Rotation { get; set; }
        public float Zoom { get; set; }
        public FVector2 Origin { get; private set; }

        public Camera(Viewport viewport)
        {
            Rotation = 0;
            Zoom = 1;
            Origin = new FVector2(viewport.Width / 2f, viewport.Height / 2f);
            Position = FVector2.Zero;
        }

        public void SetViewport(Viewport viewport)
        {
            Origin = new FVector2(viewport.Width / 2f, viewport.Height / 2f);
        }

        public Matrix GetViewMatrix()
        {
            return
                Matrix.CreateTranslation(new Vector3(-Position, 0.0f)) *
                Matrix.CreateRotationZ(Rotation) *
                Matrix.CreateScale(Zoom, Zoom, 1) *
                Matrix.CreateTranslation(new Vector3(Origin, 0.0f));
        }

        public Matrix GetGUIMatrix()
        {
            return
                Matrix.CreateScale(Zoom, Zoom, 1) *
                Matrix.CreateRotationZ(Rotation);
        }

        public FVector2 ScreenToWorldSpace(FVector2 point) => FVector2.Transform(point, Matrix.Invert(GetViewMatrix()));

        public FVector2 ScreenToGUISpace(FVector2 point) => FVector2.Transform(point, Matrix.Invert(GetGUIMatrix()));
    }
}
