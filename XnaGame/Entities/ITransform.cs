using XnaGame.Utils;

namespace XnaGame.Entities
{
    public interface ITransform
    {
        FVector2 World2Local(FVector2 position);
        FVector2 Local2World(FVector2 position);
        float World2Local(float degrees);
        float Local2World(float degrees);
    }
}
