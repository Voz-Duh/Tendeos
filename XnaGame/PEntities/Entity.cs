
namespace XnaGame.PEntities
{
    public abstract class Entity
    {
        public void Remove() => Core.RemoveEntity(Draw, Update);

        public abstract void Draw();
        public abstract void Update();
    }
}
