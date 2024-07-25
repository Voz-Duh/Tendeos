using Tendeos.Utils;
using Tendeos.Utils.SaveSystem;

namespace Tendeos.World
{
    public interface ITileInterface : IUseable
    {
        ITileInterface Clone();

        void Destroy(IMap map, int x, int y);

        [ToByte]
        void ToByte(ByteBuffer buffer);

        [FromByte]
        void FromByte(ByteBuffer buffer);
    }
}