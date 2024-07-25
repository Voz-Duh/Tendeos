using Microsoft.Xna.Framework;
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
        [SpriteLoad("@_item")] public Sprite ItemSprite { get; set; }
        public bool Flip => false;
        public bool Animated => true;

        [ContentLoad("Projectile", true)] public Projectile projectile;
        public string Projectile { get; set; }
        public (float min, float max) ArrowOffset { get; set; }
        public float Offset { get; set; }
        public float FramerateScale { get; set; } = 1;
        public float Power { get; set; }

        [SpriteLoad("@"), SplitSprite(5, 1, 1)]
        public Sprite[] sprites;

        [GetName]
        public void GetName(string name)
        {
            if (string.IsNullOrEmpty(Projectile)) Projectile = "arrow";
        }

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
            // calculate base position and left arm rotation
            Vec2 basePosition = transform.Local2World(new Vec2(2, -4));
            armLRotation = MathHelper.ToDegrees(MathF.Atan2(lookDirection.Y, lookDirection.X)) + 90;
            lookDirection.Normalize();

            // handle animation end and update timer
            sprites.AnimationEnd(out int frame, SpriteHelper.frameRate * FramerateScale, ref timer);

            // check mouse events and spawn projectile
            if (onGUI || !leftDown)
            {
                if (frame > 0)
                    projectile.Spawn(basePosition, armLRotation + 90, Power * timer);

                frame = 0;
                timer = 0;
            }

            // calculate right arm position and its rotation
            Vec2 rightPosition = transform.Local2World(new Vec2(-2, -4)) - (basePosition - lookDirection * ArrowOffset.max);
            Vec2 position = basePosition - lookDirection *
                MathHelper.Lerp(ArrowOffset.max, ArrowOffset.min, frame / (float)(sprites.Length - 1));
            armRRotation = MathHelper.ToDegrees(MathF.Atan2(rightPosition.Y, rightPosition.X)) + 90;

            // update arms state
            armsState = Player.GetState(frame, 1);

            // update arm data
            armData.Set(
                ("frame", frame),
                ("position", position)
            );
        }

        public void InArmDraw(
            SpriteBatch spriteBatch,
            IMap map, ITransform transform,
            byte armsState,
            float armLRotation,
            float armRRotation,
            ArmData armData)
        {
            // retrieve arm data
            armData.Get(out int frame, "frame");
            armData.Get(out Vec2 position, "position");

            // calculate offset
            Vec2 offset = Vec2.UpOf(armLRotation) * Offset;

            // draw sprites
            spriteBatch.Rect(sprites[frame], transform.Local2World(new Vec2(2, -4)) + offset, 1, armLRotation + 90);
            spriteBatch.Rect(projectile.sprite, position + offset, 1, armLRotation + 90, 0);
        }
    }
}