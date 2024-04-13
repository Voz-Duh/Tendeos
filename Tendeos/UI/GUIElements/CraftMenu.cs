using System;
using Tendeos.Inventory;
using Tendeos.Physical;
using Tendeos.Physical.Content;
using Tendeos.Utils;

namespace Tendeos.UI.GUIElements
{
    public class CraftMenu : Window
    {
        new protected readonly Style style;
        protected readonly Inventory.Inventory inventory;
        protected readonly ITransform transform;

        public int scroll;
        public Recipe[] recipes;

        public CraftMenu(Vec2 anchor, Vec2 position, Inventory.Inventory inventory, ITransform transform, Style style, Recipe[] recipes) : base(anchor,
            new FRectangle(position.X, position.Y, style.ButtonSize.X + style.ScrollSliderStyle.Sprites[0].Height + 5, style.ButtonSize.Y * style.MaxButtonCount + (style.WindowStyle.Label?[0].Rect.Height ?? 0) + 4), style.WindowStyle, true)
        {
            this.inventory = inventory;
            this.transform = transform;
            this.style = style;
            this.recipes = recipes;
        }

        public override void OnAdd()
        {
            float offsetY = base.style.Label?[0].Rect.Height ?? 0;
            Add(new IntSlider(Vec2.Zero, Slider.Type.Up2Down, 2, offsetY + 1, rectangle.Height - 2, style.ScrollSliderStyle, Math.Max(recipes.Length - style.MaxButtonCount, 0),
                () => scroll,
                v => scroll = v
                ));
            for (int i = 0; i < style.MaxButtonCount; i++)
            {
                int __i = i;
                Add(new Button(Vec2.Zero, new FRectangle(style.ScrollSliderStyle.Sprites[0].Rect.Height + 3, offsetY + i * (style.ButtonSize.Y + 1) + 1, style.ButtonSize.X, style.ButtonSize.Y),
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
                    (batch, rect) =>
                    {
                        int _i = scroll + __i;
                        for (int j = 0; j < recipes[_i].from.Length; j++)
                            InventoryContainer.DrawItemInfoBox(batch, recipes[_i].from[j], new Vec2(rect.X + 13 + 9 * j, rect.Y + 2));
                            //batch.Rect(recipes[_i].from[j].item.ItemSprite, new Vec2(rect.X+13+9*j, rect.Center.Y), 0, 1, 0, Origin.Zero);
                        InventoryContainer.DrawItemInfoBox(batch, recipes[_i].to, new Vec2(rect.X + 2, rect.Y + 2));
                        //batch.Rect(recipes[_i].to.item.ItemSprite, new Vec2(rect.X+2, rect.Center.Y), 0, 1, 0, Origin.Zero);
                    }));
            }
        }

        public override void OnRemove() => Clear();

        new public class Style
        {
            public Window.Style WindowStyle { get; init; }
            public Button.Style ButtonStyle { get; init; }
            public Slider.Style ScrollSliderStyle { get; init; }
            public int MaxButtonCount { get; init; }
            public Vec2 ButtonSize { get; init; }

            public Style(Window.Style windowStyle, Slider.Style scrollSliderStyle, Button.Style buttonStyle, int maxButtonCount, Vec2 buttonSize)
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
