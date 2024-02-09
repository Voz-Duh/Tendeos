using System;
using XnaGame.Content;
using XnaGame.Utils;

namespace XnaGame.PEntities.Content.EnemyComponents
{
    public class LootDropComponent : IEnemyComponent
    {
        private readonly float chance;
        private readonly Range range;
        private readonly ItemRef item;

        public LootDropComponent(float chance, Range range, ItemRef item)
        {
            this.chance = chance;
            this.range = range;
            this.item = item;
        }

        public void Draw(Enemy enemy, EnemyData data) { }

        public void CheckState(Enemy enemy, EnemyData data) { }

        public void IdleState(Enemy enemy, EnemyData data) { }

        public void AgressState(Enemy enemy, EnemyData data) { }

        public void OnDie(Enemy enemy, EnemyData data)
        {
            if (URandom.Float(100) <= chance) new Item((item(), URandom.Int(range.Start.Value, range.End.Value)), enemy.transform.Position);
        }

        public void OnHit(float damage, Enemy enemy, EnemyData data) { }
    }
}
