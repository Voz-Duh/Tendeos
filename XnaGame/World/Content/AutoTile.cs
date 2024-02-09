using Microsoft.Xna.Framework;
using XnaGame.Inventory;
using XnaGame.PEntities;
using XnaGame.Utils;
using XnaGame.Utils.Graphics;
using XnaGame.Utils.Input;

namespace XnaGame.World.Content
{
    public class AutoTile : ITile
    {
        public static readonly Color dark = new Color(0.56f, 0.5f, 0.51f);

        public string Name { get; set; }
        public string Description { get; set; }
        public int MaxCount { get; set; } = 100;
        public Sprite ItemSprite { get; init; }
        public bool Wallable { get; set; } = true;
        public bool Flip => true;
        public bool Animated => false;

        public float Health { get; set; } = 1;
        public byte Hardness { get; set; } = 0;

        public bool ShadowAvailable { get; set; } = true;
        public float ShadowIntensity { get; set; } = float.MaxValue;

        private readonly Sprite[] sprites;

        public AutoTile(Sprite sprite, Sprite item)
        {
            sprites = sprite.Split(4, 4, 1);
            ItemSprite = item;
        }

        public void Changed(bool top, IMap map, int x, int y, TileData data)
        {
            data[0] = UpdateTile(top, map, x, y);
        }

        public void Draw(bool top, IMap map, int x, int y, Vec2 drawPosition, float angle, TileData data)
        {
            float g = data.Health / Health;
            SDraw.Rect(sprites[data[0]], new Color(top ? new Vector3(g) : new Vector3(g) * dark.ToVector3()), drawPosition, angle, 1, 0, Origin.Zero, Origin.Zero);
        }

        public byte[] GetData() => new byte[] { 0 };

        public void Start(bool top, IMap map, int x, int y, TileData data)
        {
            data[0] = UpdateTile(top, map, x, y);
        }

        public static byte UpdateTile(bool top, IMap map, int x, int y)
        {
            byte res = 5;
            bool left = map.GetTile(top, x - 1, y).Tile == null, right = map.GetTile(top, x + 1, y).Tile == null,
                 down = map.GetTile(top, x, y - 1).Tile == null, up = map.GetTile(top, x, y + 1).Tile == null;
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

        public void Use(ITransform transform, ref byte armsState, ref float armLRotation, ref float armRRotation, ref int count, ref float timer, ArmData armData)
        {
            armsState = 1;

            if (!Mouse.OnGUI)
            {
                if (Mouse.LeftDown)
                    if (Core.map.TryPlaceTile(true, this, Core.map.World2Cell(Mouse.Position))) count -= 1;
                if (Mouse.RightDown)
                    if (Core.map.TryPlaceTile(false, this, Core.map.World2Cell(Mouse.Position))) count -= 1;
            }
        }

        public void With(ITransform transform, byte armsState, float armLRotation, float armRRotation, ArmData armData)
        {
            SDraw.Rect(ItemSprite, transform.Local2World(new Vec2(2, -2)));
        }
    }
}
