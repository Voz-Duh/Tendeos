
namespace XnaGame.Entities
{
    public abstract class Entity
    {
        public Entity() => Core.AddEntity(Draw, Update);

        public void Remove() => Core.RemoveEntity(Draw, Update);

        public abstract void Draw();
        public abstract void Update();
    }
}
