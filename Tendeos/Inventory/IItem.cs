using Tendeos.Physical;
using Tendeos.Utils;
using Tendeos.Utils.Graphics;
using Tendeos.World;

namespace Tendeos.Inventory
{
    public interface IItem : ITag
    {
        string Name { get; }
        string Description { get; }
        int MaxCount { get; }
        Sprite ItemSprite { get; }
        bool Flip { get; }
        bool Animated { get; }

        void InArmUpdate(
            IMap map, ITransform transform,
            Vec2 lookDirection,
            bool onGUI, bool leftDown, bool rightDown,
            ref byte armsState,
            ref float armLRotation,
            ref float armRRotation,
            ref int count,
            ref float timer,
            ArmData armData);

        void InArmDraw(
            SpriteBatch spriteBatch,
            IMap map, ITransform transform,
            byte armsState,
            float armLRotation,
            float armRRotation,
            ArmData armData);
    }
}