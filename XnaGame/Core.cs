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
    public enum EGameState { InGame, Menu }

    public partial class Core : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        public Scene[] states;
        private EGameState state;
        public EGameState State
        {
            get => state;
            set
            {
                states[(int)state].Clear();
                state = value;
                states[(int)state].Setup();
            }
        }

        public Camera camera;

        public Core()
        {
            Instance = this;
            graphics = new GraphicsDeviceManager(this);
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
            Content.LoadSpriteData("ui/icons", Icons);

            states = new[]
            {
                new GameplayScene(this)
            };
            states[(int)State].Setup();
        }

        protected void OnResize(object sender, EventArgs e)
        {
            camera.SetViewport(GraphicsDevice.Viewport);
            states[(int)State].OnResize();
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null, null, camera.GetViewMatrix());
            states[(int)State].Draw(spriteBatch);
            spriteBatch.End();
            states[(int)State].AfterDraw(spriteBatch);
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null, null, camera.GetGUIMatrix());
            states[(int)State].GUI.Draw(spriteBatch);
            extraGuiDraw(spriteBatch);
            spriteBatch.End();
            base.Draw(gameTime);
        }

        protected override void Update(GameTime gameTime)
        {
            Keyboard.Update();
            Mouse.Update();
            Time.GameTime = gameTime;

            states[(int)State].GUI.Reset();
            states[(int)State].GUI.Update();
            states[(int)State].Update();
        }
    }
}
