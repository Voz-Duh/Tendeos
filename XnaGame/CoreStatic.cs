using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using XnaGame.UI.GUIElements;
using XnaGame.Utils;
using XnaGame.Utils.Graphics;

namespace XnaGame
{
    public partial class Core
    {
        public const string ApplicationName = "Tendeos";

        public static Game Instance { get; private set; }

        public static Dictionary<string, Sprite> Icons { get; } = new Dictionary<string, Sprite>();
        public static Action<SpriteBatch, FRectangle> Sprite2Icon(Sprite sprite) => (b, r) => b.Rect(sprite, r.Center);

        public static Button.Style buttonStyle;
        public static Window.Style windowStyle;
        public static PlayerInventoryContainer.Style playerInventoryStyle;

        public static DynamicSpriteFontScaled font;

        public static Action<SpriteBatch> extraGuiDraw = b => { };
        public static event Action<SpriteBatch> ExtraGuiDraw
        {
            add => extraGuiDraw += value;
            remove => extraGuiDraw -= value;
        }
    }
}