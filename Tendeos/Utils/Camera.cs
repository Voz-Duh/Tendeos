using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tendeos.UI;
using Tendeos.Utils.Input;

namespace Tendeos.Utils
{
    public class Camera : IMouseCamera, IGUICamera
    {
        public Vec2 Position { get; set; }
        public float Rotation { get; set; }
        public float ScreenHeight { get; set; }
        /// <summary>
        /// Not recommended to change.
        /// </summary>
        public Vec2 Origin { get; set; }
        /// <summary>
        /// Not recommended to change.
        /// </summary>
        public Vec2 WorldViewport { get; set; }
        /// <summary>
        /// Not recommended to change.
        /// </summary>
        public float Scale { get; set; }

        public Camera(float screenHeight, Viewport viewport)
        {
            Rotation = 0;
            ScreenHeight = screenHeight;
            Position = Vec2.Zero;
            SetViewport(viewport);
        }

        public void SetViewport(Viewport viewport)
        {
            Scale = viewport.Height / ScreenHeight;
            WorldViewport = new Vec2((viewport.Width / (float) viewport.Height) * ScreenHeight, ScreenHeight);

            Origin = new Vec2(viewport.Width / 2f, viewport.Height / 2f);
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

        public Vec2 Screen2World(Vec2 point) => Vec2.Transform(point, Matrix.Invert(GetViewMatrix()));

        public Vec2 Screen2GUI(Vec2 point) => Vec2.Transform(point, Matrix.Invert(GetGUIMatrix()));

        public Vec2 World2Screen(Vec2 point) => Vec2.Transform(point, GetViewMatrix());

        public Vec2 GUI2Screen(Vec2 point) => Vec2.Transform(point, GetGUIMatrix());
    }
}