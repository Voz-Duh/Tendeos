using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using XnaGame.World.Liquid;

namespace XnaGame.Content
{
    public static class Liquids
    {
        public static Liquid water, foo;

        public static void Init(ContentManager content)
        {
            water = new Liquid(Color.Aqua);
            foo = new Liquid(Color.BlueViolet);
        }

        public static Liquid Get(string value) => (Liquid)typeof(Liquids).GetField(value).GetValue(null);
    }
}
