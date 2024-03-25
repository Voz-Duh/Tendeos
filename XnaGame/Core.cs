using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using XnaGame.Content;
using XnaGame.Content.Utlis;
using XnaGame.Scenes;
using XnaGame.UI.GUIElements;
using XnaGame.Utils;
using XnaGame.Utils.Graphics;
using XnaGame.Utils.Input;
using XnaGame.Utils.SaveSystem;

namespace XnaGame
{
    public enum GameScene { Menu, InGame }

    public partial class Core : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        public Scene[] scenes;
        private GameScene scene;
        public GameScene Scene
        {
            get => scene;
            set
            {
                scenes[(int)scene].Clear();
                scene = value;
                scenes[(int)scene].Setup();
            }
        }

        public Camera camera;

        public Core()
        {
            Instance = this;
            graphics = new GraphicsDeviceManager(this)
            {
                PreferredDepthStencilFormat = DepthFormat.Depth24Stencil8
            };
            Content.RootDirectory = "Assets";
            IsMouseVisible = true;
            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += new EventHandler<EventArgs>(OnResize);
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            Settings.Load();
            Localization.Load(Content);

            spriteBatch = new SpriteBatch(GraphicsDevice);

            camera = new Camera(160, GraphicsDevice.Viewport);

            Liquids.Init(Content);
            Effects.Init(Content);
            Entities.Init(Content);
            Items.Init(Content);
            Tiles.Init(Content);
            Structures.Init(Content);
            Biomes.Init(Content);
            ContentAttributes.Compute(Content, typeof(Liquids));
            ContentAttributes.Compute(Content, typeof(Effects));
            ContentAttributes.Compute(Content, typeof(Entities));
            ContentAttributes.Compute(Content, typeof(Items));
            ContentAttributes.Compute(Content, typeof(Tiles));
            ContentAttributes.Compute(Content, typeof(Structures));
            ContentAttributes.Compute(Content, typeof(Biomes));

            Mouse.Camera = camera;
            font = new DynamicSpriteFontScaled(Content, new[] {
                "NotoSans-Regular.ttf",
                "NotoSansKR-Regular.ttf"
            }, 130, .05f);
            buttonStyle = new Button.Style(new Sprite(Content.Load<Texture2D>("ui/button")));
            windowStyle = new Window.Style(new Sprite(Content.Load<Texture2D>("ui/window")));
            playerInventoryStyle = new PlayerInventoryContainer.Style(
                new Image(Vec2.Zero, new Vec2(11, 0), Sprite.Load(Content, "ui/player_inventory_window")),
                6, 5, 8,
                new Button.Style(Sprite.Load(Content, "ui/slot_button")),
                new Vec2(4)/*,
                (new Vec2(69, 3), )*/);
            Content.LoadSpriteData("ui/icons", Icons);

            scenes = new Scene[]
            {
                new MainMenuScene(this),
                new GameplayScene(this),
            };
            scenes[(int)Scene].Setup();
        }

        protected void OnResize(object sender, EventArgs e)
        {
            camera.SetViewport(GraphicsDevice.Viewport);
            scenes[(int)Scene].OnResize();
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null, null, camera.GetViewMatrix());
            scenes[(int)Scene].Draw(spriteBatch);
            spriteBatch.End();
            scenes[(int)Scene].AfterDraw(spriteBatch);
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null, null, camera.GetGUIMatrix());
            scenes[(int)Scene].GUI.Draw(spriteBatch);
            extraGuiDraw(spriteBatch);
            spriteBatch.End();
            base.Draw(gameTime);
        }

        protected override void Update(GameTime gameTime)
        {
            Keyboard.Update();
            Mouse.Update();
            Time.GameTime = gameTime;

            scenes[(int)Scene].GUI.Reset();
            scenes[(int)Scene].GUI.Update();
            scenes[(int)Scene].Update();
        }
    }
}
