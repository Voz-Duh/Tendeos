using Tendeos.Utils;
using Tendeos.Utils.Graphics;
using Tendeos.World;

namespace Tendeos.Physical
{
    public abstract class Entity
    {
        public bool ViewlessDispose;
        public uint ID;
        
        public abstract Vec2 Position { get; }

        public void Remove() => EntityManager.Remove(this);

        public abstract void Draw(SpriteBatch spriteBatch);
        public abstract void Update();

        public abstract byte[] NetworkSend();
        public abstract void NetworkAccept(byte[] data);
    }
}