
using Microsoft.Xna.Framework.Graphics;

namespace XnaGame.Physical.Content
{
    public interface IEnemyComponent
    {
        void Draw(SpriteBatch spriteBatch, Enemy enemy, EnemyData data);
        void CheckState(Enemy enemy, EnemyData data);
        void IdleState(Enemy enemy, EnemyData data);
        void AgressState(Enemy enemy, EnemyData data);
        void OnHit(float damage, Enemy enemy, EnemyData data);
        void OnDie(Enemy enemy, EnemyData data);
    }
}