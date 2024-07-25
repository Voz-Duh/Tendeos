using Microsoft.Xna.Framework;
using System;
using Tendeos.Content;
using Tendeos.Content.Utlis;
using Tendeos.Physical;
using Tendeos.Physical.Content;
using Tendeos.Utils;
using Tendeos.Utils.Graphics;
using Tendeos.Utils.Input;
using Tendeos.World;

namespace Tendeos.Inventory.Content
{
    public class MeleeWeapon : IItem
    {
        public string Tag { get; set; }
        public string Folder { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public int MaxCount => 1;
        [SpriteLoad("@_item")] public Sprite ItemSprite { get; set; }
        public bool Flip => false;
        public bool Animated => true;

        public float SwingAngle;
        public float SwingPerSecond;
        public byte State;
        public float Offset;
        public float AttackOffset;
        public float Damage;
        public float AttackRange;
        [SpriteLoad("@")] public Sprite sprite;
        public bool CanRight = false;

        private static bool side;

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
            armsState = State;
            Vec2 basePosition = transform.Local2World(new Vec2(0, -4));
            lookDirection.Normalize();
            float baseAngle = MathHelper.ToDegrees(MathF.Atan2(lookDirection.Y, lookDirection.X));
            float angle = baseAngle + (side ? SwingAngle : -SwingAngle) + 90;
            Vec2 armPosition = basePosition + Vec2.UpOf(angle) * Offset;

            Vec2 p = transform.Local2World(new Vec2(2, -4)) - armPosition;
            armLRotation = MathHelper.ToDegrees(MathF.Atan2(p.Y, p.X)) + 90;

            p = transform.Local2World(new Vec2(-2, -4)) - armPosition;
            armRRotation = MathHelper.ToDegrees(MathF.Atan2(p.Y, p.X)) + 90;

            if (leftDown || CanRight && rightDown)
            {
                timer += Time.Delta * SwingPerSecond;
                while (timer >= 1)
                {
                    Vec2 attackPosition = basePosition - lookDirection * AttackOffset;
                    Attack(map, attackPosition);
                    Effects.slashMedium.Spawn(attackPosition, baseAngle + 180);
                    side = !side;
                    timer--;
                }
            }
            else
            {
                timer = 0;
            }

            armData.Set(
                ("angle", angle),
                ("armPosition", armPosition)
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
            armData.Get(out Vec2 position, "armPosition");
            armData.Get(out float angle, "angle");

            spriteBatch.Rect(sprite, position, 1, angle + 90, 0);
        }

        public virtual void Attack(IMap map, Vec2 point)
        {
            foreach (Enemy enemy in EntityManager.GetEntities<Enemy>())
            {
                if (Vec2.Distance(enemy.Transform.Position, point) <= AttackRange * map.TileSize)
                {
                    enemy.Hit(Damage);
                }
            }
        }
    }
}