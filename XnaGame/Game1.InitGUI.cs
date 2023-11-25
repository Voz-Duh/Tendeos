using Microsoft.Xna.Framework.Graphics;
using XnaGame.UI;
using XnaGame.UI.GUIElements;
using XnaGame.Utils;

namespace XnaGame
{
    public partial class Game1
    {
        GUIElement GUI;

        protected void InitGUI()
        {
            Button.Style buttonBaseStyle = new Button.Style(new Sprite(Content.Load<Texture2D>("button")));

            GUI = new GUIElement(camera);

            new Button(GUI, new FRectangle(1, 1, 6, 6), () => Exit(), buttonBaseStyle, new Sprite(Content.Load<Texture2D>("icon_exit")));
        }
    }
}
