using Microsoft.Xna.Framework;
using System;
using XnaGame.Content;
using XnaGame.Utils;
using XnaGame.Utils.Input;

namespace XnaGame
{
    public partial class Game1
    {
        FVector2 Start;

        protected override void Update(GameTime gameTime)
        {
            Keyboard.Update();
            Mouse.Update();
            Time.GameTime = gameTime;

            GUI.BaseUpdate(FVector2.Zero);

            if (Keyboard.IsPressed(Keys.Escape)) Exit();
            
            if (Keyboard.IsPressed(Keys.E))
            {
                player.transform.Position = Mouse.Position;
            }

            if (Mouse.LeftDown)
            {
                try
                {
                    map.TrySetTile(Tiles.test, map.World2Cell(Mouse.Position));
                }
                catch (IndexOutOfRangeException) { }
            }
            map.Update();

            if (Mouse.RightPressed)
                Start = camera.Position + Mouse.GUIPosition;
            if (Mouse.RightDown)
                camera.Position = Start - Mouse.GUIPosition;

            player.Update();

            world.Step(Time.Delta);
            base.Update(gameTime);
        }
    }
}
