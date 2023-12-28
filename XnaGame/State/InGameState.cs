using Microsoft.Xna.Framework.Graphics;
using nkast.Aether.Physics2D.Common;
using nkast.Aether.Physics2D.Dynamics;
using System.Collections.Generic;
using XnaGame.Content;
using XnaGame.PEntities.Content;
using XnaGame.Utils;
using XnaGame.Utils.Graphics;
using XnaGame.Utils.Input;
using XnaGame.WorldMap;
using XnaGame.WorldMap.Liquid;
using XnaGame.WorldMap.Structures;

namespace XnaGame.State
{
    public class InGameState : GameState
    {
        public const float gravity = 9.8f * Map.tileSize;

        private readonly World world;
        private Player player;
        private Map map;

        private int mapWidth = 16, mapHeight = 10;

        public InGameState(Game1 game) : base(game)
        {
            Core.world = world = new World(new FVector2(0, gravity));
            Setup();
        }

        public override void Init()
        {
            Core.map = map = new Map(new Liquid[] { Liquids.water, Liquids.foo }, world, mapWidth, mapHeight, (x, y) => y < 15 ? null : Tiles.test);
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

            if (Mouse.RightPressed)
            {
                Entities.dummy.Spawn(Mouse.Position);
            }

            map.Update();

            Core.entities.update();

            world.Step(Time.Delta);
        }
    }
}
