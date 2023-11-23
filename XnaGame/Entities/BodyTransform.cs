using XMatrix = Microsoft.Xna.Framework.Matrix;
using XnaGame.Utils;
using nkast.Aether.Physics2D.Dynamics;

namespace XnaGame.Entities;

public class BodyTransform : ITransform
{
    public bool flipX;
    public float Rotation
    {
        get => body.Rotation;
        set => body.Rotation = value;
    }
    public FVector2 Position
    {
        get => body.Position;
        set => body.Position = value;
    }

    public readonly Body body;

    public BodyTransform(Body body)
    {
        this.body = body;
    }

    public BodyTransform(Body body, bool flipX)
    {
        this.body = body;
        this.flipX = flipX;
    }

    public float Local2World(float degrees) => degrees + body.Rotation;
    public float World2Local(float degrees) => degrees - body.Rotation;

    public FVector2 Local2World(FVector2 point) => FVector2.Transform(flipX ? new FVector2(-point.X, point.Y) : point, Matrix);
    public FVector2 World2Local(FVector2 point) => FVector2.Transform(flipX ? new FVector2(-point.X, point.Y) : point, XMatrix.Invert(Matrix));

    private XMatrix Matrix => XMatrix.CreateTranslation(body.Position.X, body.Position.Y, 0) * XMatrix.CreateRotationZ(Rotation) * XMatrix.CreateScale(1);
}
