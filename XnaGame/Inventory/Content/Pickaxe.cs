using XnaGame.Utils;
using XnaGame.Utils.Graphics;
using XnaGame.Utils.Input;

namespace XnaGame.Inventory.Content
{
    public class Pickaxe : MeleeWeapon
    {
        public float Power { get; set; }
        public float Radius { get; set; }

        public Pickaxe(Sprite sprite, Sprite item) : base(sprite, item)
        {
            CanRight = true;
        }

        public override void Attack(Vec2 point)
        {
            base.Attack(point);
            Core.map.MineTile(Mouse.LeftDown, Core.map.World2Cell(point), Power, Radius);
        }
    }
}
