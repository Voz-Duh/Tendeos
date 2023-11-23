using Microsoft.Xna.Framework;

namespace XnaGame.Utils.Input
{
    public interface IMouseCamera
    {
        FVector2 ScreenToWorldSpace(FVector2 position);
        FVector2 ScreenToGUISpace(FVector2 position);
    }
}
