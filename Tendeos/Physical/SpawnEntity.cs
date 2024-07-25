using Tendeos.World;

namespace Tendeos.Physical
{
    public abstract class SpawnEntity : Entity
    {
        public SpawnEntity() => EntityManager.Add(this);
    }
}