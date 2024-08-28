using Tendeos.Content.Utlis;
using Tendeos.Physical;
using Tendeos.Physical.Content;
using Tendeos.Utils;
using Tendeos.Utils.Graphics;
using Tendeos.World;

namespace Tendeos.Inventory.Content
{
    public class Cuirass : IItem
    {
        public string Tag { get; set; }
        public string Folder { get; set; } = "items/clothes";
        public string Name { get; set; }
        public string Description { get; set; }

        public int MaxCount => 1;

        [SpriteLoad("@_item")] public Sprite ItemSprite { get; set; }

        [SpriteLoad("@_arm_l"), SplitSprite(1, 2, 1), SplitSprite(Player.ArmStates, Player.ArmStateLines, 1)]
        public Sprite[][] LeftArmSprite { get; set; }

        [SpriteLoad("@_arm_r"), SplitSprite(1, 2, 1), SplitSprite(Player.ArmStates, Player.ArmStateLines, 1)]
        public Sprite[][] RightArmSprite { get; set; }

        [SpriteLoad("@"), SplitSprite(2, 1, 1), SplitSprite(1, 4, 1), SplitSprite(4, 1, 1)]
        public Sprite[][][] BodySprite { get; set; }

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
    }
}