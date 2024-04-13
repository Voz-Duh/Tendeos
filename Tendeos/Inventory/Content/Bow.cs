using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Tendeos.Content.Utlis;
using Tendeos.Physical;
using Tendeos.Physical.Content;
using Tendeos.Utils;
using Tendeos.Utils.Graphics;
using Tendeos.Utils.Input;
using Tendeos.World;

namespace Tendeos.Inventory.Content
{
    public class Bow : IItem
    {
        public string Tag { get; set; }
        public string Folder { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public int MaxCount => 1;
        [SpriteLoad("@_item")]
        public Sprite ItemSprite { get; set; }
        public bool Flip => false;
        public bool Animated => true;

        [ContentLoad("Projectile", true)]
        public Projectile projectile;
        public string Projectile { get; set; }
        public (float min, float max) ArrowOffset { get; set; }
        public float Offset { get; set; }
        public float FramerateScale { get; set; } = 1;
        public float Power { get; set; }

        [SpriteLoad("@", 5, 1, 1)]
        public Sprite[] sprites;

        [GetName]
        public void GetName(string name)
        {
            if (Projectile == "") Projectile = "arrow";
        }

        public void Use(IMap map, ITransform transform, ref byte armsState, ref float armLRotation, ref float armRRotation, ref int count, ref float timer, ArmData armData)
        {
            Vec2 basePosition = transform.Local2World(new Vec2(2, -4));
            Vec2 position = basePosition - Mouse.Position;
            armLRotation = MathHelper.ToDegrees(MathF.Atan2(position.Y, position.X)) + 90;
            position.Normalize();

            sprites.AnimationEnd(out int frame, SpriteHelpers.frameRate * FramerateScale, ref timer);

            if (Mouse.OnGUI || Mouse.LeftUp)
            {
                if (frame > 0)
                    projectile.Spawn(basePosition, armLRotation + 90, Power * timer);

                frame = 0;
                timer = 0;
            }

            Vec2 rp = transform.Local2World(new Vec2(-2, -4)) - (basePosition - position * ArrowOffset.max);
            position = basePosition - position * MathHelper.Lerp(ArrowOffset.max, ArrowOffset.min, frame / (float)(sprites.Length - 1));
            armRRotation = MathHelper.ToDegrees(MathF.Atan2(rp.Y, rp.X)) + 90;

            armsState = Player.GetState(frame, 1);

            armData.Set(
                ("frame", frame),
                ("position", position)
            );
        }

        public void With(SpriteBatch spriteBatch, IMap map, ITransform transform, byte armsState, float armLRotation, float armRRotation, ArmData armData)
        {
            armData.Get(out int frame, "frame");
            armData.Get(out Vec2 position, "position");

            Vec2 offset = Vec2.UpOf(armLRotation) * this.Offset;
            spriteBatch.Rect(sprites[frame], transform.Local2World(new Vec2(2, -4)) + offset, armLRotation + 90);
            spriteBatch.Rect(projectile.sprite, position + offset, armLRotation + 90, 1, 0, Origin.Zero);
        }
    }
}
