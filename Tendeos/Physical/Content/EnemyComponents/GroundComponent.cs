using Tendeos.Content.Utlis;
using Tendeos.Utils;
using Tendeos.Utils.Graphics;

namespace Tendeos.Physical.Content.EnemyComponents
{
    public class GroundComponent : IEnemyComponent
    {
        public float Acceleration;
        public float MaxSpeed;
        public float Drag;

        public void Draw(SpriteBatch spriteBatch, Enemy enemy, EnemyData data)
        {
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

            enemy.Transform.body.velocity.X = speed;

            data.Set("speed", speed);
        }

        public void AggressState(Enemy enemy, EnemyData data)
        {
            Move(enemy, data);
        }

        public void Move(Enemy enemy, EnemyData data)
        {
            data.Get(out float speed, "speed");

            if (enemy.Transform.Position.X < enemy.LastPlayerPosition.X)
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

            enemy.Transform.body.velocity.X = speed;
            
            TransformUtils.StepEffect(enemy.OnFloor, enemy.Transform.flipX ? -1 : 1, enemy.Transform);

            data.Set("speed", speed);
        }

        public void OnHit(float damage, Enemy enemy, EnemyData data)
        {
        }

        public void OnDie(Enemy enemy, EnemyData data)
        {
        }
    }
}