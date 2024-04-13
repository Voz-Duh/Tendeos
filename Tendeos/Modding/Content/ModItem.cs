
using Microsoft.Xna.Framework.Graphics;
using Tendeos.Inventory;
using Tendeos.Physical;
using Tendeos.Utils.Graphics;
using Tendeos.World;

namespace Tendeos.Modding.Content
{
    public class ModItem : IModItem
    {
        public IModScript script { get; }

        public string Name { get; set; }
        public string Description { get; set; }

        public int MaxCount => throw new System.NotImplementedException();

        public Sprite ItemSprite => throw new System.NotImplementedException();

        public bool Flip => throw new System.NotImplementedException();

        public bool Animated => throw new System.NotImplementedException();

        public string Tag => throw new System.NotImplementedException();

        public string Folder { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        public ModItem(IModScript script)
        {
            this.script = script;
        }

        public void Use(IMap map, ITransform transform, ref byte armsState, ref float armLRotation, ref float armRRotation, ref int count, ref float timer, ArmData armData)
        {
            throw new System.NotImplementedException();
        }

        public void With(SpriteBatch spriteBatch, IMap map, ITransform transform, byte armsState, float armLRotation, float armRRotation, ArmData armData)
        {
            throw new System.NotImplementedException();
        }
    }
}
