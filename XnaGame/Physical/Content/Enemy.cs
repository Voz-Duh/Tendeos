using Microsoft.Xna.Framework.Graphics;
using System;
using XnaGame.Content.Utlis;
using XnaGame.Utils;
using XnaGame.World;

namespace XnaGame.Physical.Content
{
    public class Enemy : Entity
    {
        private readonly BodyTransform target;

        public readonly EnemyData data;
        [ContentLoadable]
        public readonly IEnemyComponent[] components;
        private readonly float viewRadius;
        private readonly Vec2 size;
        private readonly bool seeAnytime;
        public readonly float maxHealth;
        public readonly BodyTransform transform;

        public Vec2 lastPlayerPosition;

        public byte state = 0;
        public bool baseState = false;
        public bool checkPosition = false;
        public float health;

        public Enemy(float health, float viewRadius, Vec2 size, params IEnemyComponent[] components) : base()
        {
            this.components = components;
            this.viewRadius = viewRadius;
            this.size = size;
            seeAnytime = false;
            maxHealth = health;
            this.health = health;
        }

        public Enemy(float health, float viewRadius, Vec2 size, bool seeAnytime, params IEnemyComponent[] components) : base()
        {
            this.components = components;
            this.viewRadius = viewRadius;
            this.size = size;
            this.seeAnytime = seeAnytime;
            maxHealth = health;
            this.health = health;
        }

        private Enemy(Vec2 position, IEnemyComponent[] components, float health, float viewRadius, Vec2 size, bool seeAnytime = false) : base()
        {
            target = EntityManager.GetEntity<Player>().transform;
            Collider collider = Physics.Create(size.X, size.Y, 1, 0);

            collider.tag = this;
            collider.position = position;

            transform = new BodyTransform(collider);
            data = new EnemyData();
            this.components = components;
            this.viewRadius = viewRadius;
            this.size = size;
            this.seeAnytime = seeAnytime;
            maxHealth = health;
            this.health = health;
        }

        public Enemy Spawn(Vec2 position)
        {
            var clone = new Enemy(position, components, health, viewRadius, size, seeAnytime);
            EntityManager.Add(clone);
            return clone;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < components.Length; i++)
            {
                data.current = i;
                components[i].Draw(spriteBatch, this, data);
            }
        }

        public override void Update()
        {
            if (seeAnytime) baseState = true;
            else if (Vec2.Distance(target.Position, transform.Position) <= viewRadius)
            {
                bool see = true;
                if (!seeAnytime) Physics.LinecastMap((_, _, _, _, _) => see = false,
                    transform.Position,
                    target.Position);
                if (baseState = see)
                {
                    checkPosition = true;
                    lastPlayerPosition = target.Position;
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
            for (int i = 0; i < components.Length; i++)
            {
                data.current = i;
                components[i].CheckState(this, data);
            }
            if (Vec2.Distance(transform.Position, lastPlayerPosition) < MathF.Max(size.X, size.Y))
                checkPosition = false;
        }

        public virtual void IdleState()
        {
            for (int i = 0; i < components.Length; i++)
            {
                data.current = i;
                components[i].IdleState(this, data);
            }
        }

        public virtual void AgressState()
        {
            for (int i = 0; i < components.Length; i++)
            {
                data.current = i;
                components[i].AgressState(this, data);
            }
        }

        public virtual void Hit(float damage)
        {
            health -= damage;
            for (int i = 0; i < components.Length; i++)
            {
                data.current = i;
                components[i].OnHit(damage, this, data);
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
            for (int i = 0; i < components.Length; i++)
            {
                data.current = i;
                components[i].OnDie(this, data);
            }
        }
    }
}
