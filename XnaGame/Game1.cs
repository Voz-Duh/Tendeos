using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using nkast.Aether.Physics2D.Dynamics;
using System;
using System.Numerics;
using XnaGame.Entities.Content;
using XnaGame.Utils;
using XnaGame.Utils.Input;
using XnaGame.WorldMap;
using XnaGame.WorldMap.Content;
using static XnaGame.WorldMap.Map;

namespace XnaGame
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Batch batch;
        private World world;
        private Camera camera;
        private Map map;
        private Player player;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            world = new World(new FVector2(0, 90));

            base.Initialize();
        }

        protected override void LoadContent()
        {
            SDraw.spriteBatch = _spriteBatch = new SpriteBatch(GraphicsDevice);
            batch = new Batch(GraphicsDevice);

            camera = new Camera(GraphicsDevice.Viewport)
            {
                Zoom = 4,
            };
            Mouse.Camera = camera;
            map = new Map(world, 8, 8);
            test = new AutoTile(2, new Sprite(Content.Load<Texture2D>("test")));
            player = new Player(world, camera,
                new Sprite(Content.Load<Texture2D>("player_m_head")),
                new Sprite(Content.Load<Texture2D>("player_m_arm_l")),
                new Sprite(Content.Load<Texture2D>("player_m_arm_r")),
                new Sprite(Content.Load<Texture2D>("player_m_body")),
                new Sprite(Content.Load<Texture2D>("player_m_legs")));
            // TODO: use this.Content to load your game content here
        }
        FVector2 Start;
        ITile test;
        protected override void Update(GameTime gameTime)
        {
            Keyboard.Update();
            Mouse.Update();
            Time.GameTime = gameTime;
            if (Keyboard.IsPressed(Keys.Escape)) Exit();
            if (Keyboard.IsPressed(Keys.E))
            {
                player.transform.Position = Mouse.Position;
            }

            // TODO: Add your update logic here
            if (Mouse.LeftDown)
            {
                try
                {
                    map.TrySetTile(test, map.World2Cell(Mouse.Position));
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

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            SDraw.Matrix = camera.GetViewMatrix();
            SDraw.Apply();
            map.Draw();
            player.Draw();
            SDraw.End();

            base.Draw(gameTime);
        }
    }
}