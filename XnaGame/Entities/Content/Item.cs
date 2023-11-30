using XnaGame.Inventory;
using XnaGame.State;
using XnaGame.Utils;
using XnaGame.Utils.Graphics;
using XnaGame.WorldMap;

namespace XnaGame.Entities.Content
{
    public class Item : Entity
    {
        public const float getItemDistance = Map.tileSize * 4;
        public (IItem, int) item;
        private FVector2 velocity;
        private FVector2 position;
        private readonly Player player;

        public Item((IItem, int) item, FVector2 position) : base()
        {
            this.item = item;
            this.position = position;
            velocity = new FVector2(Random.Float(-10, 10), Random.Float(-55, -30));
            player = Core.GetEntity<Player>();
        }

        public override void Draw() => SDraw.Rect(item.Item1.ItemSprite, position);

        public override void Update()
        {
            bool collided = false;
            Core.world.RayCast(
                (fixture, point, normal, fraction) =>
                {
                    if (fixture.Body.Tag is IMap)
                    {
                        position = point + normal * 0.1f;
                        velocity.X = 0;
                        if (normal.Y != 0)
                            velocity.Y = 0;
                        collided = true;
                        return 0;
                    }
                    return -1;
                }, position, position + velocity * Time.Delta);

            if (FVector2.Distance(player.transform.Position, position) < getItemDistance)
            {
                int count = player.inventory.Add(item.Item1, item.Item2);
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
