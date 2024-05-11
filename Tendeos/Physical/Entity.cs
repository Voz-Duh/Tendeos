using Microsoft.Xna.Framework.Graphics;
using System;
using Tendeos.World;

namespace Tendeos.Physical
{
    public abstract class Entity
    {
        public uint ID;

        public void Remove() => EntityManager.Remove(this);

        public abstract void Draw(SpriteBatch spriteBatch);
        public abstract void Update();

        public abstract byte[] NetworkSend();
        public abstract void NetworkAccept(byte[] data);
    }
}
