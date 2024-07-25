using System;

namespace Tendeos.Utils.Graphics
{
    public static class SpriteHelper
    {
        public const float frameRate = 0.6f;

        public static int Animation(this Sprite[] sprites, float frameRate, ref float timer, bool inversed = false)
        {
            if (sprites.Length == 1) return 0;
            timer += Time.Delta / frameRate;
            if (inversed) return (sprites.Length - 1) - (int) (timer * (sprites.Length - 1)) % (sprites.Length - 1);
            return (int) (timer * (sprites.Length - 1)) % (sprites.Length - 1);
        }

        public static int Animation(this Sprite[] sprites, float frameRate, float timer)
        {
            if (sprites.Length == 1) return 0;
            return (int) (timer / frameRate) % (sprites.Length - 1);
        }

        public static bool AnimationEnd(this Sprite[] sprites, out int frame, float frameRate, ref float timer)
        {
            if (sprites.Length == 1)
            {
                frame = 0;
                return true;
            }

            timer += Time.Delta / frameRate;
            timer = MathF.Min(timer, 1);
            frame = (int) (timer * (sprites.Length - 1));
            return timer == 1;
        }
    }
}