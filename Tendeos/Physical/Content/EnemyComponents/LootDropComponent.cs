using Microsoft.Xna.Framework.Graphics;
using System;
using Tendeos.Content.Utlis;
using Tendeos.Inventory;
using Tendeos.Utils;

namespace Tendeos.Physical.Content.EnemyComponents
{
    public class LootDropComponent : IEnemyComponent
    {
        private readonly float chance;
        private readonly Range range;
        public string ItemLoad { get; }
        [ContentLoad("ItemLoad", true)]
        public IItem item;

        public LootDropComponent(float chance, Range range, string item)
        {
            this.chance = chance;
            this.range = range;
            ItemLoad = item;
        }

        public void Draw(SpriteBatch spriteBatch, Enemy enemy, EnemyData data) { }

        public void CheckState(Enemy enemy, EnemyData data) { }

        public void IdleState(Enemy enemy, EnemyData data) { }

        public void AgressState(Enemy enemy, EnemyData data) { }

        public void OnDie(Enemy enemy, EnemyData data)
        {
            if (URandom.SFloat(100) <= chance) new Item((item, URandom.SInt(range.Start.Value, range.End.Value)), enemy.transform.Position);
        }

        public void OnHit(float damage, Enemy enemy, EnemyData data) { }
    }
}
