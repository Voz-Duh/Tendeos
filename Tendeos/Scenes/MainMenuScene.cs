using Microsoft.Xna.Framework.Graphics;
using System;
using System.Runtime.InteropServices;
using Tendeos.UI.GUIElements;
using Tendeos.Utils;
using Tendeos.Utils.Graphics;

namespace Tendeos.Scenes
{
    public class MainMenuScene : Scene
    {
        public MainMenuScene(Core game) : base(game)
        {
        }

        public override void Init()
        {
        }

        public override void InitGUI()
        {
            float a = 0, b = 0, c = 0, d = 0;
            int e = 0, f = 0;
            GUI.Add(new Button(new Vec2(0.5f), new FRectangle(0, 0, 20, 20),
                () => Game.Scene = GameScene.InGame,
                Core.ButtonStyle,
                (batch, rect) => batch.Text(Core.Font, "Play \"Test\"", rect.Center)
                ));
        }

        public override void Clear()
        {
            GUI.Clear();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
        }

        public override void Update()
        {
        }
    }
}
