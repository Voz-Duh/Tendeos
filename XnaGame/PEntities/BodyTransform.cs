using XMatrix = Microsoft.Xna.Framework.Matrix;
using XnaGame.Utils;
using nkast.Aether.Physics2D.Dynamics;
using Microsoft.Xna.Framework;

namespace XnaGame.PEntities;

public class BodyTransform : ITransform
{
    public bool flipX;
    public Vec2 Position
    {
        get => body.position;
        set => body.position = value;
    }


    public readonly Collider body;

    public BodyTransform(Collider body)
    {
        this.body = body;
    }

    public BodyTransform(Collider body, bool flipX)
    {
        this.body = body;
        this.flipX = flipX;
    }

    public float Local2World(float degrees) => degrees;
    public float World2Local(float degrees) => degrees;

    public Vec2 Local2World(Vec2 point) => Position + (flipX ? new Vec2(-point.X, point.Y) : point);
    public Vec2 World2Local(Vec2 point) => Position - (flipX ? new Vec2(-point.X, point.Y) : point);
}
