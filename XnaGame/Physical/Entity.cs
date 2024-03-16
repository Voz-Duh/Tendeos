using Microsoft.Xna.Framework.Graphics;
using XnaGame.World;

namespace XnaGame.Physical
{
    public abstract class Entity
    {
        public void Remove() => EntityManager.Remove(this);

        public abstract void Draw(SpriteBatch spriteBatch);
        public abstract void Update();
    }
}
