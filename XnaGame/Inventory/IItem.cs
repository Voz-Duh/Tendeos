using Microsoft.Xna.Framework.Graphics;
using XnaGame.Physical;
using XnaGame.Utils;
using XnaGame.Utils.Graphics;
using XnaGame.World;

namespace XnaGame.Inventory
{
    public interface IItem : ITag
    {
        string Description { get; }
        int MaxCount { get; }
        Sprite ItemSprite { get; }
        bool Flip { get; }
        bool Animated { get; }

        void Use(IMap map, ITransform transform, ref byte armsState, ref float armLRotation, ref float armRRotation, ref int count, ref float timer, ArmData armData);
        void With(SpriteBatch spriteBatch, IMap map, ITransform transform, byte armsState, float armLRotation, float armRRotation, ArmData armData);
    }
}
