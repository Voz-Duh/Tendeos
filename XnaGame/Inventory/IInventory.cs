namespace XnaGame.Inventory
{
    public interface IInventory
    {
        (IItem item, int count) Selected { get; }

        void Get(int x, int y);
        bool Contains(IItem item, int count);
        void Remove(IItem item, int count);
        int Add(IItem item, int count);
    }
}
