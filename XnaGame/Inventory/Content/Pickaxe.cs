using XnaGame.Utils;
using XnaGame.Utils.Graphics;

namespace XnaGame.Inventory.Content
{
    public class Pickaxe : MeleeWeapon
    {
        private readonly float power;
        private readonly float radius;

        public Pickaxe(float power, float radius, float swingAngle, float swingPerSecond, byte state, float offset, float attackOffset, float damage, float attackRange, Sprite sprite, Sprite item) : base(swingAngle, swingPerSecond, state, offset, attackOffset, damage, attackRange, sprite, item)
        {
            this.power = power;
            this.radius = radius;
        }

        public override void Attack(FVector2 point)
        {
            base.Attack(point);
            Core.map.MineTile(Core.map.World2Cell(point), power, radius);
        }
    }
}
