using System;
using Tendeos.Physical.Content.EnemyComponents;
using Tendeos.Utils;
using Tendeos.Utils.Graphics;

namespace Tendeos.Physical.Content
{
    public class Enemy : Entity
    {
        public readonly EnemyData Data;
        public readonly BodyTransform Transform;
        
        private readonly EnemyBuilder builder;
        
        
        public Vec2 LastPlayerPosition { get; private set; }
        public EnemyState State { get; private set; } = EnemyState.Idle;
        public EnemyState LastState { get; private set; } = EnemyState.Idle;
        
        public bool OnFloor { get; private set; }
        public float Health;
        
        private bool isAggressing = false;
        private bool isCheckingPosition = false;

        public override Vec2 Position => Transform.Position;

        public Enemy(Vec2 position, EnemyBuilder builder) : base()
        {
            Collider collider = Physics.Create(builder.size.X, builder.size.Y, 1, 0);

            collider.tag = this;
            collider.position = position;

            Transform = new BodyTransform(collider);
            Data = new EnemyData();
            Health = builder.health;
            this.builder = builder;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < builder.components.Length; i++)
            {
                Data.Current = i;

                builder.components[i].Draw(spriteBatch, this, Data);
            }
        }

        public override void Update()
        {
            OnFloor = Physics.Overlap(new FRectangle(
                Transform.Position.X - Transform.body.halfSize.X + 0.1f,
                Transform.Position.Y,
                Transform.body.halfSize.X * 2 - 0.2f,
                Transform.body.halfSize.Y + 0.1f));

            if (builder.seeAnytime)
            {
                isAggressing = true;
            }
            // TODO: Multiplayer check.
            else if (Vec2.Distance(Core.Player.transform.Position, Transform.Position) <= builder.viewRadius)
            {
                bool see = true;
                if (!builder.seeAnytime)
                {
                    Physics.LinecastMap((_, _, _, _, _) => see = false,
                        Transform.Position,
                        Core.Player.transform.Position);
                }

                if (isAggressing = see)
                {
                    isCheckingPosition = true;
                    LastPlayerPosition = Core.Player.transform.Position;
                }
            }
            else
            {
                isAggressing = false;
            }
            

            LastState = State;
            if (isAggressing)
            {
                State = EnemyState.Aggress;
                Transform.flipX = Transform.Position.X > LastPlayerPosition.X;
                AgressState();
            }
            else if (!isCheckingPosition)
            {
                State = EnemyState.Idle;
                IdleState();
            }
            else
            {
                State = EnemyState.Check;
                Transform.flipX = Transform.Position.X > LastPlayerPosition.X;
                CheckState();
            }
        }

        public virtual void CheckState()
        {
            for (int i = 0; i < builder.components.Length; i++)
            {
                Data.Current = i;
                builder.components[i].CheckState(this, Data);
            }

            if (Vec2.Distance(Transform.Position, LastPlayerPosition) < MathF.Max(builder.size.X, builder.size.Y))
            {
                isCheckingPosition = false;
            }
        }

        public virtual void IdleState()
        {
            for (int i = 0; i < builder.components.Length; i++)
            {
                Data.Current = i;
                builder.components[i].IdleState(this, Data);
            }
        }

        public virtual void AgressState()
        {
            for (int i = 0; i < builder.components.Length; i++)
            {
                Data.Current = i;

                builder.components[i].AggressState(this, Data);
            }
        }

        public virtual void Hit(float damage)
        {
            Health -= damage;
            for (int i = 0; i < builder.components.Length; i++)
            {
                Data.Current = i;
                builder.components[i].OnHit(damage, this, Data);
            }

            if (Health <= 0)
            {
                Die();
            }
        }

        public virtual void Die()
        {
            Physics.Destroy(Transform.body);
            Remove();
            for (int i = 0; i < builder.components.Length; i++)
            {
                Data.Current = i;
                builder.components[i].OnDie(this, Data);
            }
        }

        public override byte[] NetworkSend()
        {
            throw new NotImplementedException();
        }

        public override void NetworkAccept(byte[] data)
        {
            throw new NotImplementedException();
        }
    }
}