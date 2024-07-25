using Microsoft.Xna.Framework;
using System;
using Tendeos.Content.Utlis;
using Tendeos.Inventory;
using Tendeos.Physical;
using Tendeos.Physical.Content;
using Tendeos.Utils;
using Tendeos.Utils.Graphics;
using Tendeos.Utils.Input;

namespace Tendeos.World.Content
{
    public class Tile : ITile
    {
        public virtual bool Multitile => false;
        public static readonly Color dark = new Color(0.56f, 0.5f, 0.51f);
        public static readonly Color scheme = new HEXColor(0xADE7FF9B);
        public static readonly Color invalideScheme = new HEXColor(0xFFADAD9B);

        public bool Collision { get; set; } = true;

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
        public float ShadowIntensity { get; set; } = 0.5f;

        public string DropTag;
        [ContentLoad("DropTag", true)] public IItem Drop { get; protected set; }
        public Range DropCount { get; set; } = 1..1;
        public ITileInterface Interface { get; set; }

        public virtual void Changed(bool top, IMap map, int x, int y, ref TileData data)
        {
        }

        public virtual void Draw(SpriteBatch spriteBatch, bool top, IMap map, int x, int y, Vec2 drawPosition,
            TileData data)
        {
        }

        public virtual void DrawScheme(SpriteBatch spriteBatch, Vec2 drawPosition, bool valid)
        {
        }

        public virtual void Start(bool top, IMap map, int x, int y, ref TileData data)
        {
        }

        public virtual void Loaded(bool top, IMap map, int x, int y, ref TileData data)
        {
        }

        public virtual void Destroy(bool top, IMap map, int x, int y, TileData data)
        {
        }

        public virtual void Use(IMap map, ref TileData data, Player player)
        {
        }

        public virtual void Unuse(IMap map, ref TileData data, Player player)
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

            if (!Mouse.OnGUI)
            {
                if (Mouse.LeftDown)
                    if (map.TryPlaceTile(true, this, map.World2Cell(Mouse.Position)))
                        count -= 1;
                if (Mouse.RightDown)
                    if (map.TryPlaceTile(false, this, map.World2Cell(Mouse.Position)))
                        count -= 1;
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
            var cell = map.World2Cell(Mouse.Position);
            DrawScheme(spriteBatch, map.Cell2World(cell), map.CanPlaceTile(true, cell));
        }
    }
}