using Microsoft.Xna.Framework.Graphics;
using System;
using Tendeos.Inventory;
using Tendeos.Physical;
using Tendeos.Utils;
using Tendeos.Utils.Graphics;

namespace Tendeos.World.Content
{
    public class ReferenceTile : ITile
    {
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

        public int DataCount => throw new NotImplementedException();

        public string Folder { get => null; set { } }

        public void Changed(bool top, IMap map, int x, int y, TileData data)
        {
            TileData main = map.GetTile(top, BitConverter.ToInt32(data.Data), BitConverter.ToInt32(data.Data, 4));
            main.Tile?.Changed(top, map, x, y, main);
        }

        public void Draw(SpriteBatch spriteBatch, bool top, IMap map, int x, int y, Vec2 drawPosition, TileData data)
        {
            throw new NotImplementedException();
        }

        public void Start(bool top, IMap map, int x, int y, TileData data)
        {
            byte[] bytes = BitConverter.GetBytes(Next.x);
            data[0] = bytes[0];
            data[1] = bytes[1];
            data[2] = bytes[2];
            data[3] = bytes[3];
            bytes = BitConverter.GetBytes(Next.y);
            data[4] = bytes[0];
            data[5] = bytes[1];
            data[6] = bytes[2];
            data[7] = bytes[3];
        }

        public void Destroy(bool top, IMap map, int x, int y, TileData data)
        {
            throw new NotImplementedException();
        }

        public void Update(IMap map, int x, int y, TileData data)
        {
            throw new NotImplementedException();
        }

        public void Use(IMap map, ITransform transform, ref byte armsState, ref float armLRotation, ref float armRRotation, ref int count, ref float timer, ArmData armData)
        {
            throw new NotImplementedException();
        }

        public void With(SpriteBatch spriteBatch, IMap map, ITransform transform, byte armsState, float armLRotation, float armRRotation, ArmData armData)
        {
            throw new NotImplementedException();
        }

        public void Loaded(bool top, IMap map, int x, int y, TileData data)
        {
            throw new NotImplementedException();
        }
    }
}
