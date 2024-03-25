using Microsoft.Xna.Framework.Graphics;
using System;
using System.Runtime.InteropServices;
using XnaGame.UI.GUIElements;
using XnaGame.Utils;
using XnaGame.Utils.Graphics;

namespace XnaGame.Scenes
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
                Core.buttonStyle,
                (batch, rect) => batch.Text(Core.font, "Play \"Test\"", rect.Center)
                ))
                .Add(new Slider(Vec2.Zero, 20, 3..20, new Slider.Style(Sprite.Load(Game.Content, "ui/slider"), 1..1, Slider.Type.Down2Up),
                () => a,
                v => a = v
                ))
                .Add(new Slider(Vec2.Zero, 28, 3..20, new Slider.Style(Sprite.Load(Game.Content, "ui/slider"), 1..1, Slider.Type.Up2Down),
                () => b,
                v => b = v
                ))
                .Add(new Slider(Vec2.Zero, 20, 3..20, new Slider.Style(Sprite.Load(Game.Content, "ui/slider"), 1..1, Slider.Type.Right2Left),
                () => c,
                v => c = v
                ))
                .Add(new Slider(Vec2.Zero, 28, 3..20, new Slider.Style(Sprite.Load(Game.Content, "ui/slider"), 1..1, Slider.Type.Left2Right),
                () => d,
                v => d = v
                ))
                .Add(new IntSlider(Vec2.Zero, 34, 3..20, new Slider.Style(Sprite.Load(Game.Content, "ui/slider"), 1..1, Slider.Type.Down2Up), 5,
                () => e,
                v => e = v
                ))
                .Add(new IntSlider(Vec2.Zero, 40, 3..20, new Slider.Style(Sprite.Load(Game.Content, "ui/slider"), 1..1, Slider.Type.Up2Down), 12,
                () => f,
                v => f = v
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
