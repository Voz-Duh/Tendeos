using XnaGame.Inventory;
using XnaGame.State;
using XnaGame.Utils;
using XnaGame.Utils.Graphics;
using XnaGame.World;

namespace XnaGame.PEntities.Content
{
    public class Item : SpawnEntity
    {
        public const float getItemDistance = Map.tileSize * 4;
        public (IItem, int) item;

        public Vec2 velocity;
        public Vec2 position;

        public Item((IItem, int) item, Vec2 position) : base()
        {
            this.item = item;
            this.position = position;
            velocity = new Vec2(URandom.Float(-10, 10), URandom.Float(-55, -30));
        }

        public override void Draw() => SDraw.Rect(item.Item1.ItemSprite, position);

        public override void Update()
        {
            bool collided = false;
            Physics.RaycastMap(
                (point, normal, fraction) =>
                {
                    position = point + normal * 2;
                    velocity.X = 0;
                    if (normal.Y != 0)
                        velocity.Y = 0;
                    collided = true;
                }, position, Vec2.Normalize(velocity) * (velocity.Length() * Time.Delta + 2f));

            if (Vec2.Distance(Core.player.transform.Position, position) < getItemDistance)
            {
                int count = Core.player.inventory.Add(item.Item1, item.Item2);
                if (count != 0)
                    item.Item2 = count;
                else
                    Remove();
            }
            if (!collided) position += velocity * Time.Delta;
            velocity.Y += InGameState.gravity * Time.Delta;
        }
    }
}
