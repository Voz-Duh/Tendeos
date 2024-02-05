using Microsoft.Xna.Framework;
using System;
using XnaGame.State;
using XnaGame.Utils;
using XnaGame.Utils.Graphics;

namespace XnaGame.PEntities.Content
{
    public class Projectile : Entity
    {
        private Vec2 velocity;
        private Vec2 position;
        private readonly float speed;
        private readonly float damage;
        public readonly Sprite sprite;
        private Vec2 hitNormal;
        private float rotation;

        public Projectile(Vec2 position, float rotation, float speed, float damage, Sprite sprite)
        {
            this.position = position;
            this.speed = speed;
            velocity = Vec2.RightOf(rotation) * speed;
            this.damage = damage;
            this.sprite = sprite;
        }

        public Projectile Spawn(Vec2 position, float rotation, float power)
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
            Physics.Raycast((collider, point, normal, fraction) =>
            {
                if (collider == null)
                {
                    position = point;
                    collided = true;
                    hitNormal = normal;
                    velocity = Vec2.Zero;
                    return true;
                }
                if (collider.tag is Enemy enemy)
                {
                    position = point + normal * 0.1f;
                    Remove();
                    enemy.Hit(damage);
                    return true;
                }
                return false;
            }, position, velocity == Vec2.Zero ? -hitNormal : velocity * Time.Delta, true);
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
