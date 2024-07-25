using Tendeos.Content.Utlis;
using Tendeos.Utils;
using Tendeos.Utils.Graphics;

namespace Tendeos.Physical.Content.EnemyComponents
{
    public class JumpComponent : IEnemyComponent
    {
        private readonly float jumpPower;
        private readonly bool jumpEverytime;

        public JumpComponent(float jumpPower, bool jumpEverytime = false)
        {
            this.jumpPower = jumpPower;
            this.jumpEverytime = jumpEverytime;
        }

        public void Draw(SpriteBatch spriteBatch, Enemy enemy, EnemyData data)
        {
        }

        public void CheckState(Enemy enemy, EnemyData data)
        {
            Jump(enemy, data);
        }

        public void IdleState(Enemy enemy, EnemyData data)
        {
        }

        public void AggressState(Enemy enemy, EnemyData data)
        {
            Jump(enemy, data);
        }

        public void Jump(Enemy enemy, EnemyData data)
        {
            if (enemy.OnFloor && (!jumpEverytime || enemy.Transform.Position.Y - enemy.Transform.body.halfSize.Y > enemy.LastPlayerPosition.Y))
            {
                enemy.Transform.body.velocity.Y = -jumpPower;
            }
        }

        public void OnHit(float damage, Enemy enemy, EnemyData data)
        {
        }

        public void OnDie(Enemy enemy, EnemyData data)
        {
        }
    }
}