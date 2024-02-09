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
        public float Speed { get; set; }
        public float Damage { get; set; }
        public readonly Sprite sprite;
        private Vec2 hitNormal;
        private float rotation;

        public Projectile(Sprite sprite)
        {
            velocity = Vec2.RightOf(rotation) * Speed;
            this.sprite = sprite;
        }

        public Projectile Spawn(Vec2 position, float rotation, float power)
        {
            var clone = new Projectile(sprite)
            {
                Speed = Speed * power,
                Damage = Damage,
                velocity = Vec2.RightOf(rotation) * Speed,
                position = position
            };
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
                    enemy.Hit(Damage);
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
