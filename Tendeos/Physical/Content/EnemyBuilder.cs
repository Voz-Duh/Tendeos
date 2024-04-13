using Tendeos.Content.Utlis;
using Tendeos.Utils;
using Tendeos.World;

namespace Tendeos.Physical.Content
{
    public class EnemyBuilder
    {
        [ContentLoadable]
        public readonly IEnemyComponent[] components;
        public readonly float viewRadius;
        public readonly Vec2 size;
        public readonly bool seeAnytime;
        public readonly float spawnChance;
        public readonly float health;

        public EnemyBuilder(float spawnChance, float health, float viewRadius, Vec2 size, params IEnemyComponent[] components) : base()
        {
            this.components = components;
            this.viewRadius = viewRadius;
            this.size = size;
            seeAnytime = false;
            this.spawnChance = spawnChance;
            this.health = health;
        }

        public EnemyBuilder(float spawnChance, float health, float viewRadius, Vec2 size, bool seeAnytime, params IEnemyComponent[] components) : base()
        {
            this.components = components;
            this.viewRadius = viewRadius;
            this.size = size;
            this.seeAnytime = seeAnytime;
            this.spawnChance = spawnChance;
            this.health = health;
        }

        public Enemy Spawn(Vec2 position)
        {
            Enemy clone = new Enemy(position, this);
            EntityManager.Add(clone);
            return clone;
        }
    }
}
