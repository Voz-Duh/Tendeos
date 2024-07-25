using Microsoft.Xna.Framework;

namespace Tendeos.Utils.Graphics
{
    public struct HEXColor
    {
        public byte R, G, B, A;

        public HEXColor(uint hex)
        {
            R = (byte) ((hex >> 24) & 0xFF);
            G = (byte) ((hex >> 16) & 0xFF);
            B = (byte) ((hex >> 8) & 0xFF);
            A = (byte) (hex & 0xFF);
        }

        public static implicit operator Color(HEXColor from) => new Color(from.R, from.G, from.B, from.A);

        public override string ToString() => $"#{R:X2}{G:X2}{B:X2}{A:X2}";
    }
}