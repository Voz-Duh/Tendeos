using System;
using System.Collections.Generic;
using XnaGame.Physical.Content;
using XnaGame.UI;
using XnaGame.UI.GUIElements;
using XnaGame.Utils;
using XnaGame.Utils.Graphics;

namespace XnaGame.Inventory
{
    public class PlayerInventoryContainer : Inventory
    {
        protected int Width { get; }
        protected int Height { get; }
        protected (Vec2, Type)[] Addative { get; }
        protected readonly GUIElement parent;
        protected readonly Vec2 buttonsOffset;
        protected readonly GUIElement window;
        protected readonly Button.Style buttonStyle;
        protected readonly float slotSize;

        public PlayerInventoryContainer(GUIElement GUI, GUIElement window, Button.Style buttonStyle, float slotSize, Vec2 buttonsOffset, int width, int height, params (Vec2, Type)[] addative) : base(width * height + addative.Length)
        {
            Width = width;
            Height = height;
            Addative = addative;

            parent = GUI;
            this.window = window;
            this.buttonStyle = buttonStyle;
            this.slotSize = slotSize;
            this.buttonsOffset = buttonsOffset;
        }

        public override void Open(Vec2 anchor, Vec2 offset)
        {
            base.Open(anchor, offset);
            parent.Add(window.Add(OpenButtons()));
        }

        private IEnumerator<GUIElement> OpenButtons()
        {
            int x, y;
            for (x = 0; x < Width; x++)
                for (y = 0; y < Height; y++)
                {
                    int i = x + y * Width;
                    yield return new Button(new Vec2(0, 0), new FRectangle(
                        buttonsOffset.X + x * (slotSize + 1),
                        buttonsOffset.Y + y * (slotSize + 1),
                        slotSize, slotSize),
                        () => Get(i), buttonStyle,
                        (spriteBatch, rectangle) =>
                        {
                            if (Items[i].item == null) return;
                            spriteBatch.Rect(Items[i].item.ItemSprite, rectangle.Center);
                            spriteBatch.Text(Core.font, $"{Items[i].count}", new Vec2(rectangle.Right, rectangle.Top), 1, 0, Origin.One, Origin.One);
                        });
                }
            for (x = 0; x < Addative.Length; x++)
            {
                int i = Width * Height + x,
                    j = x;
                yield return new Button(new Vec2(0, 0), new FRectangle(Addative[j].Item1.X, Addative[j].Item1.Y, slotSize, slotSize),
                    () => Get(i, Addative[j].Item2), buttonStyle,
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
            parent.Remove(window);
            if (Selected.item != null)
            {
                int count = Add(Selected.item, Selected.count);
                if (count != 0) new Item((Selected.item, count), position);
            }
            Selected = default;
        }
    }
}
