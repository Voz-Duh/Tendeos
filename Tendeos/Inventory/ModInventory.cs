using Tendeos.Utils;

namespace Tendeos.Inventory
{
    public class ModInventory
    {
        private Inventory inventory;

        public ModInventory(Inventory inventory) => this.inventory = inventory;

        public void open(Vec2 anchor, Vec2 offset, string name) => inventory.Open(anchor, offset, name);

        public void close(Vec2 position) => inventory.Close(position);

        public void get(int index) => inventory.Get(index);

        public int add(IItem item, int count) => inventory.Add(item, count);

        public void remove(IItem item, int count) => inventory.Remove(item, count);

        public bool has(IItem item, int count) => inventory.Contains(item, count);
    }
}