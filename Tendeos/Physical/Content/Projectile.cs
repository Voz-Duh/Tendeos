using Microsoft.Xna.Framework;
using System;
using Tendeos.Content.Utlis;
using Tendeos.Utils;
using Tendeos.Utils.Graphics;
using Tendeos.World;

namespace Tendeos.Physical.Content
{
    public class Projectile : Entity
    {
        private Vec2 velocity;
        private Vec2 position;
        public float Speed { get; set; }
        public float Damage { get; set; }
        [SpriteLoad("@")] public Sprite sprite;
        private Vec2 hitNormal;
        private float rotation;

        public override Vec2 Position => position;

        public Projectile()
        {
            velocity = Vec2.RightOf(rotation) * Speed;
        }

        public Projectile Spawn(Vec2 position, float rotation, float power)
        {
            var clone = new Projectile
            {
                sprite = sprite,
                Speed = Speed * power,
                Damage = Damage,
                velocity = Vec2.RightOf(rotation) * Speed,
                position = position
            };
            EntityManager.Add(clone);
            return clone;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Rect(sprite, position, 1, rotation, 1);
        }

        public override void Update()
        {
            bool collided = false;
            Physics.Raycast((collider, point, normal, fraction) =>
            {
                if (collider == null)
                {
                    position = point + normal;
                    collided = true;
                    hitNormal = -normal;
                    velocity = Vec2.Zero;
                    return true;
                }
                else if (collider.tag is Enemy enemy)
                {
                    Remove();
                    enemy.Hit(Damage);
                    return true;
                }

                return false;
            }, position, velocity == Vec2.Zero ? hitNormal : velocity * Time.Delta, true);
            if (!collided)
            {
                position += velocity * Time.Delta;
                velocity += Physics.Gravity * Time.Delta * Physics.Meter;
                velocity.X -= velocity.X * MathF.Min(Time.Delta, 1);
                rotation = MathHelper.ToDegrees(MathF.Atan2(velocity.Y, velocity.X));
            }
        }

        public override byte[] NetworkSend()
        {
            // TODO: network send of projectile data
            throw new NotImplementedException();
        }

        public override void NetworkAccept(byte[] data)
        {
            // TODO: network accept of projectile data
            throw new NotImplementedException();
        }
    }
}