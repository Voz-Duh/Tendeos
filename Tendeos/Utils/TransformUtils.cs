using Tendeos.Physical;

namespace Tendeos.Utils;

public static class TransformUtils
{
    public static void StepEffect(bool onFloor, float horizontalMove, BodyTransform transform)
    {
        if (onFloor)
        {
            Vec2 point = Vec2.Zero;
            float fromY = transform.Position.Y + (transform.body.halfSize.Y - 2 - Physics.TileSize / 2);
            float offsetX = (transform.body.halfSize.X + 0.2f) * horizontalMove;
            bool haveCollision = false;
            Physics.RaycastMap((_, _, p, _, _) =>
                {
                    point = p;
                    haveCollision = true;
                },
                new Vec2(transform.Position.X + offsetX, fromY),
                new Vec2(0, Physics.TileSize / 3));
            if (haveCollision)
            {
                haveCollision = false;
                Physics.RaycastMap((_, _, _, _, _) => haveCollision = true,
                    transform.Position + new Vec2(offsetX, 0),
                    new Vec2(0, -transform.body.halfSize.Y - Physics.TileSize / 2));
                if (!haveCollision)
                {
                    haveCollision = false;
                    Physics.RaycastMap((_, _, _, _, _) => haveCollision = true,
                        new Vec2(transform.Position.X, fromY),
                        new Vec2(offsetX, 0));

                    if (!haveCollision)
                    {
                        transform.Position -= new Vec2(0, point.Y - fromY + 3);
                    }
                }
            }
        }
    }
}