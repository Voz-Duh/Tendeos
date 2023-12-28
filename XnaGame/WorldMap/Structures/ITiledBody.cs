using XnaGame.Utils;

namespace XnaGame.WorldMap.Structures
{
    public interface ITiledBody
    {
        void AddForce(int x, int y, FVector2 force, ForceType type, bool local = true);
    }
}
