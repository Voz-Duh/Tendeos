using XnaGame.PEntities;
using XnaGame.Utils.Graphics;

namespace XnaGame.Inventory
{
    public interface IItem
    {
        string Name { get; }
        string Description { get; }
        int MaxCount { get; }
        Sprite ItemSprite { get; }
        bool Flip { get; }
        bool Animated { get; }
        
        void Use(ITransform transform, ref byte armsState, ref float armLRotation, ref float armRRotation, ref int count, ref float timer, ArmData armData);
        void With(ITransform transform, byte armsState, float armLRotation, float armRRotation, ArmData armData);
    }
}
