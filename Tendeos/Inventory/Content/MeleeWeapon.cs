using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
        [SpriteLoad("@_item")]
        public Sprite ItemSprite { get; set; }
        public bool Flip => false;
        public bool Animated => true;

        public float SwingAngle { get; set; }
        public float SwingPerSecond { get; set; }
        public byte State { get; set; }
        public float Offset { get; set; }
        public float AttackOffset { get; set; }
        public float Damage { get; set; }
        public float AttackRange { get; set; }
        [SpriteLoad("@")]
        public Sprite sprite;
        public bool CanRight { get; set; } = false;

        private static bool side;

        public void Use(IMap map, ITransform transform, ref byte armsState, ref float armLRotation, ref float armRRotation, ref int count, ref float timer, ArmData armData)
        {
            armsState = State;
            Vec2 basePosition = transform.Local2World(new Vec2(0, -4));
            Vec2 lposition = basePosition - Mouse.Position;
            lposition.Normalize();
            float baseAngle = MathHelper.ToDegrees(MathF.Atan2(lposition.Y, lposition.X));
            float angle = baseAngle + (side ? SwingAngle : -SwingAngle) + 90;
            Vec2 armPosition = basePosition + Vec2.UpOf(angle) * Offset;

            Vec2 p = transform.Local2World(new Vec2(2, -4)) - armPosition;
            armLRotation = MathHelper.ToDegrees(MathF.Atan2(p.Y, p.X)) + 90;

            p = transform.Local2World(new Vec2(-2, -4)) - armPosition;
            armRRotation = MathHelper.ToDegrees(MathF.Atan2(p.Y, p.X)) + 90;

            if (!Mouse.OnGUI && (Mouse.LeftDown || (CanRight && Mouse.RightDown)))
            {
                timer += Time.Delta * SwingPerSecond;
                while (timer >= 1)
                {
                    Vec2 attackPosition = basePosition - lposition * AttackOffset;
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

        public void With(SpriteBatch spriteBatch, IMap map, ITransform transform, byte armsState, float armLRotation, float armRRotation, ArmData armData)
        {
            armData.Get(out Vec2 position, "armPosition");
            armData.Get(out float angle, "angle");

            spriteBatch.Rect(sprite, position, angle + 90, 1, 0, Origin.Zero);
        }

        public virtual void Attack(IMap map, Vec2 point)
        {
            foreach (Enemy enemy in EntityManager.GetEntities<Enemy>())
            {
                if (Vec2.Distance(enemy.transform.Position, point) <= AttackRange * map.TileSize)
                {
                    enemy.Hit(Damage);
                }
            }
        }
    }
}
