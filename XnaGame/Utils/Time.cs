using Microsoft.Xna.Framework;

namespace XnaGame.Utils
{
    public static class Time
    {
        public static GameTime GameTime { private get; set; }
        public static float Total => (float)GameTime.TotalGameTime.TotalSeconds;
        public static float Delta => (float)GameTime.ElapsedGameTime.TotalSeconds;
    }
}
