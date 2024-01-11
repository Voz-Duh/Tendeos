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
        public float ScreenHeight { get; set; }
        public FVector2 Origin { get; private set; }
        public FVector2 WorldViewport { get; private set; }
        private float Scale { get; set; }

        public Camera(float screenHeight, Viewport viewport)
        {
            Rotation = 0;
            ScreenHeight = screenHeight;
            Position = FVector2.Zero;
            SetViewport(viewport);
        }

        public void SetViewport(Viewport viewport)
        {
            Scale = viewport.Height / ScreenHeight;
            WorldViewport = new FVector2((float)viewport.Width / viewport.Height * ScreenHeight, ScreenHeight);

            Origin = new FVector2(viewport.Width / 2f, viewport.Height / 2f);
        }

        public Matrix GetViewMatrix()
        {
            return
                Matrix.CreateTranslation(new Vector3(-Position, 0.0f)) *
                Matrix.CreateRotationZ(Rotation) *
                Matrix.CreateScale(Scale, Scale, 1) *
                Matrix.CreateTranslation(new Vector3(Origin, 0.0f));
        }

        public Matrix GetGUIMatrix()
        {
            return
                Matrix.CreateScale(Scale, Scale, 1);
        }

        public FVector2 Screen2World(FVector2 point) => FVector2.Transform(point, Matrix.Invert(GetViewMatrix()));

        public FVector2 Screen2GUI(FVector2 point) => FVector2.Transform(point, Matrix.Invert(GetGUIMatrix()));

        public FVector2 World2Screen(FVector2 point) => FVector2.Transform(point, GetViewMatrix());

        public FVector2 GUI2Screen(FVector2 point) => FVector2.Transform(point, GetGUIMatrix());
    }
}
