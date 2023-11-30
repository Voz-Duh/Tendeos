using XnaGame.Entities;
using XnaGame.Utils;
using XnaGame.Utils.Graphics;
using XnaGame.Utils.Input;

namespace XnaGame.WorldMap.Content
{
    public class AutoTile : ITile
    {
        private readonly Sprite[] sprites;

        public AutoTile(byte health, Sprite sprite, Sprite item)
        {
            sprites = sprite.Split(4, 4, 1);
            this.health = health;
            ItemSprite = item;
        }

        public string Name => throw new System.NotImplementedException();

        public string Description => throw new System.NotImplementedException();
        public Sprite ItemSprite { get; init; }

        public byte Health => health;
        public byte health;

        public byte Hardness => throw new System.NotImplementedException();

        public int MaxCount => 40;

        public void Changed(IMap map, int x, int y, TileData data)
        {
            data[0] = UpdateTile(map, x, y);
        }

        public void Draw(IMap map, int x, int y, FVector2 drawPosition, TileData data)
        {
            SDraw.Rect(sprites[data[0]], drawPosition, 0, 1, 0, Origin.Zero, Origin.Zero);
        }

        public byte[] GetData() => new byte[] { 0 };

        public void Start(IMap map, int x, int y, TileData data)
        {
            data[0] = UpdateTile(map, x, y);
        }

        public static byte UpdateTile(IMap map, int x, int y)
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

        public void Use(ITransform transform, ref int count)
        {
            if (Mouse.LeftDown && !Mouse.OnGUI)
            {
                if (Core.map.TryPlaceTile(this, Core.map.World2Cell(Mouse.Position))) count -= 1;
            }
        }

        public void With(ITransform transform, ref byte armsState, ref float armsRotation)
        {
            armsState = 1;

            SDraw.Rect(ItemSprite, transform.Local2World(new FVector2(2, -2)));
        }
    }
}
