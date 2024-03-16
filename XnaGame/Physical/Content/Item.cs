using Microsoft.Xna.Framework.Graphics;
using XnaGame.Inventory;
using XnaGame.Utils;
using XnaGame.Utils.Graphics;
using XnaGame.World;

namespace XnaGame.Physical.Content
{
    public class Item : SpawnEntity
    {
        public static float GetItemDistance { get; set; }

        private readonly Player target;
        public (IItem, int) item;

        public Vec2 velocity;
        public Vec2 position;

        public Item((IItem, int) item, Vec2 position)
        {
            target = EntityManager.GetEntity<Player>();
            this.item = item;
            this.position = position;
            velocity = new Vec2(URandom.SFloat(-10, 10), URandom.SFloat(-55, -30));
        }

        public override void Draw(SpriteBatch spriteBatch) => spriteBatch.Rect(item.Item1.ItemSprite, position);

        public override void Update()
        {
            bool collided = false;
            Physics.RaycastMap(
                (_, _, point, normal, _) =>
                {
                    position = point + normal * 2;
                    velocity.X = 0;
                    if (normal.Y != 0)
                        velocity.Y = 0;
                    collided = true;
                }, position, Vec2.Normalize(velocity) * (velocity.Length() * Time.Delta + 2f));

            if (Vec2.Distance(target.transform.Position, position) < GetItemDistance)
            {
                int count = target.inventory.Add(item.Item1, item.Item2);
                if (count != 0)
                    item.Item2 = count;
                else
                    Remove();
            }
            if (!collided) position += velocity * Time.Delta;
            velocity += Physics.Gravity * Time.Delta * Physics.Meter;
        }
    }
}
