using Tendeos.World;

namespace Tendeos.Modding
{
    public interface IModTile : ITile
    {
        public IModScript script { get; }
    }
}
