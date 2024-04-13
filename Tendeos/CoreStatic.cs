using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Tendeos.Physical.Content;
using Tendeos.UI.GUIElements;
using Tendeos.Utils;
using Tendeos.Utils.Graphics;

namespace Tendeos
{
    public partial class Core
    {
        public const string ApplicationName = "Tendeos";
        public const float Version = 0;

        public static Game Game { get; private set; }
        public static Player Player { get; set; }

        public static Dictionary<string, Sprite> Icons { get; } = new Dictionary<string, Sprite>();
        public static Action<SpriteBatch, FRectangle> Sprite2Icon(Sprite sprite) => (b, r) => b.Rect(sprite, r.Center);

        public static Button.Style ButtonStyle { get; private set; }
        public static Window.Style WindowStyle { get; private set; }
        public static Window.Style LabelWindowStyle { get; private set; }
        public static Slider.Style ScrollSliderstyle { get; private set; }
        public static PlayerInventoryContainer.Style PlayerInventoryStyle { get; private set; }
        public static CraftMenu.Style PlayerCraftMenuStyle { get; private set; }

        public static DynamicSpriteFontScaled Font { get; private set; }

        private static Action<SpriteBatch> extraGuiDraw = b => { };
        public static event Action<SpriteBatch> ExtraGuiDraw
        {
            add => extraGuiDraw += value;
            remove => extraGuiDraw -= value;
        }

        private static Action extraGuiUpdate = () => { };
        public static event Action ExtraGuiUpdate
        {
            add => extraGuiUpdate += value;
            remove => extraGuiUpdate -= value;
        }
    }
}