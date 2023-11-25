using Microsoft.Xna.Framework;
using XnaGame.Utils;

namespace XnaGame
{
    public partial class Game1
    {
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            SDraw.Matrix = camera.GetViewMatrix();
            SDraw.Apply();

            map.Draw();
            player.Draw();

            SDraw.Matrix = camera.GetGUIMatrix();
            SDraw.Apply();

            GUI.BaseDraw(FVector2.Zero);
            SDraw.End();

            base.Draw(gameTime);
        }
    }
}
