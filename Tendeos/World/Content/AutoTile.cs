using System;
using Microsoft.Xna.Framework;
using Tendeos.Content.Utlis;
using Tendeos.Utils;
using Tendeos.Utils.Graphics;

namespace Tendeos.World.Content
{
    public class AutoTile : Tile
    {
        public static readonly byte[] Collisions =
        {
            0, 0, 0, 0, 3, 4, 1, 1, 1, 1, 0,
            0, 0, 0, 0, 5, 6, 2, 2, 2, 2, 0,
            0, 0, 0, 0, 7, 8, 1, 1, 1, 1, 0,
            0, 0, 0, 0, 9, 10, 2, 2, 2, 2, 0,
        };

        public const byte Down = 7, Up = 11 + 7, Full = 0;

        [SpriteLoad("@"), SplitSprite(11, 4, 1)]
        protected Sprite[] sprites;

        public override void Changed(bool top, IMap map, int x, int y, ref TileData data)
        {
            UpdateTile(top, map, x, y, ref data);
        }

        public override void Draw(SpriteBatch spriteBatch, bool top, IMap map, int x, int y, Vec2 drawPosition,
            TileData data)
        {
            float g = data.Health / Health;
            Color color = new Color(top ? new Vector3(g) : new Vector3(g) * dark.ToVector3());

            byte sprite = data.GetU6(0);
            spriteBatch.Rect(color, sprites[sprite], drawPosition, 1.01f);
            byte lt = data.GetU2(6);
            byte rt = data.GetU2(8);
            if (Collisions[sprite] == 0)
            {
                if (lt != 0) spriteBatch.Rect(color, sprites[lt switch {1 => 10, 2 => 11 + 10}], drawPosition, 1.01f);
                if (rt != 0)
                    spriteBatch.Rect(color, sprites[rt switch {1 => 11 + 11 + 10, 2 => 11 + 11 + 11 + 10}],
                        drawPosition, 1.01f);
            }
        }

        public override void DrawScheme(SpriteBatch spriteBatch, Vec2 drawPosition, bool valid)
        {
            spriteBatch.Rect(valid ? scheme : invalideScheme, sprites[11 + 11 + 11 + 3], drawPosition, 1.01f);
        }

        public override void Start(bool top, IMap map, int x, int y, ref TileData data)
        {
            UpdateTile(top, map, x, y, ref data);
        }

        private static void UpdateTile(bool top, IMap map, int x, int y, ref TileData data)
        {
            data.SetU2(6, 0);
            data.SetU2(8, 0);
            bool isTriangle = data.HasTriangleCollision;
            byte tileType = Collisions[data.GetU6(0)];
            TileData
                ld = map.GetTile(top, x - 1, y),
                rd = map.GetTile(top, x + 1, y),
                ud = map.GetTile(top, x, y - 1),
                dd = map.GetTile(top, x, y + 1);
            bool lb = ld.Tile is AutoTile,
                rb = rd.Tile is AutoTile,
                ub = ud.Tile is AutoTile,
                db = dd.Tile is AutoTile;
            byte lt = lb ? Collisions[ld.GetU6(0)] : (byte) 0,
                rt = rb ? Collisions[rd.GetU6(0)] : (byte) 0,
                ut = ub ? Collisions[ud.GetU6(0)] : (byte) 0,
                dt = db ? Collisions[dd.GetU6(0)] : (byte) 0;
            byte sprite = tileType switch
            {
                9 or 10 or 2 => UpdateUpTile(top, map, x, y, ref ld, ref rd, ub, rb, lb, isTriangle, lt, rt, ut),
                7 or 8 or 1 => UpdateDownTile(top, map, x, y, ref ld, ref rd, db, rb, lb, isTriangle, lt, rt, dt),
                _ => UpdateFullTile(top, map, x, y, ref data, ref ld, ref rd, lb, rb, db, ub, isTriangle, lt, rt, dt,
                    ut),
            };
            data.SetU6(0, sprite);
            SetColision(sprite, ref data);
        }

        private static byte UpdateFullTile(bool top, IMap map, int x, int y, ref TileData data, ref TileData ldata,
            ref TileData rdata, bool ld, bool rd, bool dd, bool ud, bool isTriangle, byte lt, byte rt, byte dt, byte ut)
        {
            bool l = !ld || lt == 2 || lt == 1 || lt == 4 || lt == 6 || lt == 7 || lt == 9 || lt == 8 || lt == 10,
                r = !rd || rt == 2 || rt == 1 || rt == 3 || rt == 5 || rt == 7 || rt == 9 || rt == 8 || rt == 10,
                d = !dd || dt == 1 || dt == 3 || dt == 4 || dt == 7 || dt == 8,
                u = !ud || ut == 2 || ut == 5 || ut == 6 || ut == 9 || ut == 10;
            bool lr = l && r,
                du = d && u;
            byte res = 11 + 1;

            if (d) res = 11 + 11 + 1;
            if (u) res = 1;

            if (r) res = 11 + 2;
            if (l) res = 11;

            if (isTriangle)
            {
                if (l && d) res = 11 + 4;
                if (r && d) res = 11 + 5;
                if (l && u) res = 4;
                if (r && u) res = 5;
                if (rdata.IsReference) rdata.Tile?.Changed(top, map, x + 1, y, ref rdata);
                if (ldata.IsReference) ldata.Tile?.Changed(top, map, x - 1, y, ref ldata);
            }
            else
            {
                if (l && d) res = 11 + 11;
                if (r && d) res = 11 + 11 + 2;
                if (l && u) res = 0;
                if (r && u) res = 2;
            }

            if (lr) res = 11 + 3;
            if (du) res = 11 + 11 + 11 + 1;

            if (lr && d) res = 11 + 11 + 3;
            if (lr && u) res = 3;

            if (du && l) res = 11 + 11 + 11;
            if (du && r) res = 11 + 11 + 11 + 2;

            if (lr && du) res = 11 + 11 + 11 + 3;

            data.SetU2(6, lt switch {1 or 7 => 1, 2 or 9 => 2, _ => 0});
            data.SetU2(8, rt switch {1 or 8 => 1, 2 or 10 => 2, _ => 0});

            return res;
        }

        private static byte UpdateUpTile(bool top, IMap map, int x, int y, ref TileData ld, ref TileData rd, bool ud,
            bool rb, bool lb, bool isTriangle, byte lt, byte rt, byte ut)
        {
            bool l = !lb || lt == 1 || lt == 4 || lt == 6 || lt == 8 || lt == 10,
                r = !rb || rt == 1 || rt == 3 || rt == 5 || rt == 7 || rt == 9,
                u = !ud || ut == 2 || ut == 5 || ut == 6 || ut == 9 || ut == 10;
            bool lr = l && r;
            byte res = 11 + 11 + 11 + 7;

            if (isTriangle)
            {
                if (r) res = 11 + 11 + 11 + 5;
                if (l) res = 11 + 11 + 11 + 4;
                if (rd.IsReference) rd.Tile?.Changed(top, map, x + 1, y, ref rd);
                if (ld.IsReference) ld.Tile?.Changed(top, map, x - 1, y, ref ld);
            }
            else
            {
                if (r) res = 11 + 11 + 11 + 8;
                if (l) res = 11 + 11 + 11 + 6;
            }

            if (u) res = 11 + 7;
            if (l && u) res = 11 + 6;
            if (r && u) res = 11 + 8;

            if (lr) res = 11 + 9;

            if (lr && u) res = 11 + 11 + 11 + 9;

            return res;
        }

        private static byte UpdateDownTile(bool top, IMap map, int x, int y, ref TileData ld, ref TileData rd, bool dd,
            bool rb, bool lb, bool isTriangle, byte lt, byte rt, byte dt)
        {
            bool l = !lb || lt == 2 || lt == 4 || lt == 6 || lt == 8 || lt == 10,
                r = !rb || rt == 2 || rt == 3 || rt == 5 || rt == 7 || rt == 9,
                d = !dd || dt == 1 || dt == 3 || dt == 4 || dt == 7 || dt == 8;
            bool lr = l && r;
            byte res = 11 + 11 + 7;

            if (isTriangle)
            {
                if (r) res = 11 + 11 + 5;
                if (l) res = 11 + 11 + 4;
                if (rd.IsReference) rd.Tile?.Changed(top, map, x + 1, y, ref rd);
                if (ld.IsReference) ld.Tile?.Changed(top, map, x - 1, y, ref ld);
            }
            else
            {
                if (r) res = 11 + 11 + 8;
                if (l) res = 11 + 11 + 6;
            }

            if (d) res = 7;
            if (l && d) res = 6;
            if (r && d) res = 8;

            if (lr) res = 9;

            if (lr && d) res = 11 + 11 + 9;

            return res;
        }

        private static void SetColision(byte sprite, ref TileData data)
        {
            var (HasTriangleCollision, IsFloor, XFrom, YFrom, XTo, YTo, XAdd, YAdd) = GetCollisionData(sprite);
            data.HasTriangleCollision = HasTriangleCollision;
            data.IsFloor = IsFloor;
            data.CollisionXFrom = XFrom;
            data.CollisionYFrom = YFrom;
            data.CollisionXTo = XTo;
            data.CollisionYTo = YTo;
            data.CollisionXAdd = XAdd;
            data.CollisionYAdd = YAdd;
        }

        private static (bool, bool, byte, byte, byte, byte, byte, byte) GetCollisionData(byte sprite)
        {
            switch (Collisions[sprite])
            {
                case 0: return (false, true, 0, 0, 2, 2, 0, 0);
                case 1: return (false, false, 0, 1, 2, 2, 0, 0); // down 1 > 3 4 7 8
                case 2: return (false, true, 0, 0, 2, 1, 0, 0); // up 2 > 5 6 9 10
                case 3: return (true, false, 0, 2, 2, 0, 2, 2); // down
                case 4: return (true, false, 0, 0, 2, 2, 0, 2); // down
                case 5: return (true, true, 0, 0, 2, 0, 2, 2); // up
                case 6: return (true, true, 0, 0, 2, 0, 0, 2); // up
                case 7: return (true, false, 0, 2, 2, 1, 2, 2); // down
                case 8: return (true, false, 0, 1, 2, 0, 2, 2); // down
                case 9: return (true, true, 0, 0, 2, 0, 2, 1); // up
                case 10: return (true, true, 0, 0, 2, 0, 0, 1); // up
                default: throw new InvalidOperationException("Invalid sprite index");
            }
        }
    }
}