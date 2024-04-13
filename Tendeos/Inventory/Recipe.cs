namespace Tendeos.Inventory
{
    public readonly struct Recipe
    {
        public readonly (IItem item, int count) to;
        public readonly (IItem item, int count)[] from;

        public Recipe((IItem item, int count) to, params (IItem item, int count)[] from)
        {
            this.to = to;
            this.from = from;
        }
    }
}