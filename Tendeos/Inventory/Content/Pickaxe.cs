using Tendeos.Utils;
using Tendeos.Utils.Input;
using Tendeos.World;

namespace Tendeos.Inventory.Content
{
    public class Pickaxe : MeleeWeapon
    {
        public float Power;
        public float Radius;

        public Pickaxe()
        {
            CanRight = true;
        }

        public override void Attack(IMap map, Vec2 point)
        {
            base.Attack(map, point);
            map.MineTile(Controls.UpHit, map.World2Cell(point), Power, Radius);
        }
    }
}