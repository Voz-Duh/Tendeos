using System.Collections.Generic;
using Tendeos.Inventory;
using Tendeos.Physical.Content;
using Tendeos.Utils;
using Tendeos.Utils.Graphics;
using Tendeos.Utils.Input;
using Tendeos.World;

namespace Tendeos.UI.GUIElements
{
    public class InventoryContainer : Inventory.Inventory, ITileInterface
    {
        public const float slotSize = 8;

        public static Sprite itemInfoBack;

        public readonly Style style;
        protected readonly GUIElement parent;
        protected GUIElement window;

        public InventoryContainer(Style style) : base(style.Width * style.Height)
        {
            parent = Core.MainGUI;
            this.style = style;
        }

        public override Inventory.Inventory Copy() =>
            new InventoryContainer(style);

        public override void Open(Vec2 anchor, Vec2 offset, string name)
        {
            base.Open(anchor, offset, name);
            float offsetY = style.WindowStyle.Label?[0].Rect.Height ?? 0;
            parent.Add(
                window = new Window(anchor,
                        new FRectangle(offset.X, offset.Y, style.Width * (slotSize + 1) + 3,
                            style.Height * (slotSize + 1) + 3 + offsetY), style.WindowStyle, Core.Text2Icon(name), true)
                    .Add(Buttons(offsetY))
            );
        }

        private IEnumerator<GUIElement> Buttons(float offsetY)
        {
            int y;
            for (int x = 0; x < style.Width; x++)
            for (y = 0; y < style.Height; y++)
            {
                int i = x + y * style.Width;
                yield return new Button(new Vec2(0, 0), new FRectangle(
                        x * (slotSize + 1) + 2,
                        y * (slotSize + 1) + 2 + offsetY,
                        slotSize, slotSize),
                    () => Get(i), style.ButtonStyle,
                    Icon.From((spriteBatch, rectangle, self) =>
                        DrawItemInfoBox(spriteBatch, Items[i], rectangle.Location, self.MouseOn, false)));
            }
        }

        public static void DrawItemInfoBox(SpriteBatch spriteBatch, (IItem item, int count) info, Vec2 position,
            bool drawInfo, bool drawBack = true)
        {
            if (drawBack) spriteBatch.Rect(itemInfoBack, position, 1, 0, 0, 0);
            if (info.item == null) return;
            if (info.item.ItemSprite.Texture != null)
                spriteBatch.Rect(info.item.ItemSprite, Vec2.One + position, 1, 0, 0, 0);
            if (info.count != 0)
                spriteBatch.Text(Core.Font, $"{info.count}", position + (Vec2) itemInfoBack.Rect.Size.ToVector2(), 1, 0,
                    1, 1);
            if (info.item.Name != null && drawInfo &&
                new FRectangle(position, itemInfoBack.Rect.Size.ToVector2()).Contains(Mouse.GUIPosition))
            {
                Core.ExtraShootGuiDraw += spriteBatch =>
                {
                    FRectangle rect = new FRectangle(Mouse.GUIPosition, new Vec2(46, 0));
                    Font.BoxedTextData data =
                        Core.Font.GetBoxedTextData(Localization.Translate(info.item.Description), rect);
                    float height = Core.LabelWindowStyle.Label[0].Rect.Height;
                    rect.Height = data.height + height + 1;
                    rect.Width += 6;
                    Window.Draw(spriteBatch, rect, Core.LabelWindowStyle);
                    data.rectangle.Y += height;
                    data.rectangle.X += 2;
                    data.Draw(spriteBatch);
                    spriteBatch.Text(Core.Font, Localization.Translate(info.item.Name),
                        new Vec2(rect.X + 2, rect.Y + height / 2 - 1), 1.1f, 0, 0);
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

        public ITileInterface Clone()
        {
            return new InventoryContainer(style);
        }

        public void Use(IMap map, ref TileData data, Player player)
        {
            Open(new Vec2(1, 0), Vec2.Zero, data.Tile.Tag.Translate());
        }

        public void Unuse(IMap map, ref TileData data, Player player)
        {
            Close(player.transform.Position);
        }

        public void Destroy(IMap map, int x, int y)
        {
            foreach (var item in Items)
                if (item.item != null && item.count != 0)
                    new Item((item.item, item.count), map.Cell2World(x, y));
        }

        public class Style
        {
            public Button.Style ButtonStyle { get; }
            public Window.Style WindowStyle { get; }
            public int Width { get; }
            public int Height { get; }

            public Style(Button.Style buttonStyle, Window.Style windowStyle, int width, int height)
            {
                ButtonStyle = buttonStyle;
                WindowStyle = windowStyle;
                Width = width;
                Height = height;
            }
        }
    }
}