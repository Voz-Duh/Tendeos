using Microsoft.Xna.Framework;
using System;
using XnaGame.Content;
using XnaGame.PEntities;
using XnaGame.PEntities.Content;
using XnaGame.Utils;
using XnaGame.Utils.Graphics;
using XnaGame.Utils.Input;
using XnaGame.WorldMap;

namespace XnaGame.Inventory.Content
{
    public class MeleeWeapon : IItem
    {
        public string Name => throw new NotImplementedException();
        public string Description => throw new NotImplementedException();
        public int MaxCount => 1;
        public Sprite ItemSprite { get; init; }
        public bool Flip => false;
        public bool Animated => true;

        private readonly float swingAngle;
        private readonly float swingPerSecond;
        private readonly byte state;
        private readonly float offset;
        private readonly float attackOffset;
        private readonly float damage;
        private readonly float attackRange;
        private readonly Sprite sprite;
        private static bool side;

        public MeleeWeapon(float swingAngle, float swingPerSecond, byte state, float offset, float attackOffset, float damage, float attackRange, Sprite sprite, Sprite item)
        {
            this.swingAngle = swingAngle;
            this.swingPerSecond = swingPerSecond;
            this.state = state;
            this.offset = offset;
            this.attackOffset = attackOffset;
            this.damage = damage;
            this.attackRange = attackRange * Map.tileSize;
            this.sprite = sprite;
            ItemSprite = item;
        }

        public void Use(ITransform transform, ref byte armsState, ref float armLRotation, ref float armRRotation, ref int count, ref float timer, ArmData armData)
        {
            armsState = state;
            FVector2 basePosition = transform.Local2World(new FVector2(0, -4));
            FVector2 lposition = basePosition - Mouse.Position;
            lposition.Normalize();
            float baseAngle = MathHelper.ToDegrees(MathF.Atan2(lposition.Y, lposition.X));
            float angle = baseAngle + (side ? swingAngle : -swingAngle) + 90;
            FVector2 armPosition = basePosition + FVector2.UpOf(angle) * offset;

            FVector2 p = transform.Local2World(new FVector2(2, -4)) - armPosition;
            armLRotation = MathHelper.ToDegrees(MathF.Atan2(p.Y, p.X)) + 90;

            p = transform.Local2World(new FVector2(-2, -4)) - armPosition;
            armRRotation = MathHelper.ToDegrees(MathF.Atan2(p.Y, p.X)) + 90;

            if (Mouse.LeftDown)
            {
                timer += Time.Delta * swingPerSecond;
                while (timer >= 1)
                {
                    FVector2 attackPosition = basePosition - lposition * attackOffset;
                    Attack(attackPosition);
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

        public void With(ITransform transform, byte armsState, float armLRotation, float armRRotation, ArmData armData)
        {
            armData.Get(out FVector2 position, "armPosition");
            armData.Get(out float angle, "angle");

            SDraw.Rect(sprite, position, angle + 90, 1, 0, Origin.Zero);
        }

        public virtual void Attack(FVector2 point)
        {
            foreach (Enemy enemy in Core.GetEntities<Enemy>())
            {
                if (FVector2.Distance(enemy.transform.Position, point) <= attackRange)
                {
                    enemy.Hit(damage);
                }
            }
        }
    }
}
