using XnaGame.PEntities;
using XnaGame.PEntities.Content;
using XnaGame.UI;
using XnaGame.UI.GUIElements;
using XnaGame.Utils;
using XnaGame.Utils.Graphics;
using XnaGame.Utils.Input;

namespace XnaGame.Inventory
{
    public class InventoryContainer : IInventory
    {
        public (IItem item, int count)[,] items = new (IItem item, int count)[5, 5];

        const int slotSize = 10;
        GUIElement on, off, window;

        public InventoryContainer(GUIElement GUI, ITransform transform)
        {
            on = new Button(null, new FVector2(0, 0), new FRectangle(0, 0, slotSize, slotSize), () =>
            {
                on.Remove();
                window.Remove();
                GUI.Add(off);
                Core.ExtraGuiDraw -= DrawSelected;
                if (Selected.item != null)
                {
                    int count = Add(Selected.item, Selected.count);
                    if (count != 0) new Item((Selected.item, count), transform.Local2World(FVector2.Zero));
                }
                Selected = default;
            }, Core.buttonStyle, Core.OnInventoryIcon);
            window = new Window(null, new FVector2(0, 0), new FRectangle(slotSize + 1, 0, items.GetLength(0) * (slotSize + 1) + 3, items.GetLength(1) * (slotSize + 1) + 3), Core.windowStyle);

            for (int x = 0; x < items.GetLength(0); x++)
                for (int y = 0; y < items.GetLength(1); y++)
                {
                    int _x = x, _y = y;
                    new Button(window, new FVector2(0, 0), new FRectangle(
                        y * (slotSize + 1) + 2,
                        x * (slotSize + 1) + 2,
                        slotSize, slotSize),
                        () => Get(_x, _y), Core.buttonStyle,
                        (rectangle) =>
                        {
                            if (items[_x, _y].item == null) return;
                            SDraw.Rect(items[_x, _y].item.ItemSprite, rectangle.Center);
                            SDraw.Text(Core.font, $"{items[_x, _y].count}개", rectangle.Center + FVector2.One * -4, 1, 0, Origin.Zero, Origin.Zero);
                        });
                }

            off = new Button(GUI, new FVector2(0, 0), new FRectangle(0, 0, slotSize, slotSize), () =>
            {
                off.Remove();
                GUI.Add(on);
                GUI.Add(window);
                Core.ExtraGuiDraw += DrawSelected;
            }, Core.buttonStyle, Core.OffInventoryIcon);
        }

        public void DrawSelected()
        {
            if (Selected.item == null) return;
            SDraw.Rect(Selected.item.ItemSprite, Mouse.GUIPosition);
            SDraw.Text(Core.font, $"{Selected.count}개", Mouse.GUIPosition + FVector2.One * -4, 1, 0, Origin.Zero, Origin.Zero);
        }

        public (IItem item, int count) Selected { get; private set; }

        public void Get(int x, int y)
        {
            var item = items[x, y];
            if (Selected.item == null && items[x, y].item == null) return;
            if (Selected.item != item.item || item.count == item.item.MaxCount)
            {
                items[x, y] = Selected;
                Selected = item;
            }
            else
            {
                int count = items[x, y].count += Selected.count;
                if (count > item.item.MaxCount)
                {
                    Selected = (Selected.item, count - item.item.MaxCount);
                    items[x, y].count = item.item.MaxCount;
                }
                else Selected = default;
            }
        }

        public int Add(IItem item, int count)
        {
            int _x, _y;
            for (_x = 0; _x < items.GetLength(0); _x++)
                for (_y = 0; _y < items.GetLength(1); _y++)
                    if (items[_x, _y].item == item && item != null)
                    {
                        int i = items[_x, _y].count += count;
                        if (i > item.MaxCount)
                        {
                            count = i - item.MaxCount;
                            items[_x, _y].count = item.MaxCount;
                        }
                        else return 0;
                    }

            for (_x = 0; _x < items.GetLength(0); _x++)
                for (_y = 0; _y < items.GetLength(1); _y++)
                {
                    if (items[_x, _y].item == null)
                    {
                        if (count > item.MaxCount)
                        {
                            items[_x, _y] = (item, item.MaxCount);
                            count -= item.MaxCount;
                        }
                        else
                        {
                            items[_x, _y] = (item, count);
                            return 0;
                        }
                    }
                }
            return count;
        }

        public void Remove(IItem item, int count)
        {
            for (int x = 0; x < items.GetLength(0); x++)
                for (int y = 0; y < items.GetLength(1); y++)
                    if (items[x, y].item == item)
                    {
                        count -= items[x, y].count;
                        if (count < 0)
                            items[x, y].count = -count;
                        else
                            items[x, y] = default;
                    }
        }

        public bool Contains(IItem item, int count)
        {
            int counter = 0;
            for (int x = 0; x < items.GetLength(0); x++)
                for (int y = 0; y < items.GetLength(1); y++)
                    if (items[x, y].item == item)
                        counter += items[x, y].count;
            return counter >= count;
        }
    }
}
