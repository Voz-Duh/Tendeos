using System;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tendeos.Content;
using Tendeos.Inventory;
using Tendeos.Physical.Content;
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

        private WaterWorld waterWorld;
        private WorldGenerator generator;
        // TODO: private EntitySpawner entitySpawner;
        private Player player;
        private Map map;
        private ShadowMatrix shadowMatrix;
        private PlayerInventoryContainer playerInventory;
        private SaveInstance saveInstance = new SaveInstance();
        private ThreadLoop physicsLoop;

        private int mapWidth = 30, mapHeight = 40, chunkSize = 8;

        public GameplayScene(Core game) : base(game)
        {
            physicsLoop = new ThreadLoop(Physics.Process);
        }
        uint a;

        public override async void Init()
        {
            physicsLoop.Start();
            Save.SetInstance(saveInstance);

            generator = new WorldGenerator(
                Noise.CPerlin(0.3f, 3) - Noise.CSimplex(0.2f, 4),
                (Noise.CSimplex(0.1f) + Noise.CPerlin(0.17f, 0.3f)) / 2 + Noise.CSimplex(0.03f, 1) + Noise.CSimplex(0.1f, 2)/2,
                new[]
                {
                    Biomes.test,
                    Biomes.hills,
                }, 0);

            await Task.Run(() =>
            {
                waterWorld = new WaterWorld(Game.GraphicsDevice, Game.Content, mapWidth, mapHeight, Game.camera, chunkSize);
                waterWorld.Map = Physics.Map = map = new Map(mapWidth, mapHeight, waterWorld, Game.camera, chunkSize);
                map.CameraViewSet();
                map.Ignore = Tiles.ignore;

                Physics.Meter = Physics.TileSize = map.TileSize;

                shadowMatrix = new ShadowMatrix(Game.GraphicsDevice, map, waterWorld, Game.camera)
                {
                    Smooth = ShadowMatrix.SmoothPower.Diamondly,
                    DirectionLight = Color.White.ToVector3(),
                    DirectionLightFrom = 6,
                    DirectionLightTo = 44,
                    DirectionLightRange = 10,
                    DirectionLightShadowRange = 40,
                    DirectionLightIntensity = 1
                };

                // TODO: entitySpawner = new EntitySpawner(map);

                a = shadowMatrix.Create(Color.White, 0, 0, 1, 8);

                playerInventory = new PlayerInventoryContainer(GUI, Core.PlayerInventoryStyle);

                Core.Player = player = new Player(playerInventory, Game.camera, map, Game.Content,
                    new Utils.SaveSystem.Content.PlayerInfo()
                    {
                        body = 1,
                        sex = true
                    });

                Item.GetItemDistance = map.TileSize * 2;
            });
            (saveInstance.map, saveInstance.waterWorld, saveInstance.player) =
            (map, waterWorld, player);

            if (!await Save.LoadAsync("Test"))
            {
                await generator.Generate(map);

                Save.Create("Test");
                Save.Unload();
            }
            else generator.Loaded();
            shadowMatrix.Start();
            // TODO: entitySpawner.Start();

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
            CraftMenu craftMenu = new CraftMenu(Vec2.Zero, new Vec2(11, playerInventory.style.Window.rectangle.Height), playerInventory, player.transform, Core.PlayerCraftMenuStyle, recipes);
            GUI.Add(
                new SwitchButtons(Vec2.Zero, new FRectangle(0, 0, 11, 11), Core.ButtonStyle,
                (
                    () => playerInventory.Close(player.transform.Position),
                    Core.Sprite2Icon(Core.Icons["on_inventory_icon"]),
                    () => playerInventory.Open(Vec2.Zero, Vec2.Zero),
                    Core.Sprite2Icon(Core.Icons["off_inventory_icon"])
                ),
                (
                    () =>
                    {
                        playerInventory.Close(player.transform.Position);
                        GUI.Remove(craftMenu);
                    },
                    Core.Sprite2Icon(Core.Icons["on_craft_menu_icon"]),
                    () =>
                    {
                        playerInventory.Open(Vec2.Zero, Vec2.Zero);
                        GUI.Add(craftMenu);
                    },
                    Core.Sprite2Icon(Core.Icons["off_craft_menu_icon"])
                )));
            Core.ExtraGuiDraw += DrawSelected;
            Core.ExtraGuiUpdate += UpdateSelected;
            Mods.InvokeInMains("guiInit");
        }

        public override void Clear()
        {
            EntityManager.Clear();
            waterWorld = null;
            generator = null;
            player = null;
            map = null;
            shadowMatrix = null;
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
        }
        public override void AfterDraw(SpriteBatch spriteBatch)
        {
            if (!generator.Done) return;

            waterWorld.Draw();
            shadowMatrix.Draw();
        }

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
                Save.Unload();
            }
            if (Keyboard.IsPressed(Keys.M))
            {
                EntityManager.Clear();
                EntityManager.Add(player);
                Save.Load("Test");
            }
            if (Keyboard.IsPressed(Keys.E))
            {
                player.transform.Position = Mouse.Position;
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
                Entities.dummy.Spawn(Mouse.Position);
            }

            map.Update();

            EntityManager.Update();

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
