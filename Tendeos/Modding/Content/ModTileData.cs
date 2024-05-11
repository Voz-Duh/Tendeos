
using Tendeos.World;

namespace Tendeos.Modding.Content
{
    public class ModTileData
    {
        public TileData tileData;

        public ModTileData(TileData tileData) => this.tileData = tileData;

        public float Health { get => tileData.Health; set => tileData.Health = value; }
        
        public uint u32(byte offset) => tileData.GetU32(offset);
        public void u32(byte offset, uint value) => tileData.SetU32(offset, value);
        public uint u24(byte offset) => tileData.GetU24(offset);
        public void u24(byte offset, uint value) => tileData.SetU24(offset, value);
        public ushort u16(byte offset) => tileData.GetU16(offset);
        public void u16(byte offset, ushort value) => tileData.SetU16(offset, value);
        public byte u8(byte offset) => tileData.GetU8(offset);
        public void u8(byte offset, byte value) => tileData.SetU8(offset, value);
        public byte u6(byte offset) => tileData.GetU6(offset);
        public void u6(byte offset, byte value) => tileData.SetU6(offset, value);
        public byte u2(byte offset) => tileData.GetU2(offset);
        public void u2(byte offset, byte value) => tileData.SetU2(offset, value);
        public bool boolean(byte offset) => tileData.GetBool(offset);
        public void boolean(byte offset, bool value) => tileData.SetBool(offset, value);
        public bool hasTriangleCollision { get => tileData.HasTriangleCollision; set => tileData.HasTriangleCollision = value; }
        public byte collisionXFrom { get => tileData.CollisionXFrom; set => tileData.CollisionXFrom = value; }
        public byte collisionYFrom { get => tileData.CollisionYFrom; set => tileData.CollisionYFrom = value; }
        public byte collisionXTo { get => tileData.CollisionXTo; set => tileData.CollisionXTo = value; }
        public byte collisionYTo { get => tileData.CollisionYTo; set => tileData.CollisionYTo = value; }
    } 
}