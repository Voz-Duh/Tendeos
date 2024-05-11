using System;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tendeos.Content;
using Tendeos.Inventory;
using Tendeos.Physical.Content;
using Tendeos.UI;
using Tendeos.UI.GUIElements;
using Tendeos.Utils;
using Tendeos.Utils.Graphics;
using Tendeos.Utils.Input;
using Tendeos.Utils.SaveSystem;
using Tendeos.Utils.SaveSystem.Content;
using Tendeos.World;
using Tendeos.World.Content;
using Tendeos.World.EntitySpawn;
using Tendeos.World.Generation;
using Tendeos.World.Liquid;
using Tendeos.World.Shadows;

namespace Tendeos.Scenes
{
    public class GameplayScene : Scene
    {
        public class SaveInstance
        {
            [ToByte] public float playTime;
            [ToByte] public Player player;
            [ToByte] public IMap map;
            [ToByte] public WaterWorld waterWorld;

            [ToByte] public void ToByte(ByteBuffer buffer) => EntityManager.ToByte(buffer);
            [FromByte] public void FromByte(ByteBuffer buffer) => EntityManager.FromByte(buffer);
        }

        public static string SaveName;
        public static uint GameSeed;

        private WaterWorld waterWorld;
        private WorldGenerator generator;
        private EntitySpawner entitySpawner;
        private Player player;
        private Map map;
        private ShadowMatrix shadowMatrix;
        private PlayerInventoryContainer playerInventory;
        private SaveInstance saveInstance;
        private ThreadLoop physicsLoop;
        
        private GUIElement menuPlane;
        
        private int mapWidth = 30, mapHeight = 40, chunkSize = 8;

        public GameplayScene(Core game) : base(game)
        {
            Save.SetInstance(saveInstance = new SaveInstance());
            physicsLoop = new ThreadLoop(Physics.Process);
        }
        uint a;

        public override async void Init()
        {
            generator = new WorldGenerator(
                Noise.CPerlin(0.3f, 3) - Noise.CSimplex(0.2f, 4),
                (Noise.CSimplex(0.1f) + Noise.CPerlin(0.17f, 0.3f)) / 2 + Noise.CSimplex(0.03f, 1) + Noise.CSimplex(0.1f, 2)/2,
                Noise.CPerlin(0.9f, 8) + Noise.CPerlin(0.8f, 3),
                60, new[]
                {
                    Biomes.test,
                    Biomes.hills,
                }, GameSeed);

            await Task.Run(() =>
            {
                waterWorld = new WaterWorld(Game.GraphicsDevice, Game.Content, mapWidth, mapHeight, Game.camera, chunkSize);
                waterWorld.Map = Physics.Map = map = new Map(mapWidth, mapHeight, waterWorld, Game.camera, chunkSize);
                map.CameraViewSet();
                map.Ignore = Tiles.ignore;

                Physics.Meter = (Physics.TileSize = map.TileSize) * 2;

                shadowMatrix = new ShadowMatrix(Game.GraphicsDevice, map, waterWorld, Game.camera)
                {
                    Smooth = (ShadowMatrix.SmoothPower)Settings.GetInt("shadow_smoothing"),
                    DirectionLight = Color.White.ToVector3(),
                    DirectionLightFrom = 6,
                    DirectionLightTo = 44,
                    DirectionLightRange = 10,
                    DirectionLightShadowRange = 40,
                    DirectionLightIntensity = 1
                };

                entitySpawner = new EntitySpawner(map);

                a = shadowMatrix.Create(Color.White, 0, 0, 1, 8);

                playerInventory = new PlayerInventoryContainer(GUI, Core.PlayerInventoryStyle);

                Core.Player = player = new Player(playerInventory, Game.camera, map, Game.Content,
                    new PlayerInfo()
                    {
                        body = 1,
                        sex = true
                    });

                Item.GetItemDistance = map.TileSize * 2;
            });
            (saveInstance.map, saveInstance.waterWorld, saveInstance.player) =
            (map, waterWorld, player);

            if (!await Save.LoadAsync(SaveName))
            {
                await generator.Generate(map);

                Save.Create(SaveName);
                Save.Unload();
            }
            else generator.Loaded();
            shadowMatrix.Start();
            entitySpawner.Start();
            physicsLoop.Start();

            Mods.AddToScripts("playerInventory", new ModInventory(playerInventory));
            Mods.InvokeInMains("init");
        }

        public override async void InitGUI()
        {
            Recipe[] recipes = new Recipe[]
            {
                new Recipe((Items.pickaxe, 1), (Tiles.test, 2), (Tiles.stone, 5)),
                new Recipe((Items.bow, 1), (Tiles.test, 6), (Tiles.dirt, 3)),
                new Recipe((Tiles.test, 2), (Tiles.dirt, 1)),
                new Recipe((Tiles.test, 4), (Tiles.stone, 1)),
                new Recipe((Tiles.dirt, 1), (Tiles.test, 2)),
                new Recipe((Tiles.stone, 1), (Tiles.test, 4)),
                new Recipe((Tiles.tree, 1), (Tiles.test, 20), (Tiles.stone, 12)),
            };
            while (player == null) await Task.Yield();
            GUIElement settingsPlane = null;
            settingsPlane = new WindowFiller(Game.camera, Core.WindowFillerStyle)
                .Add(new Window(new Vec2(0.5f), new FRectangle(0, 0, 70, 35), Core.LabelWindowStyle, Core.Text2Icon("settings"))
                    .Add(new Button(new Vec2(0.5f, 1), new FRectangle(0, -2, 66, 10),
                    () =>
                    {
                        menuPlane.Remove(settingsPlane);
                        Settings.SaveAsync();
                    }, Core.ButtonStyle, Core.Text2Icon("back")))
                    .Add(new EnumSwitcher<ShadowMatrix.SmoothPower>(new Vec2(0.5f, 1), new FRectangle(0, -13, 66, 10), Core.Font, Core.ButtonStyle,
                    () => (ShadowMatrix.SmoothPower)Settings.GetInt("shadow_smoothing"),
                    v =>
                    {
                        Settings.Set(Settings.Type.Int, "shadow_smoothing", (int)v);
                        shadowMatrix.Smooth = v;
                    }))
                    .Add(new Toggle(Vec2.Zero, Vec2.Zero, Core.ToggleStyle, value => shadowMatrix.IsUpscaled = value))
                );
            menuPlane = new WindowFiller(Game.camera, Core.WindowFillerStyle)
                .Add(new Window(new Vec2(0.5f), new FRectangle(0, 0, 70, 35), Core.LabelWindowStyle, Core.Text2Icon("pause"))
                    .Add(new Button(new Vec2(0.5f, 1), new FRectangle(0, -13, 66, 10),
                    () =>
                    {
                        menuPlane.Add(settingsPlane);
                    }, Core.ButtonStyle, Core.Text2Icon("settings")))
                    .Add(new Button(new Vec2(0.5f, 1), new FRectangle(0, -2, 66, 10),
                    () =>
                    {
                        GUI.Remove(menuPlane);
                        Game.Paused = false;
                    }, Core.ButtonStyle, Core.Text2Icon("back")))
                );
            CraftMenu craftMenu = new CraftMenu(Vec2.Zero, new Vec2(0, 11 + playerInventory.style.Window.rectangle.Height), playerInventory, player.transform, Core.PlayerCraftMenuStyle, recipes);
            GUI.Add(new SwitchButton(Vec2.Zero, new FRectangle(0, 0, 11, 11), Core.ButtonStyle,
                () => playerInventory.Close(player.transform.Position),
                Core.Icons["on_inventory_icon"],
                () => playerInventory.Open(new Vec2(0, 11)),
                Core.Icons["off_inventory_icon"]
                ))
                .Add(new SwitchButton(Vec2.Zero, new FRectangle(11, 0, 11, 11), Core.ButtonStyle,
                () => GUI.Remove(craftMenu),
                Core.Icons["craft_menu_icon"],
                () => GUI.Add(craftMenu),
                Core.Icons["craft_menu_icon"]
                ))
                .Add(new Button(Vec2.Zero, new FRectangle(22, 0, 11, 11),
                () =>
                {
                    GUI.Add(menuPlane);
                    Game.Paused = true;
                }, Core.ButtonStyle,
                Core.Icons["pause_menu_icon"]
                ));
            Core.ExtraGuiDraw += DrawSelected;
            Core.ExtraGuiUpdate += UpdateSelected;
            Mods.InvokeInMains("guiInit");
        }

        public override void Clear()
        {
            Mods.InvokeInMains("clear");
            EntityManager.Clear();
            EffectManager.Clear();
            waterWorld = null;
            generator = null;
            player = null;
            map = null;
            menuPlane = null;
            shadowMatrix = null;
            entitySpawner = null;
            playerInventory = null;
            Core.ExtraGuiDraw -= DrawSelected;
            Core.ExtraGuiUpdate -= UpdateSelected;
        }

        public void DrawSelected(SpriteBatch spriteBatch)
        {
            if (Inventory.Inventory.Selected.item == null) return;
            spriteBatch.Rect(Inventory.Inventory.Selected.item.ItemSprite, Mouse.GUIPosition);
            spriteBatch.Text(Core.Font, $"{Inventory.Inventory.Selected.count}", Mouse.GUIPosition + Vec2.One * -4, 1, Origin.Zero, Origin.Zero);
        }

        public void UpdateSelected()
        {
            if (Mouse.LeftReleased && !Mouse.OnGUI && Inventory.Inventory.Selected.item != null)
            {
                new Item(Inventory.Inventory.Selected, player.transform.Position, true);
                Inventory.Inventory.Selected = default;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!generator.Done)
            {
                spriteBatch.Text(Core.Font, generator.Message, Vec2.Zero);
                Game.camera.Position = Vec2.Zero;
                return;
            }

            map?.Draw(spriteBatch);
            EntityManager.Draw(spriteBatch);
            EffectManager.Draw(spriteBatch);
        }
        public override void AfterDraw(SpriteBatch spriteBatch)
        {
            if (!generator.Done) return;

            waterWorld.Draw();
            shadowMatrix.Draw();
        }

        int sad = 0;
        public override void Update()
        {
            if (!generator.Done) return;

            //waterWorld.Update();

            saveInstance.playTime += Time.Delta;

            float t = saveInstance.playTime * MathHelper.Pi * 0.001f + MathHelper.Pi / 2f;
            float s = MathF.Sin(t);
            shadowMatrix.DirectionLightAngle = t * 2;
            shadowMatrix.DirectionLightIntensity = (s - .75f) / .25f;
            
            if (Keyboard.IsPressed(Keys.Escape))
            {
                GUI.Add(menuPlane);
                Game.Paused = true;
            }
            if (Keyboard.IsPressed(Keys.M))
            {
                EntityManager.Clear();
                EntityManager.Add(player);
                Save.Load("Test");
            }
            if (Keyboard.IsPressed(Keys.RightShift))
            {
                shadowMatrix.Create(sad == 0 ? Color.Red : (sad == 1 ? Color.Green : Color.Blue), Mouse.Position, 1, 30);
                sad++;
                if (sad == 3) sad = 0;
            }
            if (Keyboard.IsPressed(Keys.E))
            {
                player.transform.Position = Mouse.Position;
            }
            if (Keyboard.IsPressed(Keys.F))
            {
                map.SetTileData<AutoTile>(true, map.World2Cell(Mouse.Position), data => AutoTile.Collisions[data.GetU6(0)] switch
                {
                    0 => data.SetU6(0, 7),
                    1 => data.SetU6(0, 11+7),
                    2 => data.SetU6(0, 0),
                    _ => data,
                });
            }

            if (Keyboard.IsPressed(Keys.O))
                new Item((Items.bow, 1), Mouse.Position);
            if (Keyboard.IsPressed(Keys.P))
                new Item((Items.pickaxe, 1), Mouse.Position);
            if (Keyboard.IsPressed(Keys.T))
            {
                var (x, y) = map.World2Cell(Mouse.Position);
                Structures.test.Spawn(map, x, y);
            }

            if (Keyboard.IsDown(Keys.K))
            {
                map.Flow(null, 1, map.World2Cell(Mouse.Position));
            }

            if (Keyboard.IsPressed(Keys.L))
            {
                Entities.dummy.Spawn(Mouse.Position);
            }

            EntityManager.Update();
            EffectManager.Update();

            shadowMatrix.SetPosition(a, player.transform.Position);
        }

        public override void OnResize()
        {
            waterWorld.CameraViewSet();
            shadowMatrix.Resize();
            map.CameraViewSet();
        }
    }
}
