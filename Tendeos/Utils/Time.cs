using Microsoft.Xna.Framework;

namespace Tendeos.Utils
{
    public static class Time
    {
        public static GameTime gameTime;
        public static float Total => (float) gameTime.TotalGameTime.TotalSeconds;
        public static float Delta => (float) gameTime.ElapsedGameTime.TotalSeconds;
    }
}