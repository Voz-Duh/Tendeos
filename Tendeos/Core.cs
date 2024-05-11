using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Tendeos.Content;
using Tendeos.Content.Utlis;
using Tendeos.Scenes;
using Tendeos.UI;
using Tendeos.UI.GUIElements;
using Tendeos.Utils;
using Tendeos.Utils.Graphics;
using Tendeos.Utils.Input;
using Tendeos.Utils.SaveSystem;

namespace Tendeos
{
    public enum GameScene { Menu, Gameplay, StructureEditor }

    public partial class Core : Game
    {
        private readonly GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private bool loaded;

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

        public Scene this[GameScene scene]
        {
            get => scenes[(int)scene];
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

            loaded = false;
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
            Font = new Font(Content, new[] {
                "NotoSans-Regular.ttf",
                "NotoSansKR-Regular.ttf"
            }, 130, .05f);
            InputFieldStyle = new InputField.Style(Sprite.Load(Content, "ui/input_field"), Sprite.Load(Content, "ui/input_field_carriage"), Font, 1, 2, 2, 10);
            ButtonStyle = new Button.Style(Sprite.Load(Content, "ui/button"));
            WindowStyle = new Window.Style(Sprite.Load(Content, "ui/window"));
            LabelWindowStyle = new Window.Style(Sprite.Load(Content, "ui/window_topless"), Sprite.Load(Content, "ui/window_label"));
            ScrollSliderStyle = new Slider.Style(Sprite.Load(Content, "ui/scroll_slider"), 3..3, false, Sprite.Load(Content, "ui/scroll_slider_thumb"));
            ScrollButtonsStyle = new ScrollButtonsStyle(ScrollSliderStyle, ButtonStyle);
            PlayerInventoryStyle = new PlayerInventoryContainer.Style(
                new Image(Vec2.Zero, new Vec2(0, 0), Sprite.Load(Content, "ui/player_inventory_window")),
                6, 5, 8,
                new Button.Style(Sprite.Load(Content, "ui/slot_button")),
                new Vec2(4)/*,
                (new Vec2(69, 3), )*/);
            PlayerCraftMenuStyle = new CraftMenu.Style(LabelWindowStyle, ScrollSliderStyle, ButtonStyle, 4, new Vec2(50, 12));
            ToggleStyle = new Toggle.Style(Sprite.Load(Content, "ui/toggle"));
            WindowFillerStyle = Sprite.Load(Game.Content, "ui/filler");
            InventoryContainer.itemInfoBack = Sprite.Load(Content, "ui/item_info_back");
            Content.LoadSpriteData("ui/icons", Icons);

            scenes = new Scene[]
            {
                new MainMenuScene(this),
                new GameplayScene(this),
                new StructureEditorScene(this, 8),
            };
            scenes[scene].Setup();

            loaded = true;
        }

        protected void OnResize(object sender, EventArgs e)
        {
            camera.SetViewport(GraphicsDevice.Viewport);
            scenes[scene].OnResize();
        }

        protected override void Draw(GameTime gameTime)
        {
            extraShootGuiDraw = b => { };
            Time.gameTime = gameTime;

            if (!loaded)
            {

            }
            
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null, null, camera.GetViewMatrix());
            scenes[scene].Draw(spriteBatch);
            spriteBatch.End();
            scenes[scene].AfterDraw(spriteBatch);
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null, null, camera.GetGUIMatrix());
            scenes[scene].GUI.Draw(spriteBatch);
            extraGuiDraw(spriteBatch);
            extraShootGuiDraw(spriteBatch);
            spriteBatch.End();
            base.Draw(gameTime);
        }

        protected override void Update(GameTime gameTime)
        {
            Keyboard.Update();
            Mouse.Update();

            Time.gameTime = gameTime;

            scenes[scene].GUI.Reset();
            scenes[scene].GUI.Update();
            GUIElement.Deselect();
            if (!paused)
            {
                scenes[scene].Update();
            }
            extraGuiUpdate();
        }
    }
}
