using Tendeos.Utils.Graphics;

namespace Tendeos.Physical.Content.EnemyComponents;

public class JumpMoveComponent : IEnemyComponent
{
    public float MaxSpeed { get; set; }
    
    public void Draw(SpriteBatch spriteBatch, Enemy enemy, EnemyData data)
    {
    }

    public void CheckState(Enemy enemy, EnemyData data)
    {
    }

    public void IdleState(Enemy enemy, EnemyData data)
    {
    }

    public void AggressState(Enemy enemy, EnemyData data)
    {
    }

    public void OnHit(float damage, Enemy enemy, EnemyData data)
    {
    }

    public void OnDie(Enemy enemy, EnemyData data)
    {
    }
}