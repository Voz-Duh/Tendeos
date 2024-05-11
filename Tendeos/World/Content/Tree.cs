using Microsoft.Xna.Framework.Graphics;
using System;
using Tendeos.Content;
using Tendeos.Content.Utlis;
using Tendeos.Physical.Content;
using Tendeos.Utils;
using Tendeos.Utils.Graphics;

namespace Tendeos.World.Content
{
    public class Tree : Tile
    {
        [SpriteLoad("@", 3, 1, 1)]
        public Sprite[] sprites;
        [SpriteLoad("@_trunk", 4, 4, 1)]
        public Sprite[] trunkSprites;
        [SpriteLoad("@_crown", 2, 2, 1)]
        public Sprite[] crownSprites;

        public Range Height { get; set; } = 3..5;

        public Tree(string drop)
        {
            DropCount = 0..0;
            DropTag = drop;
            Collision = false;
            ShadowAvailable = false;
        }

        public override void Start(bool top, IMap map, int x, int y, ref TileData data)
        {
            byte value = (byte)URandom.SInt(Height.Start.Value, Height.End.Value);
            data.SetU8(0, value);

            ReferenceTile.Next = (x, y);
            for (int i = 1; i <= value; i++)
            {
                map.SetTile(true, Tiles.reference, x, y - i);
            }
        }

        public override void Loaded(bool top, IMap map, int x, int y, ref TileData data)
        {
            ReferenceTile.Next = (x, y);
            for (int i = 1; i <= data.GetU8(0); i++)
            {
                map.SetTile(true, Tiles.reference, x, y - i);
            }
        }

        public override void Draw(SpriteBatch spriteBatch, bool top, IMap map, int x, int y, Vec2 drawPosition, TileData data)
        {
            spriteBatch.Rect(sprites[1], drawPosition, 0, 1.01f);
            byte value = data.GetU8(0);

            Random random = new Random(x + value);
            for (int i = 1; i <= value; i++)
                spriteBatch.Rect(i == value ? crownSprites[random.Next(crownSprites.Length)] : trunkSprites[random.Next(trunkSprites.Length)], drawPosition - new Vec2(0, i) * map.TileSize, 0, 1.01f);
        }

        public override void Destroy(bool top, IMap map, int x, int y, TileData data)
        {
            byte value = data.GetU8(0);
            for (int i = 1; i <= value; i++)
                map.SetTile(true, null, x, y - i);
            new Item((Drop, value), new Vec2(x + .5f, y + .5f) * map.TileSize);
        }
    }
}
