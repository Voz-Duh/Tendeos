using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Tendeos.Inventory;
using Tendeos.Physical.Content;
using Tendeos.Utils;
using Tendeos.Utils.Graphics;

namespace Tendeos.UI.GUIElements
{
    public class InventoryContainer : Inventory.Inventory
    {
        public const float slotSize = 10;

        public static Sprite itemInfoBack;

        protected readonly Style style;
        protected readonly GUIElement parent;
        protected GUIElement window;

        public InventoryContainer(GUIElement GUI, Style style) : base(style.Width * style.Height)
        {
            parent = GUI;
            this.style = style;
        }

        public override void Open(Vec2 anchor, Vec2 offset)
        {
            base.Open(anchor, offset);
            parent.Add(
                window = new Window(anchor, new FRectangle(offset.X, offset.Y, style.Width * (slotSize + 1) + 3, style.Height * (slotSize + 1) + 3), Core.WindowStyle)
                    .Add(Buttons())
                );
        }

        private IEnumerator<GUIElement> Buttons()
        {
            int y;
            for (int x = 0; x < style.Width; x++)
                for (y = 0; y < style.Height; y++)
                {
                    int i = x + y * style.Width;
                    yield return new Button(new Vec2(0, 0), new FRectangle(
                        x * (slotSize + 1) + 2,
                        y * (slotSize + 1) + 2,
                        slotSize, slotSize),
                        () => Get(i), Core.ButtonStyle,
                        (spriteBatch, rectangle) => DrawItemInfoBox(spriteBatch, Items[i], rectangle.Location, false));
                }
        }

        public static void DrawItemInfoBox(SpriteBatch spriteBatch, (IItem item, int count) info, Vec2 position, bool drawBack = true)
        {
            if (drawBack) spriteBatch.Rect(itemInfoBack, position, 0, 1, 0, Origin.Zero, Origin.Zero);
            if (info.item == null) return;
            spriteBatch.Rect(info.item.ItemSprite, Vec2.One + position, 0, 1, 0, Origin.Zero, Origin.Zero);
            spriteBatch.Text(Core.Font, $"{info.count}", position + (Vec2)itemInfoBack.Rect.Size.ToVector2(), 1, Origin.One, Origin.One);
        }

        public override void Close(Vec2 position)
        {
            base.Close(position);
            parent.Remove(window);
            window = null;
            if (Selected.item != null)
            {
                int count = Add(Selected.item, Selected.count);
                if (count != 0) new Item((Selected.item, count), position);
            }
            Selected = default;
        }

        public override void ToByte(ByteBuffer buffer)
        {
            base.ToByte(buffer);
        }

        public override void FromByte(ByteBuffer buffer)
        {
            base.ToByte(buffer);
        }

        public class Style
        {
            public int Width { get; init; }
            public int Height { get; init; }

            public Style(int width, int height)
            {
                Width = width;
                Height = height;
            }
        }
    }
}
