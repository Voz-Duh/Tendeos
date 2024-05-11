using Microsoft.Xna.Framework.Graphics;
using System;
using Tendeos.Content;
using Tendeos.Content.Utlis;
using Tendeos.Inventory;
using Tendeos.Physical;
using Tendeos.Utils;
using Tendeos.Utils.Graphics;
using Tendeos.Utils.Input;

namespace Tendeos.World.Content
{
    public class Multitile : ITile
    {
        public bool Collision { get; set; }

        public string Tag { get; set; }
        public string Folder { get; set; } = "tiles";
        public string Name { get; set; }
        public string Description { get; set; }

        public int MaxCount { get; set; } = 100;
        [SpriteLoad("@_item")]
        public Sprite ItemSprite { get; set; }
        public bool Flip => true;
        public bool Animated => false;

        public float Health { get; set; } = 1;
        public byte Hardness { get; set; } = 0;

        public bool ShadowAvailable { get; set; } = true;
        public float ShadowIntensity { get; set; } = 0;

        public string DropTag { get; set; }
        [ContentLoad("DropTag", true)]
        public IItem Drop { get; set; }
        public Range DropCount { get; set; }

        public Vec2 DrawOffset { get; set; }
        public (int x, int y)[] References { get; set; }
        public bool FloorOnly { get; set; } = true;

        [SpriteLoad("@")]
        public Sprite sprite;

        object ITile.RealInterface { get; set; }
        TileInterface ITile.Interface { get; set; }

        public virtual void Changed(bool top, IMap map, int x, int y, ref TileData data)
        {
        }

        public virtual void Draw(SpriteBatch spriteBatch, bool top, IMap map, int x, int y, Vec2 drawPosition, TileData data)
        {
            spriteBatch.Rect(sprite, drawPosition + DrawOffset * map.TileSize);
        }

        public virtual void Start(bool top, IMap map, int x, int y, ref TileData data)
        {
            ReferenceTile.Next = (x, y);
            for (int i = 0; i < References.Length; i++)
            {
                var (lx, ly) = References[i];
                map.SetTile(true, Tiles.reference, x + lx, y + ly);
            }
        }

        public virtual void Loaded(bool top, IMap map, int x, int y, ref TileData data)
        {
            ReferenceTile.Next = (x, y);
            for (int i = 0; i < References.Length; i++)
            {
                var (lx, ly) = References[i];
                map.SetTile(true, Tiles.reference, x + lx, y + ly);
            }
        }

        public virtual void Destroy(bool top, IMap map, int x, int y, TileData data)
        {
            for (int i = 0; i < References.Length; i++)
            {
                var (lx, ly) = References[i];
                map.SetTile(true, null, x + lx, y + ly);
            }
        }

        public void Use(IMap map, ITransform transform, ref byte armsState, ref float armLRotation, ref float armRRotation, ref int count, ref float timer, ArmData armData)
        {
            armsState = 1;

            if (!Mouse.OnGUI && Mouse.LeftDown)
            {
                var (x, y) = map.World2Cell(Mouse.Position);
                if (map.CanSetTile(true, x, y))
                {
                    bool can = true;
                    for (int i = 0; i < References.Length; i++)
                    {
                        var (lx, ly) = References[i];
                        if (!map.CanSetTile(true, x + lx, y + ly))
                        {
                            can = false;
                            break;
                        }
                    }
                    if (can) map.SetTile(true, this, x, y);
                    count -= 1;
                }
            }
        }

        public void With(SpriteBatch spriteBatch, IMap map, ITransform transform, byte armsState, float armLRotation, float armRRotation, ArmData armData)
        {
            spriteBatch.Rect(ItemSprite, transform.Local2World(new Vec2(2, -2)));
        }
    }
}
