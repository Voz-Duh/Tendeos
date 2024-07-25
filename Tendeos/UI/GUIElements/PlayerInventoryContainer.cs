using System;
using System.Collections.Generic;
using Tendeos.Physical.Content;
using Tendeos.Utils;

namespace Tendeos.UI.GUIElements
{
    public class PlayerInventoryContainer : Inventory.Inventory
    {
        protected readonly GUIElement parent;
        public readonly Style style;

        public PlayerInventoryContainer(GUIElement GUI, Style style) : base(style.Width * style.Height +
                                                                            style.Addative.Length)
        {
            parent = GUI;
            this.style = style;
        }

        public override Inventory.Inventory Copy() =>
            new PlayerInventoryContainer(parent, style);

        public override void Open(Vec2 anchor, Vec2 offset, string _)
        {
            base.Open(anchor, offset, _);
            style.Window.Anchor = anchor;
            style.Window.Rectangle.Location = offset;
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
                    Icon.From((spriteBatch, rectangle) =>
                    {
                        if (Items[i].item == null) return;
                        spriteBatch.Rect(Items[i].item.ItemSprite, rectangle.Center);
                        spriteBatch.Text(Core.Font, $"{Items[i].count}", new Vec2(rectangle.Right, rectangle.Bottom), 1, 0,
                            1, 1);
                    }));
            }

            for (x = 0; x < style.Addative.Length; x++)
            {
                int i = style.Width * style.Height + x,
                    j = x;
                yield return new Button(new Vec2(0, 0),
                    new FRectangle(style.Addative[j].Item1.X, style.Addative[j].Item1.Y, style.SlotSize,
                        style.SlotSize),
                    () => Get(i, style.Addative[j].Item2), style.ButtonStyle,
                    Icon.From((spriteBatch, rectangle) =>
                    {
                        if (Items[i].item == null) return;
                        spriteBatch.Rect(Items[i].item.ItemSprite, rectangle.Center);
                        spriteBatch.Text(Core.Font, $"{Items[i].count}", new Vec2(rectangle.Right, rectangle.Bottom), 1, 0,
                            1, 1);
                    }));
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
            public GUIElement Window { get; }
            public int Width { get; }
            public int Height { get; }
            public float SlotSize { get; }
            public Button.Style ButtonStyle { get; }
            public Vec2 ButtonsOffset { get; }
            public (Vec2, Type)[] Addative { get; }

            public Style(GUIElement window, int width, int height, float slotSize, Button.Style buttonStyle,
                Vec2 buttonsOffset, params (Vec2, Type)[] addative)
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