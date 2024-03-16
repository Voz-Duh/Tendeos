using XnaGame.Utils;

namespace XnaGame.Physical
{
    public interface ITransform
    {
        Vec2 World2Local(Vec2 position);
        Vec2 Local2World(Vec2 position);
        float World2Local(float degrees);
        float Local2World(float degrees);
    }
}
