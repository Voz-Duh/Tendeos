using System;
using Tendeos.Inventory;
using Tendeos.Physical;
using Tendeos.Physical.Content;
using Tendeos.Utils;
using Tendeos.Utils.Graphics;

namespace Tendeos.World.Content
{
    public class TileTag : ITile
    {
        bool ITile.Multitile => false;
        public float Health => -1;
        public byte Hardness => 0;
        public string Tag => null;

        public string Folder
        {
            get => null;
            set { }
        }

        public string Name => null;
        public string Description => null;
        public int MaxCount => -1;
        public Sprite ItemSprite => default;
        public bool Flip => false;
        public bool Animated => false;
        public bool Wallable => false;
        public bool ShadowAvailable => false;
        public float ShadowIntensity => 0;
        public IItem Drop => null;
        public Range DropCount => 0..0;
        public bool Collision => false;
        public ITileInterface Interface { get; set; }

        public void Changed(bool top, IMap map, int x, int y, ref TileData data)
        {
        }

        public void Destroy(bool top, IMap map, int x, int y, TileData data)
        {
        }

        public void Draw(SpriteBatch spriteBatch, bool top, IMap map, int x, int y, Vec2 drawPosition, TileData data)
        {
        }

        public void Loaded(bool top, IMap map, int x, int y, ref TileData data)
        {
        }

        public void Start(bool top, IMap map, int x, int y, ref TileData data)
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
        }

        public void InArmDraw(
            SpriteBatch spriteBatch,
            IMap map, ITransform transform,
            byte armsState,
            float armLRotation,
            float armRRotation,
            ArmData armData)
        {
        }

        public void Use(IMap map, ref TileData data, Player player)
        {
        }

        public void Unuse(IMap map, ref TileData data, Player player)
        {
        }
    }
}