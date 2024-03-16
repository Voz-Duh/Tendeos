using Microsoft.Xna.Framework.Graphics;
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
            GUI.Add(
                new Button(new Vec2(0.5f), new FRectangle(0, 0, 20, 20), () =>
                {
                    Game.State = EGameState.Menu;
                }, Core.buttonStyle, (batch, rect) =>
                {
                    batch.Text(Core.font, "Play \"Test\"", rect.Center, 1, 0);
                })
                );
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
