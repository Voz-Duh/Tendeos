﻿using XnaGame.Utils;
using XnaGame.Utils.Graphics;

namespace XnaGame.PEntities.Content.EnemyComponents
{
    public class GroundComponent : IEnemyComponent
    {
        private readonly Sprite sprite;
        public float Acceleration { get; set; }
        public float MaxSpeed { get; set; }
        public float Drag { get; set; }

        public GroundComponent(Sprite sprite)
        {
            this.sprite = sprite;
        }

        public void Draw(Enemy enemy, EnemyData data)
        {
            SDraw.Rect(sprite, enemy.transform.Position);
            SDraw.Text(Core.font, $"{(enemy.baseState ? "Agress" : (enemy.checkPosition ? "Check" : "Idle"))} | Health: {enemy.health}", enemy.transform.Position);
        }

        public void CheckState(Enemy enemy, EnemyData data)
        {
            Move(enemy, data);
        }

        public void IdleState(Enemy enemy, EnemyData data)
        {
            data.Get(out float speed, "speed");

            if (speed != 0)
                if (speed < 0)
                {
                    speed += Drag * Time.Delta;
                    if (speed > 0) speed = 0;
                }
                else 
                {
                    speed -= Drag * Time.Delta;
                    if (speed < 0) speed = 0;
                }
            enemy.transform.body.velocity.X = speed;

            data.Set("speed", speed);
        }

        public void AgressState(Enemy enemy, EnemyData data)
        {
            Move(enemy, data);
        }

        public void Move(Enemy enemy, EnemyData data)
        {
            data.Get(out float speed, "speed");

            if (enemy.transform.Position.X < enemy.lastPlayerPosition.X)
            {
                if (speed < 0)
                {
                    speed += Drag * Time.Delta;
                    if (speed > 0) speed = 0;
                }
                speed += Acceleration * Time.Delta;
                if (speed > MaxSpeed) speed = MaxSpeed;
            }
            else
            {
                if (speed > 0)
                {
                    speed -= Drag * Time.Delta;
                    if (speed < 0) speed = 0;
                }
                speed -= Acceleration * Time.Delta;
                if (speed < -MaxSpeed) speed = -MaxSpeed;
            }
            enemy.transform.body.velocity.X = speed;

            data.Set("speed", speed);
        }

        public void OnHit(float damage, Enemy enemy, EnemyData data) { }

        public void OnDie(Enemy enemy, EnemyData data) { }
    }
}
