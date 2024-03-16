using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Threading.Tasks;
using XnaGame.Content;
using XnaGame.Inventory;
using XnaGame.Physical.Content;
using XnaGame.UI.GUIElements;
using XnaGame.Utils;
using XnaGame.Utils.Graphics;
using XnaGame.Utils.Input;
using XnaGame.Utils.SaveSystem;
using XnaGame.World;
using XnaGame.World.Generation;
using XnaGame.World.Liquid;
using XnaGame.World.Shadows;

namespace XnaGame.Scenes
{
    public class GameplayScene : Scene
    {
        public class SaveInstance
        {
            [ToByte] public float playTime;
            [ToByte] public Player player;
            [ToByte] public IMap map;
            [ToByte] public WaterWorld waterWorld;
        }

        private WaterWorld waterWorld;
        private WorldGenerator generator;
        private Player player;
        private Map map;
        private ShadowMatrix shadowMatrix;
        private PlayerInventoryContainer playerInventory;
        private SaveInstance saveInstance = new SaveInstance();
        
        private int mapWidth = 50, mapHeight = 20, chunkSize = 8;

        public GameplayScene(Core game) : base(game)
        {
        }
        uint a;

        public override async void Init()
        {
            Save.SetInstance(saveInstance);

            generator = new WorldGenerator(new[]
            {
                Biomes.test,
                Biomes.hills,
            }, 0);

            await Task.Run(() =>
            {
                waterWorld = new WaterWorld(Game.GraphicsDevice, Game.Content, mapWidth, mapHeight, chunkSize, Game.camera);
                waterWorld.Map = Physics.Map = map = new Map(mapWidth, mapHeight, waterWorld, Game.camera, chunkSize);
                map.CameraViewSet();
                map.Ignore = Tiles.ignore;

                Physics.Meter = Physics.TileSize = map.TileSize;

                shadowMatrix = new ShadowMatrix(Game.GraphicsDevice, map, waterWorld, Game.camera)
                {
                    Smooth = true,
                    DirectionLight = Color.White.ToVector3(),
                    DirectionLightFrom = 6,
                    DirectionLightTo = 44,
                    DirectionLightRange = 10,
                    DirectionLightShadowRange = 40,
                    DirectionLightIntensity = 1
                };

                a = shadowMatrix.Create(Color.White, 0, 0, 1, 8);

                playerInventory = new PlayerInventoryContainer(GUI,
                    new Image(Vec2.Zero, new Vec2(11, 0), Sprite.Load(Game.Content, "ui/player_inventory_window")),
                    new Button.Style(Sprite.Load(Game.Content, "ui/slot_button")), 8,
                    new Vec2(4), 6, 5/*,
                    (new Vec2(69, 3), )*/);

                player = new Player(playerInventory, Game.camera, map, Game.Content,
                    new Utils.SaveSystem.Content.PlayerInfo()
                    {
                        body = 1,
                        sex = true
                    });

                Item.GetItemDistance = map.TileSize * 4;
            });
            (saveInstance.playTime, saveInstance.map, saveInstance.waterWorld, saveInstance.player) =
            (0, map, waterWorld, player);

            if (!await Save.LoadAsync("Test", saveInstance))
            {
                await generator.Generate(map);

                Save.Create("Test", saveInstance);
                Save.Unload();
            }
            else generator.Loaded();
        }

        public override void InitGUI()
        {
            GUI.Add(
                new SwitchButtons(Vec2.Zero, new FRectangle(0, 0, 11, 11), Core.buttonStyle,
                (
                    () =>
                    {
                        playerInventory.Close(player.transform.Position);
                    },
                    Core.Sprite2Icon(Core.Icons["on_inventory_icon"]),
                    () =>
                    {
                        playerInventory.Open(Vec2.Zero, Vec2.Zero);
                    },
                    Core.Sprite2Icon(Core.Icons["off_inventory_icon"])
                ))
                );
            Core.ExtraGuiDraw += DrawSelected;
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
        }

        public void DrawSelected(SpriteBatch spriteBatch)
        {
            if (Inventory.Inventory.Selected.item == null) return;
            spriteBatch.Rect(Inventory.Inventory.Selected.item.ItemSprite, Mouse.GUIPosition);
            spriteBatch.Text(Core.font, $"{Inventory.Inventory.Selected.count}", Mouse.GUIPosition + Vec2.One * -4, 1, 0, Origin.Zero, Origin.Zero);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!generator.Done)
            {
                spriteBatch.Text(Core.font, generator.Message, Vec2.Zero);
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
                Save.Load("Test", saveInstance);
            }
            if (Keyboard.IsPressed(Keys.E))
            {
                player.transform.Position = Mouse.Position;
            }

            if (Keyboard.IsPressed(Keys.O))
                new Item((Items.bow, 1), Mouse.Position);
            if (Keyboard.IsPressed(Keys.P))
                new Item((Items.pickaxe, 1), Mouse.Position);
            if (Keyboard.IsPressed(Keys.I))
                new Item((Items.pickaxeSword, 1), Mouse.Position);
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

            Physics.Process(Time.Delta);
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
