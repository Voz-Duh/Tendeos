namespace Tendeos.Utils.Input
{
    public static class Controls
    {
        public static Vec2 GetRelativeCursorPosition(Vec2 position) => position - Mouse.Position;
        public static bool ScrollLeft => Mouse.Scroll < 0;
        public static bool ScrollRight => Mouse.Scroll > 0;
        public static bool Use => Keyboard.IsPressed(Keys.E);
        public static bool GoLeft => Keyboard.IsDown(Keys.A);
        public static bool GoRight => Keyboard.IsDown(Keys.D);
        public static bool UpHit => Mouse.LeftDown;
        public static bool DownHit => Mouse.RightDown;
        public static bool Jump => Keyboard.IsPressed(Keys.Space);
        public static bool Drop => Keyboard.IsPressed(Keys.Q);
    }
}