
using nkast.Aether.Physics2D.Dynamics;
using XnaGame.Inventory;
using XnaGame.State;
using XnaGame.Utils;
using XnaGame.Utils.Graphics;
using XnaGame.WorldMap;

namespace XnaGame.PEntities.Content
{
    public class Item : SpawnEntity
    {
        public const float getItemDistance = Map.tileSize * 4;
        public (IItem, int) item;

        public FVector2 velocity;
        public FVector2 position;
        public Body parent;

        public Item((IItem, int) item, FVector2 position) : base()
        {
            this.item = item;
            this.position = position;
            velocity = new FVector2(URandom.Float(-10, 10), URandom.Float(-55, -30));
        }

        public FVector2 Position
        {
            get => parent?.GetWorldPoint(position) ?? position;
            set => position = parent?.GetLocalPoint(value) ?? value;
        }

        public override void Draw() => SDraw.Rect(item.Item1.ItemSprite, Position);

        public override void Update()
        {
            bool collided = false;
            Core.world.RayCast(
                (fixture, point, normal, fraction) =>
                {
                    if (fixture.Body.Tag is IMap)
                    {
                        Position = point + normal * 2;
                        velocity.X = 0;
                        if (normal.Y != 0)
                            velocity.Y = 0;
                        collided = true;
                        if (parent != fixture.Body)
                        {
                            FVector2 p = Position;
                            parent = fixture.Body;
                            position = parent.GetLocalPoint(p);
                        }
                        return 0;
                    }
                    return -1;
                }, Position, Position + FVector2.Normalize(velocity) * (velocity.Length() * Time.Delta + 2f));
            if (!collided)
            {
                if (parent != null)
                    velocity += (FVector2)parent.LinearVelocity;
                position = Position;
                parent = null;
            }

            if (FVector2.Distance(Core.player.transform.Position, Position) < getItemDistance)
            {
                int count = Core.player.inventory.Add(item.Item1, item.Item2);
                if (count != 0)
                    item.Item2 = count;
                else
                    Remove();
            }
            if (!collided) Position += velocity * Time.Delta;
            velocity.Y += InGameState.gravity * Time.Delta;
        }
    }
}
