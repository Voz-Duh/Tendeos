using Microsoft.Xna.Framework.Graphics;
using XnaGame.Utils;

namespace Prototype.Graphics
{
    public interface IGUICamera
    {
        FVector2 WorldViewport { get; }
    }
}
