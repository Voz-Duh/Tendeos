using Microsoft.Xna.Framework;
using XnaGame.Content;
using XnaGame.PEntities.Content;
using XnaGame.Utils;
using XnaGame.Utils.Graphics;
using XnaGame.Utils.Input;
using XnaGame.World;
using XnaGame.World.Shadows;

namespace XnaGame.State
{
    public class InGameState : GameState
    {
        public const float gravity = 9.8f * Map.tileSize;

        private Player player;
        private Map map;
        private ShadowMatrix shadowMatrix;

        private int mapWidth = 16, mapHeight = 10;

        public InGameState(Game1 game) : base(game)
        {
            Setup();
        }
        uint a, b;
        public override void Init()
        {
            Physics.meter = Physics.tileSize = Map.tileSize;
            Physics.map = Core.map = map = new Map(mapWidth, mapHeight, new WorldGenerator());
            map.ignore = Tiles.ignore;
            shadowMatrix = new ShadowMatrix(map, Game.camera);
            a = shadowMatrix.Create(Color.White, 0, 0, 1, 8);
            b = shadowMatrix.Create(Color.Red, 25, 16, 1, 16);
            player = new Player(GUI,
                Sprite.Load(Game.Content, "player_m_head"),
                Sprite.Load(Game.Content, "player_m_arm_l"),
                Sprite.Load(Game.Content, "player_m_arm_r"),
                Sprite.Load(Game.Content, "player_m_body"),
                Sprite.Load(Game.Content, "player_m_legs"));
        }

        public override void InitGUI()
        {
        }

        public override void Draw()
        {
            map.Draw();
            Core.entities.draw();
        }
        public override void AfterDraw()
        {
            shadowMatrix.Draw();
        }

        public override void Update()
        {
            if (Keyboard.IsPressed(Keys.Escape)) Game.Exit();

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

            if (Keyboard.IsPressed(Keys.L))
            {
                Entities.dummy.Spawn(Mouse.Position);
            }

            map.Update();

            Core.entities.update();

            Physics.Process(Time.Delta);
            shadowMatrix.SetPosition(a, Core.player.transform.Position);
            shadowMatrix.SetPosition(b, Mouse.Position);
        }

        public override void OnResize() => shadowMatrix.Resize();
    }
}
