using Microsoft.Xna.Framework.Content;
using Tendeos.Physical;
using Tendeos.Physical.Content;
using Tendeos.Physical.Content.EnemyComponents;
using Tendeos.Utils;

namespace Tendeos.Content
{
    public static class Entities
    {
        public static Projectile arrow;
        public static EnemyBuilder dummy;

        public static void Init(ContentManager content)
        {
            arrow = new Projectile()
            {
                Speed = 200,
                Damage = 1
            };
            dummy = new EnemyBuilder(
                spawnChance: 0,
                health: 10,
                viewRadius: 25,
                size: new Vec2(10, 20),
                new GroundComponent()
                {
                    Acceleration = 5,
                    MaxSpeed = 15,
                    Drag = 5
                },
                new LootDropComponent(chance: 75, 3..6, "test"));
        }

        public static Entity Get(string value) => (Entity)typeof(Entities).GetField(value).GetValue(null);
    }

    public delegate T EntityRef<T>() where T : Entity;
}
