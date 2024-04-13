using Microsoft.Xna.Framework.Graphics;
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
        public string Folder { get; set; }
        public string Name { get; set; }
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
