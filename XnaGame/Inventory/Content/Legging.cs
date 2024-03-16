using Microsoft.Xna.Framework.Graphics;
using XnaGame.Content.Utlis;
using XnaGame.Physical;
using XnaGame.Utils;
using XnaGame.Utils.Graphics;
using XnaGame.World;

namespace XnaGame.Inventory.Content
{
    public class Legging : IItem
    {
        public string Name { get; set; }
        public string Folder { get; set; }
        public string Description { get; set; }

        public int MaxCount => 1;

        [SpriteLoad("@")]
        public Sprite ItemSprite { get; set; }

        public bool Flip => true;

        public bool Animated => false;

        public void Use(IMap map, ITransform transform, ref byte armsState, ref float armLRotation, ref float armRRotation, ref int count, ref float timer, ArmData armData)
        {
            armsState = 1;
        }

        public void With(SpriteBatch spriteBatch, IMap map, ITransform transform, byte armsState, float armLRotation, float armRRotation, ArmData armData)
        {
            spriteBatch.Rect(ItemSprite, transform.Local2World(new Vec2(2, -2)));
        }
    }
}
