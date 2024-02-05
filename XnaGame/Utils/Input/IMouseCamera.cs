using Microsoft.Xna.Framework;

namespace XnaGame.Utils.Input
{
    public interface IMouseCamera
    {
        Vec2 Screen2World(Vec2 position);
        Vec2 Screen2GUI(Vec2 position);
    }
}
