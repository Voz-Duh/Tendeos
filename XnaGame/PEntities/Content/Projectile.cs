using Microsoft.Xna.Framework;
using System;
using XnaGame.State;
using XnaGame.Utils;
using XnaGame.Utils.Graphics;
using XnaGame.WorldMap;

namespace XnaGame.PEntities.Content
{
    public class Projectile : Entity
    {
        private FVector2 velocity;
        private FVector2 position;
        private readonly float speed;
        private readonly float damage;
        public readonly Sprite sprite;
        private FVector2 hitNormal;
        private float rotation;

        public Projectile(FVector2 position, float rotation, float speed, float damage, Sprite sprite)
        {
            this.position = position;
            this.speed = speed;
            velocity = Matrix.CreateRotationZ(MathHelper.ToRadians(rotation)).Right * speed;
            this.damage = damage;
            this.sprite = sprite;
        }

        public Projectile Spawn(FVector2 position, float rotation, float power)
        {
            var clone = new Projectile(position, rotation, speed * power, damage, sprite);
            Core.AddEntity(clone.Draw, clone.Update);
            return clone;
        }

        public override void Draw()
        {
            SDraw.Rect(sprite, position, rotation, 1, 0, Origin.One);
        }

        public override void Update()
        {
            bool collided = false;
            Core.world.RayCast((fixture, point, normal, fraction) =>
            {
                if (fixture.Body.Tag is IMap)
                {
                    position = point + normal * 0.1f;
                    collided = true;
                    hitNormal = normal;
                    velocity = FVector2.Zero;
                    return 0;
                }

                if (fixture.Body.Tag is Enemy enemy)
                {
                    position = point + normal * 0.1f;
                    Remove();
                    enemy.Hit(damage);
                    return 0;
                }

                return -1;
            }, position, position + (velocity == FVector2.Zero ? -hitNormal : velocity * Time.Delta));
            if (!collided)
            {
                position += velocity * Time.Delta;
                velocity.Y += InGameState.gravity * Time.Delta;
                velocity.X -= velocity.X * MathF.Min(Time.Delta, 1);
                rotation = MathHelper.ToDegrees(MathF.Atan2(velocity.Y, velocity.X));
            }
        }
    }
}
