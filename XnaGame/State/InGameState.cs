using Microsoft.Xna.Framework.Graphics;
using nkast.Aether.Physics2D.Dynamics;
using System;
using System.Collections.Generic;
using XnaGame.Content;
using XnaGame.Entities;
using XnaGame.Entities.Content;
using XnaGame.Utils;
using XnaGame.Utils.Graphics;
using XnaGame.Utils.Input;
using XnaGame.WorldMap;

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
            Core.map = map = new Map(world, mapWidth, mapHeight, (x, y) => y < 15 ? null : Tiles.test);
            player = new Player(GUI,
                new Sprite(Game.Content.Load<Texture2D>("player_m_head")),
                new Sprite(Game.Content.Load<Texture2D>("player_m_arm_l")),
                new Sprite(Game.Content.Load<Texture2D>("player_m_arm_r")),
                new Sprite(Game.Content.Load<Texture2D>("player_m_body")),
                new Sprite(Game.Content.Load<Texture2D>("player_m_legs")));
        }

        public override void InitGUI()
        {
        }

        public override void Draw()
        {
            map.Draw();
            player.Draw();
            Core.entities.draw();
        }

        public override void Update()
        {
            if (Keyboard.IsPressed(Keys.Escape)) Game.Exit();

            if (Keyboard.IsPressed(Keys.E))
            {
                player.transform.Position = Mouse.Position;
            }

            if (Mouse.RightDown)
            {
                (int x, int y) pos = map.World2Cell(Mouse.Position);
                TileData data = map.GetTile(pos);
                if (data.Tile != null)
                {
                    map.SetTile(null, pos);
                    new Item((data.Tile, 1), new FVector2((pos.x + 0.5f) * Map.tileSize, (pos.y + 0.5f) * Map.tileSize));
                }
            }
            map.Update();

            player.Update();

            Core.entities.update();

            world.Step(Time.Delta);
        }
    }
}
