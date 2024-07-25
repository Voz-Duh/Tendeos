using Microsoft.Xna.Framework.Content;
using Tendeos.Physical;
using Tendeos.Physical.Content;
using Tendeos.Physical.Content.EnemyComponents;
using Tendeos.Utils;

namespace Tendeos.Content
{
    public static class Entities
    {
        public static Projectile arrow;
        public static EnemyBuilder
            slime,
            zombie_0, zombie_1, zombie_2, zombie_3, zombie_4, zombie_5, zombie_6, zombie_7, zombie_8, zombie_9, zombie_10, zombie_11, zombie_12, zombie_13, zombie_14, zombie_15, zombie_16, zombie_17, zombie_18, zombie_19, zombie_20, zombie_21, zombie_22, zombie_23, zombie_24, zombie_25, zombie_26, zombie_27, zombie_28, zombie_29, zombie_30, zombie_31, zombie_32;

        public static void Init()
        {
            arrow = new Projectile()
            {
                Speed = 200,
                Damage = 1
            };
            
            

            #region ZOMBIES
            var zombie_Ground = new GroundComponent()
            {
                Acceleration = 10,
                MaxSpeed = 15,
                Drag = 10,
            };
            
            var zombie_Jump = new JumpComponent(80);
            var zombie_LootDrop = new LootDropComponent(75, 3..6, "test");
            const float zombie_spawn_chance = 20;
            const float zombie_health = 10;
            const float zombie_view_radius = 120;
            Vec2 zombie_size = new Vec2(10, 20);
            
            const float zombie_framerate = 1;
            
            const int zombie_animation_idle_width = 149;
            const int zombie_animation_idle_split_rows = 6;
            const int zombie_animation_idle_split_columns = 1;
            
            const int zombie_animation_in_air_width = 24;
            const int zombie_animation_in_air_split_rows = 1;
            const int zombie_animation_in_air_split_columns = 1;
            
            const int zombie_animation_move_width = 149;
            const int zombie_animation_move_split_rows = 6;
            const int zombie_animation_move_split_columns = 1;
            
            zombie_0 = new EnemyBuilder(
                zombie_spawn_chance, zombie_health, zombie_view_radius, zombie_size, zombie_Ground, zombie_Jump, zombie_LootDrop,
                new AnimationComponent()
                {
                    AnimationIdleWidth = zombie_animation_idle_width, AnimationIdleSplitRows = zombie_animation_idle_split_rows,  AnimationIdleSplitColumns = zombie_animation_idle_split_columns, AnimationInAirWidth = zombie_animation_in_air_width, AnimationInAirSplitRows = zombie_animation_in_air_split_rows, AnimationInAirSplitColumns = zombie_animation_in_air_split_columns, AnimationMoveWidth = zombie_animation_move_width, AnimationMoveSplitRows = zombie_animation_move_split_rows, AnimationMoveSplitColumns = zombie_animation_move_split_columns, FrameRate = zombie_framerate,
                    AnimationTexture = "enemies\\zombies\\zombie_0"
                });
                zombie_1 = new EnemyBuilder(
                    zombie_spawn_chance, zombie_health, zombie_view_radius, zombie_size, zombie_Ground, zombie_Jump, zombie_LootDrop,
                    new AnimationComponent()
                    {
                                    AnimationIdleWidth = zombie_animation_idle_width, AnimationIdleSplitRows = zombie_animation_idle_split_rows,  AnimationIdleSplitColumns = zombie_animation_idle_split_columns, AnimationInAirWidth = zombie_animation_in_air_width, AnimationInAirSplitRows = zombie_animation_in_air_split_rows, AnimationInAirSplitColumns = zombie_animation_in_air_split_columns, AnimationMoveWidth = zombie_animation_move_width, AnimationMoveSplitRows = zombie_animation_move_split_rows, AnimationMoveSplitColumns = zombie_animation_move_split_columns, FrameRate = zombie_framerate,
                        AnimationTexture = "enemies\\zombies\\zombie_1"
                    });
                zombie_2 = new EnemyBuilder(
                    zombie_spawn_chance, zombie_health, zombie_view_radius, zombie_size, zombie_Ground, zombie_Jump, zombie_LootDrop,
                    new AnimationComponent()
                    {
                                    AnimationIdleWidth = zombie_animation_idle_width, AnimationIdleSplitRows = zombie_animation_idle_split_rows,  AnimationIdleSplitColumns = zombie_animation_idle_split_columns, AnimationInAirWidth = zombie_animation_in_air_width, AnimationInAirSplitRows = zombie_animation_in_air_split_rows, AnimationInAirSplitColumns = zombie_animation_in_air_split_columns, AnimationMoveWidth = zombie_animation_move_width, AnimationMoveSplitRows = zombie_animation_move_split_rows, AnimationMoveSplitColumns = zombie_animation_move_split_columns, FrameRate = zombie_framerate,
                        AnimationTexture = "enemies\\zombies\\zombie_2"
                    });
                zombie_3 = new EnemyBuilder(
                    zombie_spawn_chance, zombie_health, zombie_view_radius, zombie_size, zombie_Ground, zombie_Jump, zombie_LootDrop,
                    new AnimationComponent()
                    {
                                    AnimationIdleWidth = zombie_animation_idle_width, AnimationIdleSplitRows = zombie_animation_idle_split_rows,  AnimationIdleSplitColumns = zombie_animation_idle_split_columns, AnimationInAirWidth = zombie_animation_in_air_width, AnimationInAirSplitRows = zombie_animation_in_air_split_rows, AnimationInAirSplitColumns = zombie_animation_in_air_split_columns, AnimationMoveWidth = zombie_animation_move_width, AnimationMoveSplitRows = zombie_animation_move_split_rows, AnimationMoveSplitColumns = zombie_animation_move_split_columns, FrameRate = zombie_framerate,
                        AnimationTexture = "enemies\\zombies\\zombie_3"
                    });
                zombie_4 = new EnemyBuilder(
                    zombie_spawn_chance, zombie_health, zombie_view_radius, zombie_size, zombie_Ground, zombie_Jump, zombie_LootDrop,
                    new AnimationComponent()
                    {
                                    AnimationIdleWidth = zombie_animation_idle_width, AnimationIdleSplitRows = zombie_animation_idle_split_rows,  AnimationIdleSplitColumns = zombie_animation_idle_split_columns, AnimationInAirWidth = zombie_animation_in_air_width, AnimationInAirSplitRows = zombie_animation_in_air_split_rows, AnimationInAirSplitColumns = zombie_animation_in_air_split_columns, AnimationMoveWidth = zombie_animation_move_width, AnimationMoveSplitRows = zombie_animation_move_split_rows, AnimationMoveSplitColumns = zombie_animation_move_split_columns, FrameRate = zombie_framerate,
                        AnimationTexture = "enemies\\zombies\\zombie_4"
                    });
                zombie_5 = new EnemyBuilder(
                    zombie_spawn_chance, zombie_health, zombie_view_radius, zombie_size, zombie_Ground, zombie_Jump, zombie_LootDrop,
                    new AnimationComponent()
                    {
                                    AnimationIdleWidth = zombie_animation_idle_width, AnimationIdleSplitRows = zombie_animation_idle_split_rows,  AnimationIdleSplitColumns = zombie_animation_idle_split_columns, AnimationInAirWidth = zombie_animation_in_air_width, AnimationInAirSplitRows = zombie_animation_in_air_split_rows, AnimationInAirSplitColumns = zombie_animation_in_air_split_columns, AnimationMoveWidth = zombie_animation_move_width, AnimationMoveSplitRows = zombie_animation_move_split_rows, AnimationMoveSplitColumns = zombie_animation_move_split_columns, FrameRate = zombie_framerate,
                        AnimationTexture = "enemies\\zombies\\zombie_5"
                    });
                zombie_6 = new EnemyBuilder(
                    zombie_spawn_chance, zombie_health, zombie_view_radius, zombie_size, zombie_Ground, zombie_Jump, zombie_LootDrop,
                    new AnimationComponent()
                    {
                                    AnimationIdleWidth = zombie_animation_idle_width, AnimationIdleSplitRows = zombie_animation_idle_split_rows,  AnimationIdleSplitColumns = zombie_animation_idle_split_columns, AnimationInAirWidth = zombie_animation_in_air_width, AnimationInAirSplitRows = zombie_animation_in_air_split_rows, AnimationInAirSplitColumns = zombie_animation_in_air_split_columns, AnimationMoveWidth = zombie_animation_move_width, AnimationMoveSplitRows = zombie_animation_move_split_rows, AnimationMoveSplitColumns = zombie_animation_move_split_columns, FrameRate = zombie_framerate,
                        AnimationTexture = "enemies\\zombies\\zombie_6"
                    });
                zombie_7 = new EnemyBuilder(
                    zombie_spawn_chance, zombie_health, zombie_view_radius, zombie_size, zombie_Ground, zombie_Jump, zombie_LootDrop,
                    new AnimationComponent()
                    {
                                    AnimationIdleWidth = zombie_animation_idle_width, AnimationIdleSplitRows = zombie_animation_idle_split_rows,  AnimationIdleSplitColumns = zombie_animation_idle_split_columns, AnimationInAirWidth = zombie_animation_in_air_width, AnimationInAirSplitRows = zombie_animation_in_air_split_rows, AnimationInAirSplitColumns = zombie_animation_in_air_split_columns, AnimationMoveWidth = zombie_animation_move_width, AnimationMoveSplitRows = zombie_animation_move_split_rows, AnimationMoveSplitColumns = zombie_animation_move_split_columns, FrameRate = zombie_framerate,
                        AnimationTexture = "enemies\\zombies\\zombie_7"
                    });
                zombie_8 = new EnemyBuilder(
                    zombie_spawn_chance, zombie_health, zombie_view_radius, zombie_size, zombie_Ground, zombie_Jump, zombie_LootDrop,
                    new AnimationComponent()
                    {
                                    AnimationIdleWidth = zombie_animation_idle_width, AnimationIdleSplitRows = zombie_animation_idle_split_rows,  AnimationIdleSplitColumns = zombie_animation_idle_split_columns, AnimationInAirWidth = zombie_animation_in_air_width, AnimationInAirSplitRows = zombie_animation_in_air_split_rows, AnimationInAirSplitColumns = zombie_animation_in_air_split_columns, AnimationMoveWidth = zombie_animation_move_width, AnimationMoveSplitRows = zombie_animation_move_split_rows, AnimationMoveSplitColumns = zombie_animation_move_split_columns, FrameRate = zombie_framerate,
                        AnimationTexture = "enemies\\zombies\\zombie_8"
                    });
                zombie_9 = new EnemyBuilder(
                    zombie_spawn_chance, zombie_health, zombie_view_radius, zombie_size, zombie_Ground, zombie_Jump, zombie_LootDrop,
                    new AnimationComponent()
                    {
                                    AnimationIdleWidth = zombie_animation_idle_width, AnimationIdleSplitRows = zombie_animation_idle_split_rows,  AnimationIdleSplitColumns = zombie_animation_idle_split_columns, AnimationInAirWidth = zombie_animation_in_air_width, AnimationInAirSplitRows = zombie_animation_in_air_split_rows, AnimationInAirSplitColumns = zombie_animation_in_air_split_columns, AnimationMoveWidth = zombie_animation_move_width, AnimationMoveSplitRows = zombie_animation_move_split_rows, AnimationMoveSplitColumns = zombie_animation_move_split_columns, FrameRate = zombie_framerate,
                        AnimationTexture = "enemies\\zombies\\zombie_9"
                    });
                zombie_10 = new EnemyBuilder(
                    zombie_spawn_chance, zombie_health, zombie_view_radius, zombie_size, zombie_Ground, zombie_Jump, zombie_LootDrop,
                    new AnimationComponent()
                    {
                                    AnimationIdleWidth = zombie_animation_idle_width, AnimationIdleSplitRows = zombie_animation_idle_split_rows,  AnimationIdleSplitColumns = zombie_animation_idle_split_columns, AnimationInAirWidth = zombie_animation_in_air_width, AnimationInAirSplitRows = zombie_animation_in_air_split_rows, AnimationInAirSplitColumns = zombie_animation_in_air_split_columns, AnimationMoveWidth = zombie_animation_move_width, AnimationMoveSplitRows = zombie_animation_move_split_rows, AnimationMoveSplitColumns = zombie_animation_move_split_columns, FrameRate = zombie_framerate,
                        AnimationTexture = "enemies\\zombies\\zombie_10"
                    });
                zombie_11 = new EnemyBuilder(
                    zombie_spawn_chance, zombie_health, zombie_view_radius, zombie_size, zombie_Ground, zombie_Jump, zombie_LootDrop,
                    new AnimationComponent()
                    {
                                    AnimationIdleWidth = zombie_animation_idle_width, AnimationIdleSplitRows = zombie_animation_idle_split_rows,  AnimationIdleSplitColumns = zombie_animation_idle_split_columns, AnimationInAirWidth = zombie_animation_in_air_width, AnimationInAirSplitRows = zombie_animation_in_air_split_rows, AnimationInAirSplitColumns = zombie_animation_in_air_split_columns, AnimationMoveWidth = zombie_animation_move_width, AnimationMoveSplitRows = zombie_animation_move_split_rows, AnimationMoveSplitColumns = zombie_animation_move_split_columns, FrameRate = zombie_framerate,
                        AnimationTexture = "enemies\\zombies\\zombie_11"
                    });
                zombie_12 = new EnemyBuilder(
                    zombie_spawn_chance, zombie_health, zombie_view_radius, zombie_size, zombie_Ground, zombie_Jump, zombie_LootDrop,
                    new AnimationComponent()
                    {
                                    AnimationIdleWidth = zombie_animation_idle_width, AnimationIdleSplitRows = zombie_animation_idle_split_rows,  AnimationIdleSplitColumns = zombie_animation_idle_split_columns, AnimationInAirWidth = zombie_animation_in_air_width, AnimationInAirSplitRows = zombie_animation_in_air_split_rows, AnimationInAirSplitColumns = zombie_animation_in_air_split_columns, AnimationMoveWidth = zombie_animation_move_width, AnimationMoveSplitRows = zombie_animation_move_split_rows, AnimationMoveSplitColumns = zombie_animation_move_split_columns, FrameRate = zombie_framerate,
                        AnimationTexture = "enemies\\zombies\\zombie_12"
                    });
                zombie_13 = new EnemyBuilder(
                    zombie_spawn_chance, zombie_health, zombie_view_radius, zombie_size, zombie_Ground, zombie_Jump, zombie_LootDrop,
                    new AnimationComponent()
                    {
                                    AnimationIdleWidth = zombie_animation_idle_width, AnimationIdleSplitRows = zombie_animation_idle_split_rows,  AnimationIdleSplitColumns = zombie_animation_idle_split_columns, AnimationInAirWidth = zombie_animation_in_air_width, AnimationInAirSplitRows = zombie_animation_in_air_split_rows, AnimationInAirSplitColumns = zombie_animation_in_air_split_columns, AnimationMoveWidth = zombie_animation_move_width, AnimationMoveSplitRows = zombie_animation_move_split_rows, AnimationMoveSplitColumns = zombie_animation_move_split_columns, FrameRate = zombie_framerate,
                        AnimationTexture = "enemies\\zombies\\zombie_13"
                    });
                zombie_14 = new EnemyBuilder(
                    zombie_spawn_chance, zombie_health, zombie_view_radius, zombie_size, zombie_Ground, zombie_Jump, zombie_LootDrop,
                    new AnimationComponent()
                    {
                                    AnimationIdleWidth = zombie_animation_idle_width, AnimationIdleSplitRows = zombie_animation_idle_split_rows,  AnimationIdleSplitColumns = zombie_animation_idle_split_columns, AnimationInAirWidth = zombie_animation_in_air_width, AnimationInAirSplitRows = zombie_animation_in_air_split_rows, AnimationInAirSplitColumns = zombie_animation_in_air_split_columns, AnimationMoveWidth = zombie_animation_move_width, AnimationMoveSplitRows = zombie_animation_move_split_rows, AnimationMoveSplitColumns = zombie_animation_move_split_columns, FrameRate = zombie_framerate,
                        AnimationTexture = "enemies\\zombies\\zombie_14"
                    });
                zombie_15 = new EnemyBuilder(
                    zombie_spawn_chance, zombie_health, zombie_view_radius, zombie_size, zombie_Ground, zombie_Jump, zombie_LootDrop,
                    new AnimationComponent()
                    {
                                    AnimationIdleWidth = zombie_animation_idle_width, AnimationIdleSplitRows = zombie_animation_idle_split_rows,  AnimationIdleSplitColumns = zombie_animation_idle_split_columns, AnimationInAirWidth = zombie_animation_in_air_width, AnimationInAirSplitRows = zombie_animation_in_air_split_rows, AnimationInAirSplitColumns = zombie_animation_in_air_split_columns, AnimationMoveWidth = zombie_animation_move_width, AnimationMoveSplitRows = zombie_animation_move_split_rows, AnimationMoveSplitColumns = zombie_animation_move_split_columns, FrameRate = zombie_framerate,
                        AnimationTexture = "enemies\\zombies\\zombie_15"
                    });
                zombie_16 = new EnemyBuilder(
                    zombie_spawn_chance, zombie_health, zombie_view_radius, zombie_size, zombie_Ground, zombie_Jump, zombie_LootDrop,
                    new AnimationComponent()
                    {
                                    AnimationIdleWidth = zombie_animation_idle_width, AnimationIdleSplitRows = zombie_animation_idle_split_rows,  AnimationIdleSplitColumns = zombie_animation_idle_split_columns, AnimationInAirWidth = zombie_animation_in_air_width, AnimationInAirSplitRows = zombie_animation_in_air_split_rows, AnimationInAirSplitColumns = zombie_animation_in_air_split_columns, AnimationMoveWidth = zombie_animation_move_width, AnimationMoveSplitRows = zombie_animation_move_split_rows, AnimationMoveSplitColumns = zombie_animation_move_split_columns, FrameRate = zombie_framerate,
                        AnimationTexture = "enemies\\zombies\\zombie_16"
                    });
                zombie_17 = new EnemyBuilder(
                    zombie_spawn_chance, zombie_health, zombie_view_radius, zombie_size, zombie_Ground, zombie_Jump, zombie_LootDrop,
                    new AnimationComponent()
                    {
                                    AnimationIdleWidth = zombie_animation_idle_width, AnimationIdleSplitRows = zombie_animation_idle_split_rows,  AnimationIdleSplitColumns = zombie_animation_idle_split_columns, AnimationInAirWidth = zombie_animation_in_air_width, AnimationInAirSplitRows = zombie_animation_in_air_split_rows, AnimationInAirSplitColumns = zombie_animation_in_air_split_columns, AnimationMoveWidth = zombie_animation_move_width, AnimationMoveSplitRows = zombie_animation_move_split_rows, AnimationMoveSplitColumns = zombie_animation_move_split_columns, FrameRate = zombie_framerate,
                        AnimationTexture = "enemies\\zombies\\zombie_17"
                    });
                zombie_18 = new EnemyBuilder(
                    zombie_spawn_chance, zombie_health, zombie_view_radius, zombie_size, zombie_Ground, zombie_Jump, zombie_LootDrop,
                    new AnimationComponent()
                    {
                                    AnimationIdleWidth = zombie_animation_idle_width, AnimationIdleSplitRows = zombie_animation_idle_split_rows,  AnimationIdleSplitColumns = zombie_animation_idle_split_columns, AnimationInAirWidth = zombie_animation_in_air_width, AnimationInAirSplitRows = zombie_animation_in_air_split_rows, AnimationInAirSplitColumns = zombie_animation_in_air_split_columns, AnimationMoveWidth = zombie_animation_move_width, AnimationMoveSplitRows = zombie_animation_move_split_rows, AnimationMoveSplitColumns = zombie_animation_move_split_columns, FrameRate = zombie_framerate,
                        AnimationTexture = "enemies\\zombies\\zombie_18"
                    });
                zombie_19 = new EnemyBuilder(
                    zombie_spawn_chance, zombie_health, zombie_view_radius, zombie_size, zombie_Ground, zombie_Jump, zombie_LootDrop,
                    new AnimationComponent()
                    {
                                    AnimationIdleWidth = zombie_animation_idle_width, AnimationIdleSplitRows = zombie_animation_idle_split_rows,  AnimationIdleSplitColumns = zombie_animation_idle_split_columns, AnimationInAirWidth = zombie_animation_in_air_width, AnimationInAirSplitRows = zombie_animation_in_air_split_rows, AnimationInAirSplitColumns = zombie_animation_in_air_split_columns, AnimationMoveWidth = zombie_animation_move_width, AnimationMoveSplitRows = zombie_animation_move_split_rows, AnimationMoveSplitColumns = zombie_animation_move_split_columns, FrameRate = zombie_framerate,
                        AnimationTexture = "enemies\\zombies\\zombie_19"
                    });
                zombie_20 = new EnemyBuilder(
                    zombie_spawn_chance, zombie_health, zombie_view_radius, zombie_size, zombie_Ground, zombie_Jump, zombie_LootDrop,
                    new AnimationComponent()
                    {
                                    AnimationIdleWidth = zombie_animation_idle_width, AnimationIdleSplitRows = zombie_animation_idle_split_rows,  AnimationIdleSplitColumns = zombie_animation_idle_split_columns, AnimationInAirWidth = zombie_animation_in_air_width, AnimationInAirSplitRows = zombie_animation_in_air_split_rows, AnimationInAirSplitColumns = zombie_animation_in_air_split_columns, AnimationMoveWidth = zombie_animation_move_width, AnimationMoveSplitRows = zombie_animation_move_split_rows, AnimationMoveSplitColumns = zombie_animation_move_split_columns, FrameRate = zombie_framerate,
                        AnimationTexture = "enemies\\zombies\\zombie_20"
                    });
                zombie_21 = new EnemyBuilder(
                    zombie_spawn_chance, zombie_health, zombie_view_radius, zombie_size, zombie_Ground, zombie_Jump, zombie_LootDrop,
                    new AnimationComponent()
                    {
                                    AnimationIdleWidth = zombie_animation_idle_width, AnimationIdleSplitRows = zombie_animation_idle_split_rows,  AnimationIdleSplitColumns = zombie_animation_idle_split_columns, AnimationInAirWidth = zombie_animation_in_air_width, AnimationInAirSplitRows = zombie_animation_in_air_split_rows, AnimationInAirSplitColumns = zombie_animation_in_air_split_columns, AnimationMoveWidth = zombie_animation_move_width, AnimationMoveSplitRows = zombie_animation_move_split_rows, AnimationMoveSplitColumns = zombie_animation_move_split_columns, FrameRate = zombie_framerate,
                        AnimationTexture = "enemies\\zombies\\zombie_21"
                    });
                zombie_22 = new EnemyBuilder(
                    zombie_spawn_chance, zombie_health, zombie_view_radius, zombie_size, zombie_Ground, zombie_Jump, zombie_LootDrop,
                    new AnimationComponent()
                    {
                                    AnimationIdleWidth = zombie_animation_idle_width, AnimationIdleSplitRows = zombie_animation_idle_split_rows,  AnimationIdleSplitColumns = zombie_animation_idle_split_columns, AnimationInAirWidth = zombie_animation_in_air_width, AnimationInAirSplitRows = zombie_animation_in_air_split_rows, AnimationInAirSplitColumns = zombie_animation_in_air_split_columns, AnimationMoveWidth = zombie_animation_move_width, AnimationMoveSplitRows = zombie_animation_move_split_rows, AnimationMoveSplitColumns = zombie_animation_move_split_columns, FrameRate = zombie_framerate,
                        AnimationTexture = "enemies\\zombies\\zombie_22"
                    });
                zombie_23 = new EnemyBuilder(
                    zombie_spawn_chance, zombie_health, zombie_view_radius, zombie_size, zombie_Ground, zombie_Jump, zombie_LootDrop,
                    new AnimationComponent()
                    {
                                    AnimationIdleWidth = zombie_animation_idle_width, AnimationIdleSplitRows = zombie_animation_idle_split_rows,  AnimationIdleSplitColumns = zombie_animation_idle_split_columns, AnimationInAirWidth = zombie_animation_in_air_width, AnimationInAirSplitRows = zombie_animation_in_air_split_rows, AnimationInAirSplitColumns = zombie_animation_in_air_split_columns, AnimationMoveWidth = zombie_animation_move_width, AnimationMoveSplitRows = zombie_animation_move_split_rows, AnimationMoveSplitColumns = zombie_animation_move_split_columns, FrameRate = zombie_framerate,
                        AnimationTexture = "enemies\\zombies\\zombie_23"
                    });
                zombie_24 = new EnemyBuilder(
                    zombie_spawn_chance, zombie_health, zombie_view_radius, zombie_size, zombie_Ground, zombie_Jump, zombie_LootDrop,
                    new AnimationComponent()
                    {
                                    AnimationIdleWidth = zombie_animation_idle_width, AnimationIdleSplitRows = zombie_animation_idle_split_rows,  AnimationIdleSplitColumns = zombie_animation_idle_split_columns, AnimationInAirWidth = zombie_animation_in_air_width, AnimationInAirSplitRows = zombie_animation_in_air_split_rows, AnimationInAirSplitColumns = zombie_animation_in_air_split_columns, AnimationMoveWidth = zombie_animation_move_width, AnimationMoveSplitRows = zombie_animation_move_split_rows, AnimationMoveSplitColumns = zombie_animation_move_split_columns, FrameRate = zombie_framerate,
                        AnimationTexture = "enemies\\zombies\\zombie_24"
                    });
                zombie_25 = new EnemyBuilder(
                    zombie_spawn_chance, zombie_health, zombie_view_radius, zombie_size, zombie_Ground, zombie_Jump, zombie_LootDrop,
                    new AnimationComponent()
                    {
                                    AnimationIdleWidth = zombie_animation_idle_width, AnimationIdleSplitRows = zombie_animation_idle_split_rows,  AnimationIdleSplitColumns = zombie_animation_idle_split_columns, AnimationInAirWidth = zombie_animation_in_air_width, AnimationInAirSplitRows = zombie_animation_in_air_split_rows, AnimationInAirSplitColumns = zombie_animation_in_air_split_columns, AnimationMoveWidth = zombie_animation_move_width, AnimationMoveSplitRows = zombie_animation_move_split_rows, AnimationMoveSplitColumns = zombie_animation_move_split_columns, FrameRate = zombie_framerate,
                        AnimationTexture = "enemies\\zombies\\zombie_25"
                    });
                zombie_26 = new EnemyBuilder(
                    zombie_spawn_chance, zombie_health, zombie_view_radius, zombie_size, zombie_Ground, zombie_Jump, zombie_LootDrop,
                    new AnimationComponent()
                    {
                                    AnimationIdleWidth = zombie_animation_idle_width, AnimationIdleSplitRows = zombie_animation_idle_split_rows,  AnimationIdleSplitColumns = zombie_animation_idle_split_columns, AnimationInAirWidth = zombie_animation_in_air_width, AnimationInAirSplitRows = zombie_animation_in_air_split_rows, AnimationInAirSplitColumns = zombie_animation_in_air_split_columns, AnimationMoveWidth = zombie_animation_move_width, AnimationMoveSplitRows = zombie_animation_move_split_rows, AnimationMoveSplitColumns = zombie_animation_move_split_columns, FrameRate = zombie_framerate,
                        AnimationTexture = "enemies\\zombies\\zombie_26"
                    });
                zombie_27 = new EnemyBuilder(
                    zombie_spawn_chance, zombie_health, zombie_view_radius, zombie_size, zombie_Ground, zombie_Jump, zombie_LootDrop,
                    new AnimationComponent()
                    {
                                    AnimationIdleWidth = zombie_animation_idle_width, AnimationIdleSplitRows = zombie_animation_idle_split_rows,  AnimationIdleSplitColumns = zombie_animation_idle_split_columns, AnimationInAirWidth = zombie_animation_in_air_width, AnimationInAirSplitRows = zombie_animation_in_air_split_rows, AnimationInAirSplitColumns = zombie_animation_in_air_split_columns, AnimationMoveWidth = zombie_animation_move_width, AnimationMoveSplitRows = zombie_animation_move_split_rows, AnimationMoveSplitColumns = zombie_animation_move_split_columns, FrameRate = zombie_framerate,
                        AnimationTexture = "enemies\\zombies\\zombie_27"
                    });
                zombie_28 = new EnemyBuilder(
                    zombie_spawn_chance, zombie_health, zombie_view_radius, zombie_size, zombie_Ground, zombie_Jump, zombie_LootDrop,
                    new AnimationComponent()
                    {
                                    AnimationIdleWidth = zombie_animation_idle_width, AnimationIdleSplitRows = zombie_animation_idle_split_rows,  AnimationIdleSplitColumns = zombie_animation_idle_split_columns, AnimationInAirWidth = zombie_animation_in_air_width, AnimationInAirSplitRows = zombie_animation_in_air_split_rows, AnimationInAirSplitColumns = zombie_animation_in_air_split_columns, AnimationMoveWidth = zombie_animation_move_width, AnimationMoveSplitRows = zombie_animation_move_split_rows, AnimationMoveSplitColumns = zombie_animation_move_split_columns, FrameRate = zombie_framerate,
                        AnimationTexture = "enemies\\zombies\\zombie_28"
                    });
                zombie_29 = new EnemyBuilder(
                    zombie_spawn_chance, zombie_health, zombie_view_radius, zombie_size, zombie_Ground, zombie_Jump, zombie_LootDrop,
                    new AnimationComponent()
                    {
                                    AnimationIdleWidth = zombie_animation_idle_width, AnimationIdleSplitRows = zombie_animation_idle_split_rows,  AnimationIdleSplitColumns = zombie_animation_idle_split_columns, AnimationInAirWidth = zombie_animation_in_air_width, AnimationInAirSplitRows = zombie_animation_in_air_split_rows, AnimationInAirSplitColumns = zombie_animation_in_air_split_columns, AnimationMoveWidth = zombie_animation_move_width, AnimationMoveSplitRows = zombie_animation_move_split_rows, AnimationMoveSplitColumns = zombie_animation_move_split_columns, FrameRate = zombie_framerate,
                        AnimationTexture = "enemies\\zombies\\zombie_29"
                    });
                zombie_30 = new EnemyBuilder(
                    zombie_spawn_chance, zombie_health, zombie_view_radius, zombie_size, zombie_Ground, zombie_Jump, zombie_LootDrop,
                    new AnimationComponent()
                    {
                                    AnimationIdleWidth = zombie_animation_idle_width, AnimationIdleSplitRows = zombie_animation_idle_split_rows,  AnimationIdleSplitColumns = zombie_animation_idle_split_columns, AnimationInAirWidth = zombie_animation_in_air_width, AnimationInAirSplitRows = zombie_animation_in_air_split_rows, AnimationInAirSplitColumns = zombie_animation_in_air_split_columns, AnimationMoveWidth = zombie_animation_move_width, AnimationMoveSplitRows = zombie_animation_move_split_rows, AnimationMoveSplitColumns = zombie_animation_move_split_columns, FrameRate = zombie_framerate,
                        AnimationTexture = "enemies\\zombies\\zombie_30"
                    });
                zombie_31 = new EnemyBuilder(
                    zombie_spawn_chance, zombie_health, zombie_view_radius, zombie_size, zombie_Ground, zombie_Jump, zombie_LootDrop,
                    new AnimationComponent()
                    {
                                    AnimationIdleWidth = zombie_animation_idle_width, AnimationIdleSplitRows = zombie_animation_idle_split_rows,  AnimationIdleSplitColumns = zombie_animation_idle_split_columns, AnimationInAirWidth = zombie_animation_in_air_width, AnimationInAirSplitRows = zombie_animation_in_air_split_rows, AnimationInAirSplitColumns = zombie_animation_in_air_split_columns, AnimationMoveWidth = zombie_animation_move_width, AnimationMoveSplitRows = zombie_animation_move_split_rows, AnimationMoveSplitColumns = zombie_animation_move_split_columns, FrameRate = zombie_framerate,
                        AnimationTexture = "enemies\\zombies\\zombie_31"
                    });
                zombie_32 = new EnemyBuilder(
                    zombie_spawn_chance, zombie_health, zombie_view_radius, zombie_size, zombie_Ground, zombie_Jump, zombie_LootDrop,
                    new AnimationComponent()
                    {
                                    AnimationIdleWidth = zombie_animation_idle_width, AnimationIdleSplitRows = zombie_animation_idle_split_rows,  AnimationIdleSplitColumns = zombie_animation_idle_split_columns, AnimationInAirWidth = zombie_animation_in_air_width, AnimationInAirSplitRows = zombie_animation_in_air_split_rows, AnimationInAirSplitColumns = zombie_animation_in_air_split_columns, AnimationMoveWidth = zombie_animation_move_width, AnimationMoveSplitRows = zombie_animation_move_split_rows, AnimationMoveSplitColumns = zombie_animation_move_split_columns, FrameRate = zombie_framerate,
                        AnimationTexture = "enemies\\zombies\\zombie_32"
                    });
            #endregion
        }

        public static Entity Get(string value) => (Entity) typeof(Entities).GetField(value).GetValue(null);
    }

    public delegate T EntityRef<T>() where T : Entity;
}