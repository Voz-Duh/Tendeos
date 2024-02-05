using Microsoft.Xna.Framework;
using System;
using XnaGame.PEntities;
using XnaGame.PEntities.Content;
using XnaGame.Utils;
using XnaGame.Utils.Graphics;
using XnaGame.Utils.Input;

namespace XnaGame.Inventory.Content
{
    public class Bow : IItem
    {
        public string Name => throw new NotImplementedException();
        public string Description => throw new NotImplementedException();
        public int MaxCount => 1;
        public Sprite ItemSprite { get; init; }
        public bool Flip => false;
        public bool Animated => true;

        private readonly Func<Projectile> projectile;
        private readonly (float min, float max) arrowOffset;
        private readonly float offset;
        private readonly float framerateScale;
        private readonly float power;
        private readonly Sprite[] sprites;

        public Bow(Func<Projectile> projectile, (float min, float max) arrowOffset, float offset, float power, Sprite[] sprites, Sprite item, float framerateScale = 1)
        {
            this.projectile = projectile;
            this.arrowOffset = arrowOffset;
            this.offset = offset;
            this.framerateScale = framerateScale;
            this.power = power;
            this.sprites = sprites;
            ItemSprite = item;
        }

        public void Use(ITransform transform, ref byte armsState, ref float armLRotation, ref float armRRotation, ref int count, ref float timer, ArmData armData)
        {
            int frame = 0;
            Vec2 basePosition = transform.Local2World(new Vec2(2, -4));
            Vec2 position = basePosition - Mouse.Position;
            armLRotation = MathHelper.ToDegrees(MathF.Atan2(position.Y, position.X)) + 90;
            position.Normalize();

            sprites.AnimationEnd(out frame, SpriteHelpers.frameRate * framerateScale, ref timer);
            
            if (Mouse.LeftUp)
            {
                if (frame > 0)
                    projectile().Spawn(basePosition, armLRotation + 90, power * timer);

                frame = 0;
                timer = 0;
            }

            Vec2 rp = transform.Local2World(new Vec2(-2, -4)) - (basePosition - position * arrowOffset.max);
            position = basePosition - position * MathHelper.Lerp(arrowOffset.max, arrowOffset.min, frame / (float)(sprites.Length-1));
            armRRotation = MathHelper.ToDegrees(MathF.Atan2(rp.Y, rp.X)) + 90;

            armsState = Player.GetState(frame, 1);

            armData.Set(
                ("frame", frame),
                ("position", position)
            );
        }

        public void With(ITransform transform, byte armsState, float armLRotation, float armRRotation, ArmData armData)
        {
            armData.Get(out int frame, "frame");
            armData.Get(out Vec2 position, "position");

            Vec2 offset = Vec2.UpOf(armLRotation) * this.offset;
            SDraw.Rect(sprites[frame], transform.Local2World(new Vec2(2, -4)) + offset, armLRotation + 90);
            SDraw.Rect(projectile().sprite, position + offset, armLRotation + 90, 1, 0, Origin.Zero);
        }
    }
}
