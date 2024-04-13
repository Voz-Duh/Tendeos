using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Tendeos.Content.Utlis;
using Tendeos.Inventory;
using Tendeos.Physical;
using Tendeos.Utils;
using Tendeos.Utils.Graphics;
using Tendeos.Utils.Input;

namespace Tendeos.World.Content
{
    public class Tile : ITile
    {
        public static readonly Color dark = new Color(0.56f, 0.5f, 0.51f);

        public bool Collision { get; set; } = true;

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
        public float ShadowIntensity { get; set; } = 0.5f;

        public string DropTag { get; set; }
        [ContentLoad("DropTag", true)]
        public IItem Drop { get; set; }
        public Range DropCount { get; set; } = 1..1;

        public int DataCount { get; protected set; }

        public virtual void Changed(bool top, IMap map, int x, int y, TileData data)
        {
        }

        public virtual void Draw(SpriteBatch spriteBatch, bool top, IMap map, int x, int y, Vec2 drawPosition, TileData data)
        {
        }

        public virtual void Start(bool top, IMap map, int x, int y, TileData data)
        {
        }

        public virtual void Loaded(bool top, IMap map, int x, int y, TileData data)
        {
        }

        public virtual void Destroy(bool top, IMap map, int x, int y, TileData data)
        {
        }

        public virtual void Update(IMap map, int x, int y, TileData data)
        {
        }

        public void Use(IMap map, ITransform transform, ref byte armsState, ref float armLRotation, ref float armRRotation, ref int count, ref float timer, ArmData armData)
        {
            armsState = 1;

            if (!Mouse.OnGUI)
            {
                if (Mouse.LeftDown)
                    if (map.TryPlaceTile(true, this, map.World2Cell(Mouse.Position))) count -= 1;
                if (Mouse.RightDown)
                    if (map.TryPlaceTile(false, this, map.World2Cell(Mouse.Position))) count -= 1;
            }
        }

        public void With(SpriteBatch spriteBatch, IMap map, ITransform transform, byte armsState, float armLRotation, float armRRotation, ArmData armData)
        {
            spriteBatch.Rect(ItemSprite, transform.Local2World(new Vec2(2, -2)));
        }
    }
}
