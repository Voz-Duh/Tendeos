using Microsoft.Xna.Framework.Content;
using XnaGame.Physical;
using XnaGame.Physical.Content;
using XnaGame.Physical.Content.EnemyComponents;
using XnaGame.Utils;

namespace XnaGame.Content
{
    public static class Entities
    {
        public static Projectile arrow;
        public static Enemy dummy;

        public static void Init(ContentManager content)
        {
            arrow = new Projectile()
            {
                Speed = 200,
                Damage = 1
            };
            dummy = new Enemy(10, 25, new Vec2(10, 20),
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
