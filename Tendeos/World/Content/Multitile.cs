using System;
using System.Linq;
using Tendeos.Content;
using Tendeos.Content.Utlis;
using Tendeos.Inventory;
using Tendeos.Physical;
using Tendeos.Physical.Content;
using Tendeos.Utils;
using Tendeos.Utils.Graphics;
using Tendeos.Utils.Input;

namespace Tendeos.World.Content
{
    public class Multitile : ITile
    {
        bool ITile.Multitile => true;
        public bool Collision { get; set; }

        public string Tag { get; set; }
        public string Folder { get; set; } = "tiles";
        public string Name { get; set; }
        public string Description { get; set; }

        public int MaxCount { get; set; } = 100;
        [SpriteLoad("@_item")]
        public Sprite ItemSprite { get; protected set; }
        public bool Flip => true;
        public bool Animated => false;

        public float Health { get; set; } = 1;
        public byte Hardness { get; set; } = 0;

        public bool ShadowAvailable { get; set; } = true;
        public float ShadowIntensity { get; set; } = 0;

        public string DropTag { get; set; }
        [ContentLoad("DropTag", true)]
        public IItem Drop { get; protected set; }
        public Range DropCount { get; set; } = 1..1;

        public Vec2 DrawOffset { get; set; }
        public (int x, int y)[] References { get; set; }
        public (int x, int y, bool floorOnly, bool isWall)[] Required { get; set; }

        [SpriteLoad("@")] protected Sprite sprite;

        public ITileInterface Interface { get; set; }

        public virtual void Changed(bool top, IMap map, int x, int y, ref TileData data)
        {
        }

        public virtual void Draw(SpriteBatch spriteBatch, bool top, IMap map, int x, int y, Vec2 drawPosition,
            TileData data)
        {
            spriteBatch.Rect(sprite, drawPosition + DrawOffset);
        }

        public virtual void DrawScheme(SpriteBatch spriteBatch, Vec2 drawPosition, bool valid)
        {
            spriteBatch.Rect(valid ? Tile.scheme : Tile.invalideScheme, sprite, drawPosition + DrawOffset);
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

        public virtual void Use(IMap map, ref TileData data, Player player)
        {
        }

        public virtual void Unuse(IMap map, ref TileData data, Player player)
        {
        }

        public virtual void OnPlace(IMap map, ITransform transform, ref TileData data)
        {
        }

        public void InArmUpdate(
            IMap map, ITransform transform,
            Vec2 lookDirection,
            bool onGUI, bool leftDown, bool rightDown,
            ref byte armsState,
            ref float armLRotation,
            ref float armRRotation,
            ref int count,
            ref float timer,
            ArmData armData)
        {
            armsState = 1;

            if (!Mouse.OnGUI && Mouse.LeftDown)
            {
                var cell = map.World2Cell(Mouse.Position - DrawOffset);
                if (CanPlace(map, cell))
                {
                    map.SetTile(true, this, cell);
                    OnPlace(map, transform, ref map.GetTile(true, cell));
                    count -= 1;
                }
            }
        }

        public void InArmDraw(
            SpriteBatch spriteBatch,
            IMap map, ITransform transform,
            byte armsState,
            float armLRotation,
            float armRRotation,
            ArmData armData)
        {
            spriteBatch.Rect(ItemSprite, transform.Local2World(new Vec2(2, -2)));

            var cell = map.World2Cell(Mouse.Position - DrawOffset);
            DrawScheme(spriteBatch, map.Cell2World(cell), CanPlace(map, cell));
        }

        public bool CanPlace(IMap map, (int x, int y) cell)
        {
            if (map.CanSetTile(true, cell))
            {
                for (int i = 0; i < References.Length; i++)
                {
                    var (lx, ly) = References[i];
                    if (!map.CanSetTile(true, cell.x + lx, cell.y + ly))
                    {
                        return false;
                    }
                }

                for (int i = 0; i < Required.Length; i++)
                {
                    var (lx, ly, floor, wall) = Required[i];
                    if (wall && map.CanSetTile(false, cell.x + lx, cell.y + ly))
                    {
                        return false;
                    }

                    if (floor)
                    {
                        TileData data = map.GetTile(true, cell.x + lx, cell.y + ly);
                        if (data.Tile == null || !data.IsFloor)
                        {
                            return false;
                        }
                    }
                }

                return true;
            }

            return false;
        }
    }
}