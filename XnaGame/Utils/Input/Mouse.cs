using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

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
            return currentKeyState;
        }

        public static bool LeftUp => currentKeyState.LeftButton == ButtonState.Released;
        public static bool LeftReleased => currentKeyState.LeftButton == ButtonState.Released && previousKeyState.LeftButton != ButtonState.Released;
        public static bool LeftDown => currentKeyState.LeftButton == ButtonState.Pressed;
        public static bool LeftPressed => currentKeyState.LeftButton == ButtonState.Pressed && previousKeyState.LeftButton != ButtonState.Pressed;

        public static bool RightUp => currentKeyState.RightButton == ButtonState.Released;
        public static bool RightReleased => currentKeyState.RightButton == ButtonState.Released && previousKeyState.RightButton != ButtonState.Released;
        public static bool RightDown => currentKeyState.RightButton == ButtonState.Pressed;
        public static bool RightPressed => currentKeyState.RightButton == ButtonState.Pressed && previousKeyState.RightButton != ButtonState.Pressed;

        public static bool MiddleUp => currentKeyState.MiddleButton == ButtonState.Released;
        public static bool MiddleReleased => currentKeyState.MiddleButton == ButtonState.Released && previousKeyState.MiddleButton != ButtonState.Released;
        public static bool MiddleDown => currentKeyState.MiddleButton == ButtonState.Pressed;
        public static bool MiddlePressed => currentKeyState.MiddleButton == ButtonState.Pressed && previousKeyState.MiddleButton != ButtonState.Pressed;

        public static bool X1Up => currentKeyState.XButton1 == ButtonState.Released;
        public static bool X1Released => currentKeyState.XButton1 == ButtonState.Released && previousKeyState.XButton1 != ButtonState.Released;
        public static bool X1Down => currentKeyState.XButton1 == ButtonState.Pressed;
        public static bool X1Pressed => currentKeyState.XButton1 == ButtonState.Pressed && previousKeyState.XButton1 != ButtonState.Pressed;

        public static bool X2Up => currentKeyState.XButton2 == ButtonState.Released;
        public static bool X2Released => currentKeyState.XButton2 == ButtonState.Released && previousKeyState.XButton2 != ButtonState.Released;
        public static bool X2Down => currentKeyState.XButton2 == ButtonState.Pressed;
        public static bool X2Pressed => currentKeyState.XButton2 == ButtonState.Pressed && previousKeyState.XButton2 != ButtonState.Pressed;

        public static Point Point => currentKeyState.Position;
        public static FVector2 Position => Camera?.ScreenToWorldSpace(currentKeyState.Position.ToVector2()) ?? currentKeyState.Position.ToVector2();
        public static FVector2 GUIPosition => Camera?.ScreenToGUISpace(currentKeyState.Position.ToVector2()) ?? currentKeyState.Position.ToVector2();

        public static int Scroll => currentKeyState.ScrollWheelValue;
        public static int XScroll => currentKeyState.HorizontalScrollWheelValue;

        public static bool OnGUI { get; set; }
    }
}
