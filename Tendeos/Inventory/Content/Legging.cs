using Tendeos.Content.Utlis;
using Tendeos.Physical;
using Tendeos.Utils;
using Tendeos.Utils.Graphics;
using Tendeos.World;

namespace Tendeos.Inventory.Content
{
    public class Legging : IItem
    {
        public string Tag { get; set; }
        public string Folder { get; set; } = "items/clothes";
        public string Name { get; set; }
        public string Description { get; set; }

        public int MaxCount => 1;

        [SpriteLoad("@_item")] public Sprite ItemSprite { get; set; }

        [SpriteLoad("@"), SplitSprite(1, 2, 1), SplitSprite(8, 1, 1)]
        public Sprite[][] Sprite { get; set; }

        public Sprite[][] MoveSprites { get; private set; }
        public Sprite[][] IdleSprites { get; private set; }

        public bool Flip => true;

        public bool Animated => false;

        public void InArmUpdate(
            IMap map, ITransform transform,
            Vec2 lookDirection,
            bool onGUI, bool leftDown, bool rightDown,
            ref byte armsState,
            ref float armLRotation,
            ref float armRRotation,
            ref int count,
            ref float timer,
            ArmData armData)
        {
            armsState = 1;
        }

        public void InArmDraw(
            SpriteBatch spriteBatch,
            IMap map, ITransform transform,
            byte armsState,
            float armLRotation,
            float armRRotation,
            ArmData armData)
        {
            spriteBatch.Rect(ItemSprite, transform.Local2World(new Vec2(2, -2)));
        }

        public void OnContentLoaded()
        {
            MoveSprites = new Sprite[][] {Sprite[0][2..8], Sprite[1][2..8]};
            IdleSprites = new Sprite[][] {Sprite[0][0..2], Sprite[1][0..2]};
        }
    }
}