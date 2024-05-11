using System;
using Tendeos.World.Content;

namespace Tendeos.World
{
    public struct TileData
    {
        private static readonly UInt128 n1 = 0b1;
        private static readonly UInt128 n2 = 0b11;
        private static readonly UInt128 n3 = 0b111;
        private static readonly UInt128 n6 = 0b111111;
        private static readonly UInt128 n7 = 0b1111111;
        private static readonly UInt128 n8 = 0b11111111;
        private static readonly UInt128 n16 = 0b1111111111111111;
        private static readonly UInt128 n24 = 0b111111111111111111111111;
        private static readonly UInt128 n32 = 0b11111111111111111111111111111111;

        public float Health { get; set; }
        
        public readonly uint GetU32(byte offset) => (uint)((data >> offset) & n32);
        public TileData SetU32(byte offset, uint value)
        {
            data &= ~(n32 << offset);
            data |= (value & n32) << offset;
            return this;
        }
        public readonly uint GetU24(byte offset) => (uint)((data >> offset) & n24);
        public TileData SetU24(byte offset, uint value)
        {
            data &= ~(n24 << offset);
            data |= (value & n24) << offset;
            return this;
        }
        public readonly ushort GetU16(byte offset) => (ushort)((data >> offset) & n16);
        public TileData SetU16(byte offset, ushort value)
        {
            data &= ~(n16 << offset);
            data |= (value & n16) << offset;
            return this;
        }
        public readonly byte GetU8(byte offset) => (byte)((data >> offset) & n8);
        public TileData SetU8(byte offset, byte value)
        {
            data &= ~(n8 << offset);
            data |= (value & n8) << offset;
            return this;
        }
        public readonly byte GetU7(byte offset) => (byte)((data >> offset) & n7);
        public TileData SetU7(byte offset, byte value)
        {
            data &= ~(n7 << offset);
            data |= (value & n7) << offset;
            return this;
        }
        public readonly byte GetU6(byte offset) => (byte)((data >> offset) & n6);
        public TileData SetU6(byte offset, byte value)
        {
            data &= ~(n6 << offset);
            data |= (value & n6) << offset;
            return this;
        }
        public readonly byte GetU3(byte offset) => (byte)((data >> offset) & n3);
        public TileData SetU3(byte offset, byte value)
        {
            data &= ~(n3 << offset);
            data |= (value & n3) << offset;
            return this;
        }
        public readonly byte GetU2(byte offset) => (byte)((data >> offset) & n2);
        public TileData SetU2(byte offset, byte value)
        {
            data &= ~(n2 << offset);
            data |= (value & n2) << offset;
            return this;
        }
        public readonly bool GetBool(byte offset) => ((data >> offset) & n1) == n1;
        public TileData SetBool(byte offset, bool value)
        {
            data &= ~(n1 << offset);
            if (value) data |= n1 << offset;
            return this;
        }
        public readonly bool IsReference
        {
            get => GetBool(127);
            init => SetBool(127, value);
        }
        public bool HasTriangleCollision
        {
            readonly get => GetBool(126);
            set => SetBool(126, value);
        }
        public byte CollisionXFrom
        {
            readonly get => GetU2(124);
            set => SetU2(124, value);
        }
        public byte CollisionYFrom
        {
            readonly get => GetU2(122);
            set => SetU2(122, value);
        }
        public byte CollisionXTo
        {
            readonly get => GetU2(120);
            set => SetU2(120, value);
        }
        public byte CollisionYTo
        {
            readonly get => GetU2(118);
            set => SetU2(118, value);
        }
        public byte CollisionXAdd
        {
            readonly get => GetU2(116);
            set => SetU2(116, value);
        }
        public byte CollisionYAdd
        {
            readonly get => GetU2(114);
            set => SetU2(114, value);
        }

        public UInt128 data = 0;
        public object Interface { get; init; }
        public ITile Tile { get; init; }

        public TileData()
        {
            Health = 0;
            Tile = null;
        }

        public TileData(ITile tile) : this()
        {
            if (tile == null) return;
            if (tile is ReferenceTile)
            {
                IsReference = true;
            }
            else
            {
                IsReference = false;
                CollisionXTo = 2;
                CollisionYTo = 2;
                Health = tile.Health;
                switch (tile.Interface)
                {
                    default:
                        break;
                    case TileInterface.CraftMenu:
                        Interface = tile.RealInterface;
                        break;
                    case TileInterface.Inventory:
                        Interface = ((Inventory.Inventory)tile.RealInterface).Copy();
                        break;
                }
            }
            Tile = tile;
        }
    }
}