using XnaGame.World;

namespace XnaGame.Physical
{
    public abstract class SpawnEntity : Entity
    {
        public SpawnEntity() => EntityManager.Add(this);
    }
}
