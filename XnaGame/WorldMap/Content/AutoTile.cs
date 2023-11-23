using Microsoft.Xna.Framework.Graphics;
using XnaGame.Utils;

namespace XnaGame.WorldMap.Content
{
    public class AutoTile : ITile
    {
        private readonly Sprite[] sprites;

        public AutoTile(byte health, Sprite sprite)
        {
            sprites = sprite.Split(4, 4, 1);
            this.health = health;
        }

        public string Name => throw new System.NotImplementedException();

        public string Description => throw new System.NotImplementedException();

        public byte Health => health;
        public byte health;

        public byte Hardness => throw new System.NotImplementedException();

        public void Changed(IMap map, int x, int y, TileData data)
        {
            data[0] = UpdateTile(map, x, y);
        }

        public void Draw(IMap map, int x, int y, FVector2 drawPosition, TileData data)
        {
            SDraw.Rect(sprites[data[0]], drawPosition, 0, 1, 0, SDraw.Origin.Zero, SDraw.Origin.Zero);
        }

        public byte[] GetData() => new byte[] { 0 };

        public void Start(IMap map, int x, int y, TileData data)
        {
            data[0] = UpdateTile(map, x, y);
        }

        public byte UpdateTile(IMap map, int x, int y)
        {
            byte res = 5;
            bool left = map.GetTile(x - 1, y).Tile == null, right = map.GetTile(x + 1, y).Tile == null,
                 down = map.GetTile(x, y - 1).Tile == null, up = map.GetTile(x, y + 1).Tile == null;
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

        public void Update(IMap map, int x, int y, TileData data)
        {
        }

        public void Use()
        {
            throw new System.NotImplementedException();
        }

        public byte With()
        {
            throw new System.NotImplementedException();
        }
    }
}
