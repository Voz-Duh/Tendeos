
using XnaGame.Entities;
using XnaGame.Utils.Graphics;

namespace XnaGame.Inventory
{
    public interface IItem
    {
        string Name { get; }
        string Description { get; }
        int MaxCount { get; }
        Sprite ItemSprite { get; }

        void Use(ITransform transform, ref int count);
        void With(ITransform transform, ref byte armsState, ref float armsRotation);
    }
}
