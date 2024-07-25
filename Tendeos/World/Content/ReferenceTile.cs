using System;
using Tendeos.Inventory;
using Tendeos.Physical;
using Tendeos.Physical.Content;
using Tendeos.Utils;
using Tendeos.Utils.Graphics;

namespace Tendeos.World.Content
{
    public class ReferenceTile : ITile
    {
        bool ITile.Multitile => false;
        public string Name { get; set; }
        public static (int x, int y) Next { private get; set; }

        public float Health => throw new NotImplementedException();

        public byte Hardness => throw new NotImplementedException();

        public IItem Drop => throw new NotImplementedException();

        public Range DropCount => throw new NotImplementedException();

        public string Tag => throw new NotImplementedException();

        public string Description => throw new NotImplementedException();

        public int MaxCount => throw new NotImplementedException();

        public Sprite ItemSprite => throw new NotImplementedException();

        public bool Flip => throw new NotImplementedException();

        public bool Animated => throw new NotImplementedException();

        public bool ShadowAvailable => throw new NotImplementedException();

        public float ShadowIntensity => throw new NotImplementedException();

        public bool Collision => throw new NotImplementedException();

        public string Folder
        {
            get => null;
            set { }
        }

        public ITileInterface Interface { get; set; }

        public void Changed(bool top, IMap map, int x, int y, ref TileData data)
        {
            TileData main = map.GetTile(top, (int) data.GetU32(0), (int) data.GetU32(32));
            main.Tile?.Changed(top, map, x, y, ref main);
        }

        public void Draw(SpriteBatch spriteBatch, bool top, IMap map, int x, int y, Vec2 drawPosition, TileData data)
        {
            throw new NotImplementedException();
        }

        public void Start(bool top, IMap map, int x, int y, ref TileData data)
        {
            data.SetU32(0, (uint) Next.x);
            data.SetU32(32, (uint) Next.y);
        }

        public void Destroy(bool top, IMap map, int x, int y, TileData data)
        {
            throw new NotImplementedException();
        }

        public void Update(IMap map, int x, int y, ref TileData data)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public void InArmDraw(
            SpriteBatch spriteBatch,
            IMap map, ITransform transform,
            byte armsState,
            float armLRotation,
            float armRRotation,
            ArmData armData)
        {
            throw new NotImplementedException();
        }

        public void Loaded(bool top, IMap map, int x, int y, ref TileData data)
        {
            throw new NotImplementedException();
        }

        public void Use(IMap map, ref TileData data, Player player)
        {
            throw new NotImplementedException();
        }

        public void Unuse(IMap map, ref TileData data, Player player)
        {
            throw new NotImplementedException();
        }
    }
}