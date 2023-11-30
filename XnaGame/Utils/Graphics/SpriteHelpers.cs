using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XnaGame.Utils.Graphics
{
    public static class SpriteHelpers
    {
        public static int Animation(this Sprite[] sprites, float frameRate) => (int)(Time.Total / frameRate * (sprites.Length - 1)) % (sprites.Length - 1);
    }
}
