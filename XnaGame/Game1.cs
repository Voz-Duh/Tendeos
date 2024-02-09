using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using XnaGame.Content;
using XnaGame.State;
using XnaGame.UI.GUIElements;
using XnaGame.Utils;
using XnaGame.Utils.Graphics;
using XnaGame.Utils.Input;

namespace XnaGame
{
    public enum EGameState { InGame }

    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        public Camera camera;
        public GameState[] states;
        public EGameState State { private get; set; }

        public Game1()
        {
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
            SDraw.spriteBatch = new SpriteBatch(GraphicsDevice);

            Core.camera = camera = new Camera(160, GraphicsDevice.Viewport);

            Liquids.Init(Content);
            Effects.Init(Content);
            Entities.Init(Content);
            Items.Init(Content);
            Tiles.Init(Content);
            Structures.Init(Content);

            Mouse.Camera = camera;

            Core.font = new DynamicSpriteFontScaled(Content.LoadFileBytes("NotoSansKR-Regular.ttf"), 130, .05f);
            //SpriteChar.LoadFont(Vars.font, GraphicsDevice, Vars.fontLibrary, FontType.NoAlpha, "Assets\\NotoSansKR-Regular.ttf", 10);
            
            Core.buttonStyle = new Button.Style(new Sprite(Content.Load<Texture2D>("ui/button")));
            Core.windowStyle = new Window.Style(new Sprite(Content.Load<Texture2D>("ui/window")));

            Core.icons = new Sprite(Content.Load<Texture2D>("ui/icons")).Split(2, 1, 1);

            states = new[]{
                new InGameState(this)
            };
        }

        protected void OnResize(object sender, EventArgs e)
        {
            camera.SetViewport(GraphicsDevice.Viewport);
            states[(int)State].OnResize();
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            SDraw.Matrix = camera.GetViewMatrix();
            SDraw.Apply();
            states[(int)State].Draw();
            SDraw.End();
            states[(int)State].AfterDraw();
            SDraw.Matrix = camera.GetGUIMatrix();
            SDraw.Apply();
            states[(int)State].GUI.Draw();
            Core.extraGuiDraw();
            SDraw.End();
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
