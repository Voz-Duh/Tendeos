using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tendeos.Content.Utlis;
using Tendeos.Utils;
using Tendeos.Utils.Graphics;

namespace Tendeos.World.Content
{
    public class AutoTile : Tile
    {
        [SpriteLoad("@", 4, 4, 1)]
        public Sprite[] sprites;

        public AutoTile()
        {
            DataCount = 1;
        }

        public override void Changed(bool top, IMap map, int x, int y, TileData data)
        {
            data[0] = UpdateTile(top, map, x, y);
        }

        public override void Draw(SpriteBatch spriteBatch, bool top, IMap map, int x, int y, Vec2 drawPosition, TileData data)
        {
            float g = data.Health / Health;
            spriteBatch.Rect(sprites[data[0]], new Color(top ? new Vector3(g) : new Vector3(g) * dark.ToVector3()), drawPosition, 0, 1.01f);
        }

        public override void Start(bool top, IMap map, int x, int y, TileData data)
        {
            data[0] = UpdateTile(top, map, x, y);
        }

        public static byte UpdateTile(bool top, IMap map, int x, int y)
        {
            byte res = 5;
            bool left = map.GetTile(top, x - 1, y).Tile is not AutoTile, right = map.GetTile(top, x + 1, y).Tile is not AutoTile,
                 down = map.GetTile(top, x, y - 1).Tile is not AutoTile, up = map.GetTile(top, x, y + 1).Tile is not AutoTile;
            bool lr = left && right,
                 du = down && up;

            if (up) res = 9;
            if (down) res = 1;

            if (right) res = 6;
            if (left) res = 4;

            if (left && up) res = 8;
            if (right && up) res = 10;
            if (left && down) res = 0;
            if (right && down) res = 2;

            if (lr) res = 7;
            if (du) res = 13;

            if (lr && up) res = 11;
            if (lr && down) res = 3;

            if (du && left) res = 12;
            if (du && right) res = 14;

            if (lr && du) res = 15;

            return res;
        }
    }
}
