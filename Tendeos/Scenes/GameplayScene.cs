using System;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Tendeos.Content;
using Tendeos.Inventory;
using Tendeos.Physical.Content;
using Tendeos.UI;
using Tendeos.UI.GUIElements;
using Tendeos.Utils;
using Tendeos.Utils.Graphics;
using Tendeos.Utils.Input;
using Tendeos.Utils.SaveSystem;
using Tendeos.World;
using Tendeos.World.EntitySpawn;
using Tendeos.World.Generation;
using Tendeos.World.Liquid;
using Tendeos.World.Shadows;
using Console = Tendeos.UI.GUIElements.DeveloperUI.Console;

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

            [ToByte]
            public void ToByte(ByteBuffer buffer) => EntityManager.ToByte(buffer);

            [FromByte]
            public void FromByte(ByteBuffer buffer) => EntityManager.FromByte(buffer);
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
        private Console console;
        private bool consoleOpened;

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
                (Noise.CSimplex(0.4f) + Noise.CPerlin(0.17f, 0.3f)) / 2 + Noise.CSimplex(0.03f, 1) +
                Noise.CSimplex(0.1f, 2) / 2,
                Noise.CPerlin(0.9f, 8) + Noise.CPerlin(0.8f, 3),
                60, new[]
                {
                    Biomes.test,
                    Biomes.hills,
                }, GameSeed);
            
            await Task.Run(() =>
            {
                waterWorld = new WaterWorld(Game.GraphicsDevice, mapWidth, mapHeight, Game.camera, chunkSize);
                waterWorld.Map = Physics.Map = map = new Map(mapWidth, mapHeight, waterWorld, Game.camera, chunkSize);
                map.CameraViewSet();
                map.Ignore = Tiles.ignore;

                Physics.Meter = (Physics.TileSize = map.TileSize) * 2;

                shadowMatrix = new ShadowMatrix(Game.GraphicsDevice, map, waterWorld, Game.camera)
                {
                    Smooth = (ShadowMatrix.SmoothPower) Settings.GetInt("shdsmth"),
                    IsUpscaled = Settings.GetBool("shdup"),
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

                Core.Player = player = new Player(playerInventory, map, Game.Assets,
                    new PlayerInfo()
                    {
                        BodyType = 1,
                        Sex = true
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
            console = new Console(
                camera: Game.camera,

                inputField: new InputField(
                    style: Core.InputFieldStyle,
                    anchor: Vec2.Zero,
                    position: Vec2.Zero,
                    length: 100
                ),
                
                style: Core.WindowFillerStyle,

                closeOnClick: false,
                childs: null,

                Console.CreateCommand(
                    "give",
                    delegate (string item, int value)
                    {
                        // TODO: Multiplayer, give to command executer.
                        new Item((Items.Get(item), value), Core.Player.Position);
                    }
                ));
            
            Recipe[] recipes = new Recipe[]
            {
                new((Items.pickaxe, 1), (Tiles.test, 2), (Tiles.stone, 5)),
                new((Items.bow, 1), (Tiles.test, 6), (Tiles.dirt, 3)),
                new((Tiles.test, 2), (Tiles.dirt, 1)),
                new((Tiles.test, 4), (Tiles.stone, 1)),
                new((Tiles.dirt, 1), (Tiles.test, 2)),
                new((Tiles.stone, 1), (Tiles.test, 4)),
                new((Tiles.tree, 1), (Tiles.test, 20), (Tiles.stone, 12)),
            };
            while (player == null) await Task.Yield();
            GUIElement settingsPlane = null;
            settingsPlane = new WindowFiller(
                style: Core.WindowFillerStyle,
                camera: Game.camera,
                
                childs: new GUIElement[]
                {
                    new Window(
                        style: Core.LabelWindowStyle,
                        addativeDraw: Core.Text2Icon("settings"),
                    
                        anchor: new Vec2(0.5f),
                        rectangle: new FRectangle(0, 0, 70, 100),
                        childs: new GUIElement[]
                        {
                            new Button(
                                icon: Core.Text2Icon("back"),
                                style: Core.ButtonStyle,
                                
                                anchor: new Vec2(0.5f, 1),
                                rectangle: new FRectangle(0, -2, 66, 10),
                                
                                action: () =>
                                {
                                    menuPlane.Remove(settingsPlane);
                                    Settings.SaveAsync();
                                }
                            ),
                            new EnumSwitcher<ShadowMatrix.SmoothPower>(
                                font: Core.Font,
                                style: Core.ButtonStyle,
                                
                                anchor: Vec2.UnitY,
                                rectangle: new FRectangle(0, -13, 66, 10),
                                
                                get: () => (ShadowMatrix.SmoothPower) Settings.GetInt("shdsmth"),
                                set: v => Settings.Set(Settings.Type.Int, "shdsmth", (int)(shadowMatrix.Smooth = v))
                            ),
                            new Toggle(
                                style: Core.ToggleStyle,
                                
                                anchor: Vec2.UnitY,
                                position: new(0, -24),
                                
                                changed: value => Settings.Set(Settings.Type.Bool, "shdup", shadowMatrix.IsUpscaled = value),
                                startValue: Settings.GetBool("shdup")
                            )
                        })
                });
            menuPlane = new WindowFiller(
                style: Core.WindowFillerStyle,
                camera: Game.camera,
                
                childs: new GUIElement[]
                {
                    new Window(
                        style: Core.LabelWindowStyle,
                        addativeDraw: Core.Text2Icon("pause"),
                        
                        anchor: new Vec2(0.5f),
                        rectangle: new FRectangle(0, 0, 70, 35),
                        
                        childs: new GUIElement[]
                        {
                            new Button(
                                icon: Core.Text2Icon("settings"),
                                style: Core.ButtonStyle,
                                
                                anchor: new Vec2(0.5f, 1),
                                rectangle: new FRectangle(0, -13, 66, 10),
                                
                                action: () => menuPlane.Add(settingsPlane)
                            ),
                            new Button(
                                icon: Core.Text2Icon("back"),
                                style: Core.ButtonStyle,
                                
                                anchor: new Vec2(0.5f, 1),
                                rectangle: new FRectangle(0, -2, 66, 10),
                                
                                action: () =>
                                {
                                    GUI.Remove(menuPlane);
                                    Game.Paused = false;
                                }
                            )
                        })
                });
            CraftMenu craftMenu = new CraftMenu(
                    style: Core.PlayerCraftMenuStyle,
                    
                    anchor: Vec2.Zero,
                    position: new Vec2(0, 11 + playerInventory.style.Window.Rectangle.Height),
                    
                    inventory: playerInventory,
                    recipes: recipes,
                    
                    transform: player.transform
                );
            GUI.Add(
                new SwitchButton(
                    style: Core.ButtonStyle,
                    
                    anchor: Vec2.Zero,
                    rectangle: new FRectangle(0, 0, 11, 11),
                    
                    actionOff: () => playerInventory.Close(player.transform.Position),
                    iconOff: Core.Icons["on_inventory_icon"],
                    
                    actionOn: () => playerInventory.Open(Vec2.Zero, new Vec2(0, 11), ""),
                    iconOn: Core.Icons["off_inventory_icon"]
                ),
                new SwitchButton(
                    style: Core.ButtonStyle,
                    
                    anchor: Vec2.Zero,
                    rectangle: new FRectangle(11, 0, 11, 11), 
                    
                    actionOff: () => GUI.Remove(craftMenu),
                    iconOff: Core.Icons["craft_menu_icon"],
                    actionOn: () => GUI.Add(craftMenu),
                    iconOn: Core.Icons["craft_menu_icon"]
                ),
                new Button(
                    icon: Core.Icons["pause_menu_icon"],
                    style: Core.ButtonStyle,
                    
                    anchor: Vec2.Zero,
                    rectangle: new FRectangle(22, 0, 11, 11),
                    action: () =>
                    {
                        GUI.Add(menuPlane);
                        Game.Paused = true;
                    }
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
            spriteBatch.Text(Core.Font, $"{Inventory.Inventory.Selected.count}", Mouse.GUIPosition + Vec2.One * -4, 1,
                0, 0, 0);
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

            waterWorld.Update();

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

            if (Keyboard.IsPressed(Keys.F1))
            {
                Save.Unload();
            }

            if (Keyboard.IsPressed(Keys.F2))
            {
                EntityManager.Clear();
                EntityManager.Add(player);
                map.TryUnuseTile();
                Save.Load(Save.Name);
            }

            if (Keyboard.IsPressed(Keys.F12))
            {
                consoleOpened = !consoleOpened;
                GUIElement.Deselect();
                if (consoleOpened) GUI.Add(console);
                else GUI.Remove(console);
            }

            if (Keyboard.IsPressed(Keys.RightShift))
            {
                shadowMatrix.Create(sad == 0 ? Color.Red : (sad == 1 ? Color.Green : Color.Blue), Mouse.Position, 1,
                    30);
                sad++;
                if (sad == 3) sad = 0;
            }

            if (Keyboard.IsPressed(Keys.N))
            {
                new Item((Tiles.chest, 1), Mouse.Position);
            }

            if (Keyboard.IsPressed(Keys.F))
            {
                generator.Generate(map);
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
                Entities.zombie_0.Spawn(Mouse.Position);
            }

            ControlPlayer(Core.Player);

            EntityManager.Update();
            EffectManager.Update();

            shadowMatrix.SetPosition(a, player.transform.Position);
        }

        public void ControlPlayer(Player player)
        {
            Game.camera.Position = Vec2.Lerp(Game.camera.Position, player.transform.Position, Time.Delta * 8);
            
            player.XMovement = 0;
            if (Controls.GoRight) player.XMovement++;
            if (Controls.GoLeft) player.XMovement--;
            
            if (Controls.Jump) player.Jump();

            player.LeftDown = !Mouse.OnGUI && Controls.UpHit;
            player.RightDown = !Mouse.OnGUI && Controls.DownHit;
            
            player.LookDirection = Controls.GetRelativeCursorPosition(player.Position);

            this.player.MouseOnGUI = Mouse.OnGUI;
        }

        public override void OnResize()
        {
            waterWorld.CameraViewSet();
            shadowMatrix.Resize();
            map.CameraViewSet();
        }
    }
}