using System;
using System.Collections.Generic;
using Tendeos.Network;
using Tendeos.Utils;
using Tendeos.World;

namespace Tendeos.Synchronization
{
    public class EntitySynchronizer : INetworkSync
    {
        public void Accept(byte[] data)
        {
            EntityManager.Entities[BitConverter.ToUInt32(data)].NetworkAccept(data[4..]);
        }

        public byte[][] Send()
        {
            List<byte[]> objects = new List<byte[]>();

            for (uint i = 0; i < EntityManager.Entities.Max; i++)
            {
                if (EntityManager.Entities[i] != null)
                    objects.Add(new ByteBuffer()
                        .Append(i)
                        .Append(EntityManager.Entities[i].NetworkSend()));
            }

            return objects.ToArray();
        }
    }
}