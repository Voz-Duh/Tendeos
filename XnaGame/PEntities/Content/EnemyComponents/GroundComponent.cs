using XnaGame.Utils;
using XnaGame.Utils.Graphics;

namespace XnaGame.PEntities.Content.EnemyComponents
{
    public class GroundComponent : IEnemyComponent
    {
        private readonly Sprite sprite;
        private readonly float acceleration;
        private readonly float maxSpeed;
        private readonly float drag;

        public GroundComponent(Sprite sprite, float acceleration, float maxSpeed, float drag)
        {
            this.sprite = sprite;
            this.acceleration = acceleration;
            this.maxSpeed = maxSpeed;
            this.drag = drag;
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
                    speed += drag * Time.Delta;
                    if (speed > 0) speed = 0;
                }
                else 
                {
                    speed -= drag * Time.Delta;
                    if (speed < 0) speed = 0;
                }
            enemy.transform.body.LinearVelocity = new FVector2(speed, enemy.transform.body.LinearVelocity.Y);

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
                    speed += drag * Time.Delta;
                    if (speed > 0) speed = 0;
                }
                speed += acceleration * Time.Delta;
                if (speed > maxSpeed) speed = maxSpeed;
            }
            else
            {
                if (speed > 0)
                {
                    speed -= drag * Time.Delta;
                    if (speed < 0) speed = 0;
                }
                speed -= acceleration * Time.Delta;
                if (speed < -maxSpeed) speed = -maxSpeed;
            }
            enemy.transform.body.LinearVelocity = new FVector2(speed, enemy.transform.body.LinearVelocity.Y);

            data.Set("speed", speed);
        }

        public void OnHit(float damage, Enemy enemy, EnemyData data) { }

        public void OnDie(Enemy enemy, EnemyData data) { }
    }
}
