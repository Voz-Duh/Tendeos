using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using XnaGame.Utils.Graphics;
using XnaGame.WorldMap;
using XnaGame.WorldMap.Content;

namespace XnaGame.Content
{
    public static class Tiles
    {
        public static ITile test;

        public static void Init(ContentManager content)
        {
            test = new AutoTile(2, new Sprite(content.Load<Texture2D>("tiles/test")), new Sprite(content.Load<Texture2D>("tiles/test_item")));
        }
    }
}
