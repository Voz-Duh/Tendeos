using Microsoft.Xna.Framework.Graphics;
using System;
using XnaGame.Content;
using XnaGame.Content.Utlis;
using XnaGame.Physical.Content;
using XnaGame.Utils;
using XnaGame.Utils.Graphics;

namespace XnaGame.World.Content
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
            DataCount = 1;
            DropCount = 0..0;
            DropTag = drop;
            Collision = false;
            ShadowAvailable = false;
        }

        public override void Start(bool top, IMap map, int x, int y, TileData data)
        {
            data[0] = (byte)URandom.SInt(Height.Start.Value, Height.End.Value);

            ReferenceTile.Next = (x, y);
            for (int i = 1; i <= data[0]; i++)
            {
                map.SetTile(true, Tiles.reference, x, y - i);
            }
        }

        public override void Loaded(bool top, IMap map, int x, int y, TileData data)
        {
            ReferenceTile.Next = (x, y);
            for (int i = 1; i <= data[0]; i++)
            {
                map.SetTile(true, Tiles.reference, x, y - i);
            }
        }

        public override void Draw(SpriteBatch spriteBatch, bool top, IMap map, int x, int y, Vec2 drawPosition, TileData data)
        {
            spriteBatch.Rect(sprites[1], drawPosition, 0, 1.01f);

            Random random = new Random(x + data[0]);
            for (int i = 1; i <= data[0]; i++)
                spriteBatch.Rect(i == data[0] ? crownSprites[random.Next(crownSprites.Length)] : trunkSprites[random.Next(trunkSprites.Length)], drawPosition - new Vec2(0, i) * map.TileSize, 0, 1.01f);
        }

        public override void Destroy(bool top, IMap map, int x, int y, TileData data)
        {
            for (int i = 1; i <= data[0]; i++)
                map.SetTile(true, null, x, y - i);
            new Item((Drop, data[0]), new Vec2(x + .5f, y + .5f) * map.TileSize);
        }
    }
}
