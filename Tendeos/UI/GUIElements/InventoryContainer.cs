using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Text;
using Tendeos.Inventory;
using Tendeos.Physical.Content;
using Tendeos.Utils;
using Tendeos.Utils.Graphics;
using Tendeos.Utils.Input;

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

        public override Inventory.Inventory Copy() =>
            new InventoryContainer(parent, style);

        public override void Open(Vec2 offset)
        {
            base.Open(offset);
            parent.Add(
                window = new Window(Vec2.Zero, new FRectangle(offset.X, offset.Y, style.Width * (slotSize + 1) + 3, style.Height * (slotSize + 1) + 3), Core.WindowStyle)
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
                        Icon.From((spriteBatch, rectangle, self) => DrawItemInfoBox(spriteBatch, Items[i], rectangle.Location, self.MouseOn, false)));
                }
        }

        public static void DrawItemInfoBox(SpriteBatch spriteBatch, (IItem item, int count) info, Vec2 position, bool drawInfo, bool drawBack = true)
        {
            if (drawBack) spriteBatch.Rect(itemInfoBack, position, 0, 1, 0, Origin.Zero, Origin.Zero);
            if (info.item == null) return;
            if (info.item.ItemSprite.Texture != null)
                spriteBatch.Rect(info.item.ItemSprite, Vec2.One + position, 0, 1, 0, Origin.Zero, Origin.Zero);
            if (info.count != 0)
                spriteBatch.Text(Core.Font, $"{info.count}", position + (Vec2)itemInfoBack.Rect.Size.ToVector2(), 1, Origin.One, Origin.One);
            if (info.item.Name != null && drawInfo && new FRectangle(position, itemInfoBack.Rect.Size.ToVector2()).Contains(Mouse.GUIPosition))
            {
                Core.ExtraShootGuiDraw += spriteBatch =>
                {
                    FRectangle rect = new FRectangle(Mouse.GUIPosition, new Vec2(46, 0));
                    Font.BoxedTextData data = Core.Font.GetBoxedTextData(Localization.Translate(info.item.Description), rect);
                    float height = Core.LabelWindowStyle.Label[0].Rect.Height;
                    rect.Height = data.height + height + 1;
                    rect.Width += 6;
                    Window.Draw(spriteBatch, rect, Core.LabelWindowStyle);
                    data.rectangle.Y += height;
                    data.rectangle.X += 2;
                    data.Draw(spriteBatch);
                    spriteBatch.Text(Core.Font, Localization.Translate(info.item.Name), new Vec2(rect.X + 2, rect.Y + height / 2 - 1), 1.1f, Origin.Zero);
                };
            }
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
            public int Width { get; }
            public int Height { get; }

            public Style(int width, int height)
            {
                Width = width;
                Height = height;
            }
        }
    }
}
