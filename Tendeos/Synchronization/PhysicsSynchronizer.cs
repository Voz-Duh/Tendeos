using System.Collections.Generic;
using Tendeos.Network;
using Tendeos.Utils;

namespace Tendeos.Synchronization
{
    public class PhysicsSynchronizer : INetworkSync
    {
        public void Accept(byte[] data)
        {
            ByteBuffer buffer = new ByteBuffer(data);
            Collider collider = Physics.colliders[buffer.ReadUInt()].collider;
            if (collider != null)
            {
                buffer.Read(out collider.position.X).Read(out collider.position.Y)
                    .Read(out collider.velocity.X).Read(out collider.velocity.Y);
            }
        }

        public byte[][] Send()
        {
            List<byte[]> objects = new List<byte[]>();
            Collider collider;
            for (int i = 0; i < Physics.colliders.Max; i++)
            {
                collider = Physics.colliders[i].collider;
                if (collider != null)
                {
                    objects.Add(new ByteBuffer()
                        .Append(collider.index)
                        .Append(collider.position.X).Append(collider.position.Y)
                        .Append(collider.velocity.X).Append(collider.velocity.Y));
                }
            }

            return objects.ToArray();
        }
    }
}