using System;
using System.Collections.Generic;
using XnaGame.Physical.Content;
using XnaGame.Utils;
using XnaGame.Utils.Graphics;

namespace XnaGame.UI.GUIElements
{
    public class PlayerInventoryContainer : Inventory.Inventory
    {
        protected readonly GUIElement parent;
        protected readonly Style style;

        public PlayerInventoryContainer(GUIElement GUI, Style style) : base(style.Width * style.Height + style.Addative.Length)
        {
            parent = GUI;
            this.style = style;
        }

        public override void Open(Vec2 anchor, Vec2 offset)
        {
            base.Open(anchor, offset);
            parent.Add(style.Window.Add(OpenButtons()));
        }

        private IEnumerator<GUIElement> OpenButtons()
        {
            int x, y;
            for (x = 0; x < style.Width; x++)
                for (y = 0; y < style.Height; y++)
                {
                    int i = x + y * style.Width;
                    yield return new Button(new Vec2(0, 0), new FRectangle(
                        style.ButtonsOffset.X + x * (style.SlotSize + 1),
                        style.ButtonsOffset.Y + y * (style.SlotSize + 1),
                        style.SlotSize, style.SlotSize),
                        () => Get(i), style.ButtonStyle,
                        (spriteBatch, rectangle) =>
                        {
                            if (Items[i].item == null) return;
                            spriteBatch.Rect(Items[i].item.ItemSprite, rectangle.Center);
                            spriteBatch.Text(Core.font, $"{Items[i].count}", new Vec2(rectangle.Right, rectangle.Top), 1, 0, Origin.One, Origin.One);
                        });
                }
            for (x = 0; x < style.Addative.Length; x++)
            {
                int i = style.Width * style.Height + x,
                    j = x;
                yield return new Button(new Vec2(0, 0), new FRectangle(style.Addative[j].Item1.X, style.Addative[j].Item1.Y, style.SlotSize, style.SlotSize),
                    () => Get(i, style.Addative[j].Item2), style.ButtonStyle,
                    (spriteBatch, rectangle) =>
                    {
                        if (Items[i].item == null) return;
                        spriteBatch.Rect(Items[i].item.ItemSprite, rectangle.Center);
                        spriteBatch.Text(Core.font, $"{Items[i].count}", new Vec2(rectangle.Right, rectangle.Top), 1, 0, Origin.One, Origin.One);
                    });
            }
        }

        public override void Close(Vec2 position)
        {
            base.Close(position);
            parent.Remove(style.Window);
            if (Selected.item != null)
            {
                int count = Add(Selected.item, Selected.count);
                if (count != 0) new Item((Selected.item, count), position);
            }
            Selected = default;
        }

        public class Style
        {
            public GUIElement Window { get; init; }
            public int Width { get; init; }
            public int Height { get; init; }
            public float SlotSize { get; init; }
            public Button.Style ButtonStyle { get; init; }
            public Vec2 ButtonsOffset { get; init; }
            public (Vec2, Type)[] Addative { get; init; }
            
            public Style(GUIElement window, int width, int height, float slotSize, Button.Style buttonStyle, Vec2 buttonsOffset, params (Vec2, Type)[] addative)
            {
                Window = window;
                Width = width;
                Height = height;
                SlotSize = slotSize;
                ButtonStyle = buttonStyle;
                ButtonsOffset = buttonsOffset;
                Addative = addative;
            }
        }
    }
}
