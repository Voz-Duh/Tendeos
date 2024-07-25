using Tendeos.Utils.Graphics;

namespace Tendeos.Physical.Content
{
    public interface IEnemyComponent
    {
        void Draw(SpriteBatch spriteBatch, Enemy enemy, EnemyData data);
        void CheckState(Enemy enemy, EnemyData data);
        void IdleState(Enemy enemy, EnemyData data);
        void AggressState(Enemy enemy, EnemyData data);
        void OnHit(float damage, Enemy enemy, EnemyData data);
        void OnDie(Enemy enemy, EnemyData data);
    }
}