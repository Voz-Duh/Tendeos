using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Tendeos.Physical.Content;
using Tendeos.UI;
using Tendeos.UI.GUIElements;
using Tendeos.Utils;
using Tendeos.Utils.Graphics;

namespace Tendeos
{
    public partial class Core
    {
        public const string ApplicationName = "Tendeos";
        public const float Version = 0;

        public static readonly HashSet<char> UnsignedNumbers = new HashSet<char> { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
        public static readonly HashSet<char> Numbers = new HashSet<char> { '-', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
        public static readonly HashSet<char> FloatNumbers = new HashSet<char> { '-', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '.' };

        public static Game Game { get; private set; }
        public static Player Player { get; set; }

        public static Dictionary<string, Sprite> Icons { get; } = new Dictionary<string, Sprite>();
        public static Icon Sprite2Icon(Sprite sprite) => Icon.From((b, r) => b.Rect(sprite, r.Center));
        public static Icon Text2Icon(string text) => Icon.From((b, r) => b.Text(Font, Localization.Translate(text), r.Center));

        public static Button.Style ButtonStyle { get; private set; }
        public static Window.Style WindowStyle { get; private set; }
        public static Window.Style LabelWindowStyle { get; private set; }
        public static InputField.Style InputFieldStyle { get; private set; }
        public static Slider.Style ScrollSliderStyle { get; private set; }
        public static ScrollButtonsStyle ScrollButtonsStyle { get; private set; }
        public static PlayerInventoryContainer.Style PlayerInventoryStyle { get; private set; }
        public static CraftMenu.Style PlayerCraftMenuStyle { get; private set; }
        public static Toggle.Style ToggleStyle { get; private set; }
        public static Sprite WindowFillerStyle { get; private set; }

        public static Font Font { get; private set; }

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

        private static Action<SpriteBatch> extraShootGuiDraw;
        public static event Action<SpriteBatch> ExtraShootGuiDraw
        {
            add => extraShootGuiDraw += value;
            remove => extraShootGuiDraw -= value;
        }
    }
}