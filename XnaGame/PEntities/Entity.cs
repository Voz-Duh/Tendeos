using nkast.Aether.Physics2D.Dynamics;
using XnaGame.Utils;

namespace XnaGame.PEntities
{
    public abstract class Entity
    {
        public void Remove() => Core.RemoveEntity(Draw, Update);

        public abstract void Draw();
        public abstract void Update();

        private void UpdateParent(Body body, FVector2 last)
        {
        }
    }
}
