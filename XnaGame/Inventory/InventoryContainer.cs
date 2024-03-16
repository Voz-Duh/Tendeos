using System.Collections.Generic;
using XnaGame.Physical.Content;
using XnaGame.UI;
using XnaGame.UI.GUIElements;
using XnaGame.Utils;
using XnaGame.Utils.Graphics;

namespace XnaGame.Inventory
{
    public class InventoryContainer : Inventory
    {
        public const float slotSize = 10;

        protected int Width { get; set; }
        protected int Height { get; set; }
        protected readonly GUIElement parent;
        protected GUIElement window;

        public InventoryContainer(GUIElement GUI, int width = 10, int height = 10) : base(width * height)
        {
            Width = width;
            Height = height;
            parent = GUI;
        }

        public override void Open(Vec2 anchor, Vec2 offset)
        {
            base.Open(anchor, offset);
            parent.Add(
                window = new Window(anchor, new FRectangle(offset.X, offset.Y, Width * (slotSize + 1) + 3, Height * (slotSize + 1) + 3), Core.windowStyle)
                    .Add(Buttons())
                );
        }

        private IEnumerator<GUIElement> Buttons()
        {
            int y;
            for (int x = 0; x < Width; x++)
                for (y = 0; y < Height; y++)
                {
                    int i = x + y * Width;
                    yield return new Button(new Vec2(0, 0), new FRectangle(
                        x * (slotSize + 1) + 2,
                        y * (slotSize + 1) + 2,
                        slotSize, slotSize),
                        () => Get(i), Core.buttonStyle,
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
            buffer.Append(Width);
            buffer.Append(Height);
            base.ToByte(buffer);
        }

        public override void FromByte(ByteBuffer buffer)
        {
            Width = buffer.ReadInt();
            Height = buffer.ReadInt();
            base.ToByte(buffer);
        }
    }
}
