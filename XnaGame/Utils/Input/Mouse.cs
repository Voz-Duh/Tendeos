using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace XnaGame.Utils.Input
{
    public static class Mouse
    {
        public static IMouseCamera Camera { private get; set; }

        static MouseState currentKeyState;
        static MouseState previousKeyState;

        public static MouseState Update()
        {
            previousKeyState = currentKeyState;
            currentKeyState = Microsoft.Xna.Framework.Input.Mouse.GetState();
            LeftUp = currentKeyState.LeftButton == ButtonState.Released;
            LeftReleased = currentKeyState.LeftButton == ButtonState.Released && previousKeyState.LeftButton != ButtonState.Released;
            LeftDown = currentKeyState.LeftButton == ButtonState.Pressed;
            LeftPressed = currentKeyState.LeftButton == ButtonState.Pressed && previousKeyState.LeftButton != ButtonState.Pressed;
            RightUp = currentKeyState.RightButton == ButtonState.Released;
            RightReleased = currentKeyState.RightButton == ButtonState.Released && previousKeyState.RightButton != ButtonState.Released;
            RightDown = currentKeyState.RightButton == ButtonState.Pressed;
            RightPressed = currentKeyState.RightButton == ButtonState.Pressed && previousKeyState.RightButton != ButtonState.Pressed;
            MiddleUp = currentKeyState.MiddleButton == ButtonState.Released;
            MiddleReleased = currentKeyState.MiddleButton == ButtonState.Released && previousKeyState.MiddleButton != ButtonState.Released;
            MiddleDown = currentKeyState.MiddleButton == ButtonState.Pressed;
            MiddlePressed = currentKeyState.MiddleButton == ButtonState.Pressed && previousKeyState.MiddleButton != ButtonState.Pressed;
            X1Up = currentKeyState.XButton1 == ButtonState.Released;
            X1Released = currentKeyState.XButton1 == ButtonState.Released && previousKeyState.XButton1 != ButtonState.Released;
            X1Down = currentKeyState.XButton1 == ButtonState.Pressed;
            X1Pressed = currentKeyState.XButton1 == ButtonState.Pressed && previousKeyState.XButton1 != ButtonState.Pressed;
            X2Up = currentKeyState.XButton2 == ButtonState.Released;
            X2Released = currentKeyState.XButton2 == ButtonState.Released && previousKeyState.XButton2 != ButtonState.Released;
            X2Down = currentKeyState.XButton2 == ButtonState.Pressed;
            X2Pressed = currentKeyState.XButton2 == ButtonState.Pressed && previousKeyState.XButton2 != ButtonState.Pressed;
            Point = currentKeyState.Position;
            Position = Camera?.Screen2World(currentKeyState.Position.ToVector2()) ?? currentKeyState.Position.ToVector2();
            GUIPosition = Camera?.Screen2GUI(currentKeyState.Position.ToVector2()) ?? currentKeyState.Position.ToVector2();
            PointDelta = Point - previousKeyState.Position;
            PositionDelta = Position - Camera?.Screen2World(previousKeyState.Position.ToVector2()) ?? previousKeyState.Position.ToVector2();
            GUIPositionDelta = GUIPosition - Camera?.Screen2GUI(previousKeyState.Position.ToVector2()) ?? previousKeyState.Position.ToVector2();
            onUpdate();
            return currentKeyState;
        }

        public static bool LeftUp { get; private set; }
        public static bool LeftReleased { get; private set; }
        public static bool LeftDown { get; private set; }
        public static bool LeftPressed { get; private set; }

        public static bool RightUp { get; private set; }
        public static bool RightReleased { get; private set; }
        public static bool RightDown { get; private set; }
        public static bool RightPressed { get; private set; }

        public static bool MiddleUp { get; private set; }
        public static bool MiddleReleased { get; private set; }
        public static bool MiddleDown { get; private set; }
        public static bool MiddlePressed { get; private set; }

        public static bool X1Up { get; private set; }
        public static bool X1Released { get; private set; }
        public static bool X1Down { get; private set; }
        public static bool X1Pressed { get; private set; }

        public static bool X2Up { get; private set; }
        public static bool X2Released { get; private set; }
        public static bool X2Down { get; private set; }
        public static bool X2Pressed { get; private set; }

        public static Point Point { get; private set; }
        public static Vec2 Position { get; private set; }
        public static Vec2 GUIPosition { get; private set; }

        public static Point PointDelta { get; private set; }
        public static Vec2 PositionDelta { get; private set; }
        public static Vec2 GUIPositionDelta { get; private set; }


        private static Action onUpdate = () => { };
        public static event Action OnUpdate
        {
            add => onUpdate += value;
            remove => onUpdate -= value;
        }

        public static int Scroll => currentKeyState.ScrollWheelValue;
        public static int XScroll => currentKeyState.HorizontalScrollWheelValue;

        public static bool OnGUI { get; set; }
    }
}
