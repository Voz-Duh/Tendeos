using Microsoft.Xna.Framework;
using System;
using System.IO;
using System.Reflection;
using Tendeos.Content;
using Tendeos.Content.Utlis;
using Tendeos.Inventory.Content;
using Tendeos.Scenes;
using Tendeos.UI;
using Tendeos.UI.GUIElements;
using Tendeos.Utils;
using Tendeos.Utils.Graphics;
using Tendeos.Utils.Input;
using Tendeos.Utils.SaveSystem;
using Tendeos.World.Shadows;

namespace Tendeos
{
    public enum GameScene
    {
        Menu,
        Gameplay,
        StructureEditor
    }

    public partial class Core : Game
    {
        private readonly GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private bool loaded;

        private Scene[] scenes;
        private int scene;

        public GameScene Scene
        {
            get => (GameScene) scene;
            set
            {
                scenes[scene].Clear();
                scene = (int) value;
                scenes[scene].Setup();
                MainGUI = scenes[scene].GUI;
            }
        }

        public Scene this[GameScene scene]
        {
            get => scenes[(int) scene];
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
        public Assets Assets;

        public bool Pixelated = true;

        private Shader defaultShader;

        public Core() : base()
        {
            Game = this;
            graphics = new GraphicsDeviceManager(this)
            {
                PreferredDepthStencilFormat = Microsoft.Xna.Framework.Graphics.DepthFormat.Depth24Stencil8
            };
            IsMouseVisible = true;
            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += new EventHandler<EventArgs>(OnResize);

            loaded = false;
        }

        private void DefineDefaultSettings()
        {
            Settings.Default(Settings.Type.String, "lng", "en");
            Settings.Default(Settings.Type.Int, "shdsmth", (int) ShadowMatrix.SmoothPower.Blocky);
            Settings.Default(Settings.Type.Bool, "shdup", false);
        }

        protected override void Initialize()
        {
            DefineDefaultSettings();
            Settings.Load();
            
            Localization.Load(AssetsPath, "lng");
            Window.Title = $"{ApplicationName} v{FormatVersion}: <title_text_{URandom.SInt(0, 10)}>".WithTranslates();
            
            base.Initialize();
        }

        protected override void LoadContent()
        {
            Font = new Font(130, .05f);
            Assets = new Assets(GraphicsDevice, Font, AssetsPath, 4096, 4096);

            defaultShader = Assets.GetShader("default");

            spriteBatch = new SpriteBatch(GraphicsDevice, Assets.atlas);

            Mouse.Camera = camera = new Camera(160, GraphicsDevice.Viewport);

            InputFieldStyle = new InputField.Style(Assets.GetSprite("ui/input_field"),
                Assets.GetSprite("ui/input_field_carriage"), Font, 1, 2, 2, 10);
            ButtonStyle = new Button.Style(Assets.GetSprite("ui/button"));
            SlotButtonStyle = new Button.Style(Assets.GetSprite("ui/slot_button"));
            WindowStyle = new Window.Style(Assets.GetSprite("ui/window"));
            LabelWindowStyle =
                new Window.Style(Assets.GetSprite("ui/window_topless"), Assets.GetSprite("ui/window_label"));
            ScrollSliderStyle = new Slider.Style(Assets.GetSprite("ui/scroll_slider"), 3..3, false,
                Assets.GetSprite("ui/scroll_slider_thumb"));
            ScrollButtonsStyle = new ScrollButtonsStyle(ScrollSliderStyle, ButtonStyle);
            PlayerInventoryStyle = new PlayerInventoryContainer.Style(
                new Image(Vec2.Zero, new Vec2(0, 0), Assets.GetSprite("ui/player_inventory_window")),
                6, 5, 8,
                SlotButtonStyle,
                new Vec2(4),
                (new Vec2(70, 3), typeof(Helmet)),
                (new Vec2(70, 12), typeof(Cuirass)),
                (new Vec2(70, 21), typeof(Legging)));
            PlayerCraftMenuStyle =
                new CraftMenu.Style(LabelWindowStyle, ScrollSliderStyle, ButtonStyle, 4, new Vec2(50, 12));
            ToggleStyle = new Toggle.Style(Assets.GetSprite("ui/toggle"));
            WindowFillerStyle = Assets.GetSprite("ui/filler");
            InventoryContainer.itemInfoBack = Assets.GetSprite("ui/item_info_back");
            Assets.LoadSpriteData("ui/icons", Icons);

            Mods.Init(spriteBatch, Assets);
            Liquids.Init();
            Effects.Init(Assets);
            Entities.Init();
            Items.Init();
            Tiles.Init();
            Structures.Init();
            Biomes.Init();
            ContentAttributes.Compute(Assets, typeof(Liquids));
            ContentAttributes.Compute(Assets, typeof(Effects));
            ContentAttributes.Compute(Assets, typeof(Entities));
            ContentAttributes.Compute(Assets, typeof(Items));
            ContentAttributes.Compute(Assets, typeof(Tiles));
            ContentAttributes.Compute(Assets, typeof(Structures));
            ContentAttributes.Compute(Assets, typeof(Biomes));
            Mods.Start();

            Font.Init();

            scenes = new Scene[]
            {
                new MainMenuScene(this),
                new GameplayScene(this),
                new StructureEditorScene(this, 8),
            };
            scenes[scene].Setup();
            MainGUI = scenes[scene].GUI;

            loaded = true;
        }

        protected void OnResize(object sender, EventArgs e)
        {
            camera.SetViewport(GraphicsDevice.Viewport);
            scenes[scene].OnResize();
        }

        protected override void Draw(GameTime gameTime)
        {
            if (!IsActive) return;

            extraShootGuiDraw = b => { };
            Time.gameTime = gameTime;

            if (!loaded)
            {
            }

            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin(defaultShader, Assets.atlas.texture, 8172, camera.GetViewMatrix());
            scenes[scene].Draw(spriteBatch);
            spriteBatch.End();
            scenes[scene].AfterDraw(spriteBatch);
            spriteBatch.Begin(defaultShader, Assets.atlas.texture, 8172, camera.GetGUIMatrix());
            scenes[scene].GUI.Draw(spriteBatch);
            extraGuiDraw(spriteBatch);
            extraShootGuiDraw(spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }

        protected override void Update(GameTime gameTime)
        {
            if (!IsActive) return;

            Keyboard.Update();
            Mouse.Update();

            Time.gameTime = gameTime;

            if (Keyboard.IsPressed(Keys.Tab))
            {
                MessageBox.Show("Test title", "Test", MessageBox.Type.Info);
            }

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