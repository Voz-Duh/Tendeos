using XnaGame.Utils;

namespace Prototype.Graphics
{
    public interface IGUICamera
    {
        FVector2 Origin { get; }
        float Zoom { get; }
    }
}
