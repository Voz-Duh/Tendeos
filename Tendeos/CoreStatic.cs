using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Tendeos.Physical.Content;
using Tendeos.UI;
using Tendeos.UI.GUIElements;
using Tendeos.Utils;
using Tendeos.Utils.Graphics;

namespace Tendeos
{
    public partial class Core
    {
        private const uint @ByteBitmask = byte.MaxValue;
        private const uint @ShortBitmask = ushort.MaxValue;

        public const string ApplicationName = "Tendeos";
        public const uint Version = 0x0000_06_02;
        
        public const ushort VersionRelease = (ushort)(Version >> 16 & @ShortBitmask);
        public const byte VersionAlfa = (byte)(Version >> 8 & @ByteBitmask);
        public const byte VersionBeta = (byte)(Version & @ByteBitmask);

        public static string FormatVersion => $"{VersionRelease}.{VersionAlfa}.{VersionBeta}";

        public const float ViewRadius = 8 * 30;

        public static string AssetsPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets");

        public static readonly HashSet<char> UnsignedNumbers = new HashSet<char>
            {'0', '1', '2', '3', '4', '5', '6', '7', '8', '9'};

        public static readonly HashSet<char> Numbers = new HashSet<char>
            {'-', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9'};

        public static readonly HashSet<char> FloatNumbers = new HashSet<char>
            {'-', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '.'};

        public static readonly HashSet<char> HEXNumbers = new HashSet<char>
            {'0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F'};

        public static Core Game { get; private set; }
        public static Player Player { get; set; }

        public static Dictionary<string, Sprite> Icons { get; } = new Dictionary<string, Sprite>();
        public static Icon Sprite2Icon(Sprite sprite) => Icon.From((b, r) => b.Rect(sprite, r.Center));

        public static Icon Text2Icon(string text) =>
            Icon.From((b, r) => b.Text(Font, Localization.Translate(text), r.Center));

        public static Button.Style ButtonStyle { get; private set; }
        public static Button.Style SlotButtonStyle { get; private set; }
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
        public static GUIElement MainGUI { get; private set; }

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