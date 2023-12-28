﻿using nkast.Aether.Physics2D.Dynamics;
using System;
using XnaGame.Utils;
using XnaGame.WorldMap;

namespace XnaGame.PEntities.Content
{
    public class Enemy : Entity
    {
        public readonly EnemyData data;
        private readonly IEnemyComponent[] components;
        private readonly float viewRadius;
        private readonly FVector2 size;
        private readonly bool seeAnytime;
        public readonly float maxHealth;
        public readonly BodyTransform transform;

        public FVector2 lastPlayerPosition;

        public byte state = 0;
        public bool baseState = false;
        public bool checkPosition = false;
        public float health;

        public Enemy(float health, float viewRadius, FVector2 size, params IEnemyComponent[] components)
        {
            this.components = components;
            this.viewRadius = viewRadius * Map.tileSize;
            this.size = size;
            seeAnytime = false;
            maxHealth = health;
            this.health = health;
        }

        public Enemy(float health, float viewRadius, FVector2 size, bool seeAnytime, params IEnemyComponent[] components)
        {
            this.components = components;
            this.viewRadius = viewRadius * Map.tileSize;
            this.size = size;
            this.seeAnytime = seeAnytime;
            maxHealth = health;
            this.health = health;
        }

        private Enemy(FVector2 position, IEnemyComponent[] components, float health, float viewRadius, FVector2 size, bool seeAnytime = false)
        {
            Body body = new Body();
            Fixture fixture = body.CreateSmoothRectangle(1, size.X, size.Y, 1, FVector2.Zero);
            fixture.Friction = 0;
            fixture.CollisionCategories = Category.Cat2;
            fixture.CollidesWith = Category.Cat1;

            body.BodyType = BodyType.Dynamic;
            body.FixedRotation = true;
            body.Tag = this;
            body.SleepingAllowed = false;
            body.Position = position;
            Core.world.Add(body);
            transform = new BodyTransform(body);
            data = new EnemyData();
            this.components = components;
            this.viewRadius = viewRadius;
            this.size = size;
            this.seeAnytime = seeAnytime;
            maxHealth = health;
            this.health = health;
        }

        public Enemy Spawn(FVector2 position)
        {
            var clone = new Enemy(position, components, health, viewRadius, size, seeAnytime);
            Core.AddEntity(clone.Draw, clone.Update);
            return clone;
        }

        public override void Draw()
        {
            foreach (IEnemyComponent component in components)
            {
                data.current = component.GetType().Name;
                component.Draw(this, data);
            }
        }

        public override void Update()
        {
            if (seeAnytime) baseState = true;
            else if (FVector2.Distance(Core.player.transform.Position, transform.Position) <= viewRadius)
            {
                bool see = true;
                Core.world.RayCast((fixture, point, normal, fraction) =>
                    {
                        if (fixture.Body.Tag is IMap)
                        {
                            see = false;
                            return 0;
                        }
                        return -1;
                    },
                    transform.Position,
                    Core.player.transform.Position);
                if (baseState = see)
                {
                    checkPosition = true;
                    lastPlayerPosition = Core.player.transform.Position;
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
            foreach (IEnemyComponent component in components)
            {
                data.current = component.GetType().Name;
                component.CheckState(this, data);
            }
            if (FVector2.Distance(transform.Position, lastPlayerPosition) < MathF.Max(size.X, size.Y))
                checkPosition = false;
        }

        public virtual void IdleState()
        {
            foreach (IEnemyComponent component in components)
            {
                data.current = component.GetType().Name;
                component.IdleState(this, data);
            }
        }

        public virtual void AgressState()
        {
            foreach (IEnemyComponent component in components)
            {
                data.current = component.GetType().Name;
                component.AgressState(this, data);
            }
        }

        public virtual void Hit(float damage)
        {
            health -= damage;
            foreach (IEnemyComponent component in components)
            {
                data.current = component.GetType().Name;
                component.OnHit(damage, this, data);
            }
            if (health <= 0)
            {
                Die();
            }
        }

        public virtual void Die()
        {
            transform.body.World.Remove(transform.body);
            Remove();
            foreach (IEnemyComponent component in components)
            {
                data.current = component.GetType().Name;
                component.OnDie(this, data);
            }
        }
    }
}
