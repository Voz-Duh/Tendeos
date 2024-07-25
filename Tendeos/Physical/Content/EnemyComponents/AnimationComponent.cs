using Microsoft.Xna.Framework;
using Tendeos.Content.Utlis;
using Tendeos.Utils.Graphics;

namespace Tendeos.Physical.Content.EnemyComponents;

public class AnimationComponent : IEnemyComponent
{
    [SpriteLoad("AnimationTexture", true)]
    private Sprite animationSprite;

    protected Sprite[] idleAnimation, moveAnimation, inAirAnimation;

    public int AnimationIdleWidth, AnimationIdleSplitRows, AnimationIdleSplitColumns;
    public int AnimationInAirWidth, AnimationInAirSplitRows, AnimationInAirSplitColumns;
    public int AnimationMoveWidth, AnimationMoveSplitRows, AnimationMoveSplitColumns;
    public float FrameRate;
    public string AnimationTexture;
    public EnemyState State;

    public void OnContentLoaded()
    {
        idleAnimation = new Sprite(animationSprite,
                new Rectangle(0, 0, AnimationIdleWidth, animationSprite.Rect.Height)
                ).Split(AnimationIdleSplitRows, AnimationIdleSplitColumns, 1);
        inAirAnimation = new Sprite(animationSprite,
            new Rectangle(AnimationIdleWidth + 1, 0, AnimationInAirWidth, animationSprite.Rect.Height)
        ).Split(AnimationInAirSplitRows, AnimationInAirSplitColumns, 1);
        moveAnimation = new Sprite(animationSprite,
            new Rectangle(AnimationIdleWidth + AnimationInAirWidth + 2, 0, AnimationMoveWidth, animationSprite.Rect.Height)
        ).Split(AnimationMoveSplitRows, AnimationMoveSplitColumns, 1);
    }
    
    public void Draw(SpriteBatch spriteBatch, Enemy enemy, EnemyData data)
    {
        data.Get(out float animationTimer, "animation");
        if (enemy.LastState != enemy.State)
        {
            animationTimer = 0;
        }
        
        Sprite[] animation = enemy.OnFloor ? enemy.State switch
        {
            EnemyState.Idle => idleAnimation,
            _ => moveAnimation
        } : inAirAnimation;
        
        int frame = animation.Animation(FrameRate, ref animationTimer);
        spriteBatch.Rect(animation[frame], enemy.Transform.Position, flipX: enemy.Transform.flipX);
            
        data.Set("animation", animationTimer);
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