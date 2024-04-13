using Tendeos.Inventory;

namespace Tendeos.Modding
{
    public interface IModItem : IItem
    {
        public IModScript script { get; }
    }
}
