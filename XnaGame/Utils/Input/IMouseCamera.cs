using Microsoft.Xna.Framework;

namespace XnaGame.Utils.Input
{
    public interface IMouseCamera
    {
        FVector2 Screen2World(FVector2 position);
        FVector2 Screen2GUI(FVector2 position);
    }
}
