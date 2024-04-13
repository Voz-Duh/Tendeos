using Microsoft.Xna.Framework.Graphics;
using System;
using Tendeos.Utils;

namespace Tendeos.Physical.Content
{
    public class Enemy : Entity
    {
        public readonly EnemyData data;
        public readonly EnemyBuilder builder;
        public readonly BodyTransform transform;
        public Vec2 lastPlayerPosition;

        public byte state = 0;
        public bool baseState = false;
        public bool checkPosition = false;
        public float health;

        public Enemy(Vec2 position, EnemyBuilder builder) : base()
        {
            Collider collider = Physics.Create(builder.size.X, builder.size.Y, 1, 0);

            collider.tag = this;
            collider.position = position;

            transform = new BodyTransform(collider);
            data = new EnemyData();
            health = builder.health;
            this.builder = builder;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < builder.components.Length; i++)
            {
                data.current = i;
                builder.components[i].Draw(spriteBatch, this, data);
            }
        }

        public override void Update()
        {
            if (builder.seeAnytime) baseState = true;
            else if (Vec2.Distance(Core.Player.transform.Position, transform.Position) <= builder.viewRadius)
            {
                bool see = true;
                if (!builder.seeAnytime) Physics.LinecastMap((_, _, _, _, _) => see = false,
                    transform.Position,
                    Core.Player.transform.Position);
                if (baseState = see)
                {
                    checkPosition = true;
                    lastPlayerPosition = Core.Player.transform.Position;
                }
            }
            else baseState = false;

            if (baseState)
                AgressState();
            else if (!checkPosition)
                IdleState();
            else
                CheckState();
        }

        public virtual void CheckState()
        {
            for (int i = 0; i < builder.components.Length; i++)
            {
                data.current = i;
                builder.components[i].CheckState(this, data);
            }
            if (Vec2.Distance(transform.Position, lastPlayerPosition) < MathF.Max(builder.size.X, builder.size.Y))
                checkPosition = false;
        }

        public virtual void IdleState()
        {
            for (int i = 0; i < builder.components.Length; i++)
            {
                data.current = i;
                builder.components[i].IdleState(this, data);
            }
        }

        public virtual void AgressState()
        {
            for (int i = 0; i < builder.components.Length; i++)
            {
                data.current = i;
                builder.components[i].AgressState(this, data);
            }
        }

        public virtual void Hit(float damage)
        {
            health -= damage;
            for (int i = 0; i < builder.components.Length; i++)
            {
                data.current = i;
                builder.components[i].OnHit(damage, this, data);
            }
            if (health <= 0)
            {
                Die();
            }
        }

        public virtual void Die()
        {
            Physics.Destroy(transform.body);
            Remove();
            for (int i = 0; i < builder.components.Length; i++)
            {
                data.current = i;
                builder.components[i].OnDie(this, data);
            }
        }
    }
}
