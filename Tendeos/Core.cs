using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Tendeos.Content;
using Tendeos.Content.Utlis;
using Tendeos.Scenes;
using Tendeos.UI.GUIElements;
using Tendeos.Utils;
using Tendeos.Utils.Graphics;
using Tendeos.Utils.Input;
using Tendeos.Utils.SaveSystem;

namespace Tendeos
{
    public enum GameScene { Menu, InGame }

    public partial class Core : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        private Scene[] scenes;
        private int scene;
        public GameScene Scene
        {
            get => (GameScene)scene;
            set
            {
                scenes[scene].Clear();
                scene = (int)value;
                scenes[scene].Setup();
            }
        }

        private bool paused;
        public bool Paused
        {
            get => paused;
            set
            {
                paused = value;
                ThreadLoop.SetPause(paused);
            }
        }

        public Camera camera;

        public Core()
        {
            Game = this;
            graphics = new GraphicsDeviceManager(this)
            {
                PreferredDepthStencilFormat = DepthFormat.Depth24Stencil8
            };
            Content.RootDirectory = "Assets";
            IsMouseVisible = true;
            Window.Title = $"{ApplicationName}: v{Version}";
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

            Mods.Init(spriteBatch, Content);
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
            Mods.Start(GraphicsDevice);

            Mouse.Camera = camera;
            Font = new DynamicSpriteFontScaled(Content, new[] {
                "NotoSans-Regular.ttf",
                "NotoSansKR-Regular.ttf"
            }, 130, .05f);
            ButtonStyle = new Button.Style(new Sprite(Content.Load<Texture2D>("ui/button")));
            WindowStyle = new Window.Style(new Sprite(Content.Load<Texture2D>("ui/window")));
            LabelWindowStyle = new Window.Style(new Sprite(Content.Load<Texture2D>("ui/window_topless")), new Sprite(Content.Load<Texture2D>("ui/window_label")));
            ScrollSliderstyle = new Slider.Style(Sprite.Load(Content, "ui/scroll_slider"), 3..3, false, Sprite.Load(Content, "ui/scroll_slider_thumb"));
            PlayerInventoryStyle = new PlayerInventoryContainer.Style(
                new Image(Vec2.Zero, new Vec2(11, 0), Sprite.Load(Content, "ui/player_inventory_window")),
                6, 5, 8,
                new Button.Style(Sprite.Load(Content, "ui/slot_button")),
                new Vec2(4)/*,
                (new Vec2(69, 3), )*/);
            PlayerCraftMenuStyle = new CraftMenu.Style(LabelWindowStyle, ScrollSliderstyle, ButtonStyle, 4, new Vec2(50, 12));
            InventoryContainer.itemInfoBack = Sprite.Load(Content, "ui/item_info_back");
            Content.LoadSpriteData("ui/icons", Icons);

            scenes = new Scene[]
            {
                new MainMenuScene(this),
                new GameplayScene(this),
            };
            scenes[scene].Setup();
        }

        protected void OnResize(object sender, EventArgs e)
        {
            camera.SetViewport(GraphicsDevice.Viewport);
            scenes[scene].OnResize();
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null, null, camera.GetViewMatrix());
            scenes[scene].Draw(spriteBatch);
            spriteBatch.End();
            scenes[scene].AfterDraw(spriteBatch);
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null, null, camera.GetGUIMatrix());
            scenes[scene].GUI.Draw(spriteBatch);
            extraGuiDraw(spriteBatch);
            spriteBatch.End();
            base.Draw(gameTime);
        }

        protected override void Update(GameTime gameTime)
        {
            Keyboard.Update();
            Mouse.Update();

            if (Keyboard.IsPressed(Keys.Tab)) Paused = !Paused;

            if (paused) return;
            Time.GameTime = gameTime;

            scenes[scene].GUI.Reset();
            scenes[scene].GUI.Update();
            scenes[scene].Update();
            extraGuiUpdate();
        }
    }
}
