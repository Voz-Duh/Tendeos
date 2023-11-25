using Microsoft.Xna.Framework.Graphics;
using XnaGame.Entities.Content;
using XnaGame.Utils;
using XnaGame.Utils.Input;
using XnaGame.WorldMap;
using XnaGame.Content;

namespace XnaGame
{
    public partial class Game1
    {
        protected override void LoadContent()
        {
            SDraw.spriteBatch = _spriteBatch = new SpriteBatch(GraphicsDevice);
            batch = new Batch(GraphicsDevice);
            
            camera = new Camera(GraphicsDevice.Viewport)
            {
                Zoom = 4,
            };

            Tiles.Init(Content);

            InitGUI();

            Mouse.Camera = camera;
            map = new Map(world, 16, 8, (x, y) => y < 15 ? null : Tiles.test);
            player = new Player(world, camera,
                new Sprite(Content.Load<Texture2D>("player_m_head")),
                new Sprite(Content.Load<Texture2D>("player_m_arm_l")),
                new Sprite(Content.Load<Texture2D>("player_m_arm_r")),
                new Sprite(Content.Load<Texture2D>("player_m_body")),
                new Sprite(Content.Load<Texture2D>("player_m_legs")));
        }
    }
}
