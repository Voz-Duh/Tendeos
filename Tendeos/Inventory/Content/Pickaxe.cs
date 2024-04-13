using Tendeos.Utils;
using Tendeos.Utils.Input;
using Tendeos.World;

namespace Tendeos.Inventory.Content
{
    public class Pickaxe : MeleeWeapon
    {
        public float Power { get; set; }
        public float Radius { get; set; }

        public Pickaxe() : base()
        {
            CanRight = true;
        }

        public override void Attack(IMap map, Vec2 point)
        {
            base.Attack(map, point);
            map.MineTile(Mouse.LeftDown, map.World2Cell(point), Power, Radius);
        }
    }
}
