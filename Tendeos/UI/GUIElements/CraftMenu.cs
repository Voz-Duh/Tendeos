using System;
using Tendeos.Inventory;
using Tendeos.Physical;
using Tendeos.Physical.Content;
using Tendeos.Utils;
using Tendeos.Utils.Input;
using Tendeos.World;

namespace Tendeos.UI.GUIElements
{
    public class CraftMenu : Window, ITileInterface
    {
        public readonly Style style;
        protected readonly Inventory.Inventory inventory;
        protected readonly ITransform transform;

        public int scroll;
        public Recipe[] recipes;

        public CraftMenu(Vec2 anchor, Vec2 position, Inventory.Inventory inventory, ITransform transform, Style style,
            Recipe[] recipes, GUIElement[] childs = null) : base(anchor,
            new FRectangle(position.X, position.Y,
                style.ButtonSize.X + style.ScrollSliderStyle.Sprites[0].Rect.Height + 5,
                style.ButtonSize.Y * style.MaxButtonCount + (style.WindowStyle.Label?[0].Rect.Height ?? 0) + 4),
            style.WindowStyle, null, true, childs)
        {
            this.inventory = inventory;
            this.transform = transform;
            this.style = style;
            this.recipes = recipes;
        }

        public override void Update(FRectangle rectangle)
        {
            base.Update(rectangle);

            if (MouseOn)
            {
                scroll = Math.Clamp(scroll - Mouse.Scroll, 0, Math.Max(recipes.Length - style.MaxButtonCount, 0));
            }
        }

        public override void OnAdd()
        {
            float offsetY = base.style.Label?[0].Rect.Height ?? 0;
            Add(new IntSlider(Vec2.Zero, Slider.Type.Up2Down, 2,
                offsetY + 1, Rectangle.Height - 2,
                style.ScrollSliderStyle,
                Math.Max(recipes.Length - style.MaxButtonCount, 0),
                () => scroll,
                v => scroll = v));
            for (int i = 0; i < style.MaxButtonCount; i++)
            {
                int __i = i;
                Add(new Button(Vec2.Zero,
                    new FRectangle(style.ScrollSliderStyle.Sprites[0].Rect.Height + 3,
                        offsetY + i * (style.ButtonSize.Y + 1) + 1, style.ButtonSize.X, style.ButtonSize.Y),
                    () =>
                    {
                        int _i = scroll + __i;
                        int j;
                        for (j = 0; j < recipes[_i].from.Length; j++)
                            if (!inventory.Contains(recipes[_i].from[j].item, recipes[_i].from[j].count))
                                return;
                        for (j = 0; j < recipes[_i].from.Length; j++)
                            inventory.Remove(recipes[_i].from[j].item, recipes[_i].from[j].count);
                        int back = inventory.Add(recipes[_i].to.item, recipes[_i].to.count);
                        if (back != 0) new Item((recipes[_i].to.item, back), transform.Local2World(Vec2.Zero));
                    }, style.ButtonStyle,
                    Icon.From((batch, rect, self) =>
                    {
                        int _i = scroll + __i;
                        for (int j = 0; j < recipes[_i].from.Length; j++)
                            InventoryContainer.DrawItemInfoBox(batch, recipes[_i].from[j],
                                new Vec2(rect.X + 13 + 9 * j, rect.Y + 2), self.MouseOn);
                        InventoryContainer.DrawItemInfoBox(batch, recipes[_i].to, new Vec2(rect.X + 2, rect.Y + 2),
                            self.MouseOn);
                    })));
            }
        }

        public override void OnRemove() => Clear();

        public ITileInterface Clone() => new CraftMenu(new Vec2(1, 0), Vec2.Zero, inventory, transform, style, recipes);

        public void Destroy(IMap map, int x, int y)
        {
        }

        public void ToByte(ByteBuffer buffer)
        {
        }

        public void FromByte(ByteBuffer buffer)
        {
        }

        public void Use(IMap map, ref TileData data, Player player) => Core.MainGUI.Add(this);

        public void Unuse(IMap map, ref TileData data, Player player) => Core.MainGUI.Remove(this);

        new public class Style
        {
            public Window.Style WindowStyle { get; }
            public Button.Style ButtonStyle { get; }
            public Slider.Style ScrollSliderStyle { get; }
            public int MaxButtonCount { get; }
            public Vec2 ButtonSize { get; }

            public Style(Window.Style windowStyle, Slider.Style scrollSliderStyle, Button.Style buttonStyle,
                int maxButtonCount, Vec2 buttonSize)
            {
                WindowStyle = windowStyle;
                ButtonStyle = buttonStyle;
                ScrollSliderStyle = scrollSliderStyle;
                MaxButtonCount = maxButtonCount;
                ButtonSize = buttonSize;
            }
        }
    }
}