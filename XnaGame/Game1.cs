using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using nkast.Aether.Physics2D.Dynamics;
using System;
using XnaGame.Entities.Content;
using XnaGame.Utils;
using XnaGame.WorldMap;

namespace XnaGame
{
    public partial class Game1 : Game
    {
        private GraphicsDeviceManager Graphics;
        private SpriteBatch _spriteBatch;
        private Batch batch;
        private World world;
        private Camera camera;
        private Player player;
        private Map map;

        public Game1()
        {
            Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Assets";
            IsMouseVisible = true;
            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += new EventHandler<EventArgs>(OnResize);
        }

        protected override void Initialize()
        {
            world = new World(new FVector2(0, 90));
            base.Initialize();
        }

        protected void OnResize(object sender, EventArgs e) => camera.SetViewport(GraphicsDevice.Viewport);
    }
}
