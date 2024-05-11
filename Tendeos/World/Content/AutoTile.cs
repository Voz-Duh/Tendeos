using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tendeos.Content.Utlis;
using Tendeos.Utils;
using Tendeos.Utils.Graphics;

namespace Tendeos.World.Content
{
    public class AutoTile : Tile
    {
        public static readonly byte[] Collisions = {
            0,0,0,0,3,4,1,1,1,1,0,
            0,0,0,0,5,6,2,2,2,2,0,
            0,0,0,0,7,8,1,1,1,1,0,
            0,0,0,0,9,10,2,2,2,2,0,
        };

        public const byte Down = 7, Up = 11+7, Full = 0;

        [SpriteLoad("@", 11, 4, 1)]
        public Sprite[] sprites;

        public override void Changed(bool top, IMap map, int x, int y, ref TileData data)
        {
            UpdateTile(top, map, x, y, ref data);
        }

        public override void Draw(SpriteBatch spriteBatch, bool top, IMap map, int x, int y, Vec2 drawPosition, TileData data)
        {
            float g = data.Health / Health;
            byte sprite = data.GetU6(0);
            spriteBatch.Rect(sprites[sprite], new Color(top ? new Vector3(g) : new Vector3(g) * dark.ToVector3()), drawPosition, 0, 1.01f);
            byte lt = data.GetU2(6);
            byte rt = data.GetU2(8);
            if (Collisions[sprite] == 0)
            {
                if (lt != 0) spriteBatch.Rect(sprites[lt switch { 1=>10, 2=>11+10 }], new Color(top ? new Vector3(g) : new Vector3(g) * dark.ToVector3()), drawPosition, 0, 1.01f);
                if (rt != 0) spriteBatch.Rect(sprites[rt switch { 1=>11+11+10, 2=>11+11+11+10 }], new Color(top ? new Vector3(g) : new Vector3(g) * dark.ToVector3()), drawPosition, 0, 1.01f);
            }
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
            byte lt = lb ? Collisions[ld.GetU6(0)] : (byte)0,
                 rt = rb ? Collisions[rd.GetU6(0)] : (byte)0,
                 ut = ub ? Collisions[ud.GetU6(0)] : (byte)0,
                 dt = db ? Collisions[dd.GetU6(0)] : (byte)0;
            byte sprite = tileType switch
            {
                2 => UpdateUpTile(top, map, x, y, ref ld, ref rd, ub, isTriangle, lt, rt, ut),
                1 => UpdateDownTile(top, map, x, y, ref ld, ref rd, db, isTriangle, lt, rt, dt),
                _ => UpdateFullTile(ref data, lb, rb, db, ub, isTriangle, lt, rt, dt, ut),
            };
            data.SetU6(0, sprite);
            SetColision(sprite, ref data);
        }

        private static byte UpdateFullTile(ref TileData data, bool ld, bool rd, bool dd, bool ud, bool isTriangle, byte lt, byte rt, byte dt, byte ut)
        {
            bool l = !ld || lt == 2 || lt == 1 || lt == 4 || lt == 6 || lt == 8 || lt == 10, r = !rd || rt == 2 || rt == 1 || rt == 3 || rt == 5 || rt == 7 || rt == 9,
                 d = !dd || dt == 1 || dt == 3 || dt == 4 || dt == 7 || dt == 8,            u = !ud || ut == 2 || ut == 5 || ut == 7 || ut == 9 || ut == 10;
            bool lr = l && r,
                 du = d && u;
            byte res = 11+1;

            if (d) res = 11+11+1;
            if (u) res = 1;

            if (r) res = 11+2;
            if (l) res = 11;

            if (l && d) res = (byte)(isTriangle ? 11+4 : 11+11);
            if (r && d) res = (byte)(isTriangle ? 11+5 : 11+11+2);
            if (l && u) res = (byte)(isTriangle ? 4 : 0);
            if (r && u) res = (byte)(isTriangle ? 5 : 2);

            if (lr) res = 11+3;
            if (du) res = 11+11+11+1;

            if (lr && d) res = 11+11+3;
            if (lr && u) res = 3;

            if (du && l) res = 11+11+11;
            if (du && r) res = 11+11+11+2;

            if (lr && du) res = 11+11+11+3;
            
            data.SetU2(6, lt switch { 1=>1, 2=>2, _=>0 });
            data.SetU2(8, rt switch { 1=>1, 2=>2, _=>0 });

            return res;
        }

        private static byte UpdateUpTile(bool top, IMap map, int x, int y, ref TileData ld, ref TileData rd, bool ud, bool isTriangle, byte lt, byte rt, byte ut)
        {
            bool l = ld.Tile is not AutoTile || lt == 1 || lt == 4 || lt == 6 || lt == 8 || lt == 10, r = rd.Tile is not AutoTile || rt == 1 || rt == 3 || rt == 5 || rt == 7 || rt == 9,
                 u = !ud || ut == 2 || ut == 5 || ut == 7 || ut == 9 || ut == 10;
            bool lr = l && r;
            byte res = 11+11+11+7;

            if (isTriangle)
            {
                if (r) res = 11+11+11+4;
                if (l) res = 11+11+11+5;
                if (rd.IsReference) rd.Tile?.Changed(top, map, x + 1, y, ref rd);
                if (ld.IsReference) ld.Tile?.Changed(top, map, x - 1, y, ref ld);
            }
            else
            {
                if (r) res = 11+11+11+8;
                if (l) res = 11+11+11+6;
            }
            
            if (u) res = 11+7;
            if (l && u) res = 11+6;
            if (r && u) res = 11+8;

            if (lr) res = 11+9;

            if (lr && u) res = 11+11+11+9;

            return res;
        }

        private static byte UpdateDownTile(bool top, IMap map, int x, int y, ref TileData ld, ref TileData rd, bool dd, bool isTriangle, byte lt, byte rt, byte dt)
        {
            bool l = ld.Tile is not AutoTile || lt == 2 || lt == 4 || lt == 6 || lt == 8 || lt == 10, r = rd.Tile is not AutoTile || rt == 2 || rt == 3 || rt == 5 || rt == 7 || rt == 9,
                 d = !dd || dt == 1 || dt == 3 || dt == 4 || dt == 7 || dt == 8;
            bool lr = l && r;
            byte res = 11+11+7;

            if (isTriangle)
            {
                if (r) res = 11+11+4;
                if (l) res = 11+11+5;
                if (rd.IsReference) rd.Tile?.Changed(top, map, x + 1, y, ref rd);
                if (ld.IsReference) ld.Tile?.Changed(top, map, x - 1, y, ref ld);
            }
            else
            {
                if (r) res = 11+11+8;
                if (l) res = 11+11+6;
            }
            
            if (d) res = 7;
            if (l && d) res = 6;
            if (r && d) res = 8;

            if (lr) res = 9;

            if (lr && d) res = 11+11+9;

            return res;
        }

        private static void SetColision(byte sprite, ref TileData data)
        {
            var (HasTriangleCollision, XFrom, YFrom, XTo, YTo, XAdd, YAdd) = GetCollisionData(sprite);
            data.HasTriangleCollision = HasTriangleCollision;
            data.CollisionXFrom = XFrom;
            data.CollisionYFrom = YFrom;
            data.CollisionXTo = XTo;
            data.CollisionYTo = YTo;
            data.CollisionXAdd = XAdd;
            data.CollisionYAdd = YAdd;
        }

        private static (bool, byte, byte, byte, byte, byte, byte) GetCollisionData(byte sprite)
        {
            switch (Collisions[sprite])
            {
                case 0: return (false, 0, 0, 2, 2, 0, 0);
                case 1: return (false, 0, 1, 2, 2, 0, 0); // down 1 > 3 4 7 8
                case 2: return (false, 0, 0, 2, 1, 0, 0); // up 2 > 5 6 9 10
                case 3: return (true, 0, 2, 2, 0, 2, 2); // down
                case 4: return (true, 0, 0, 2, 2, 0, 2); // down
                case 5: return (true, 0, 0, 2, 0, 2, 2); // up
                case 6: return (true, 0, 0, 2, 0, 0, 2); // up
                case 7: return (true, 0, 2, 2, 1, 2, 2); // down
                case 8: return (true, 0, 1, 2, 0, 2, 2); // down
                case 9: return (true, 0, 0, 2, 0, 2, 1); // up
                case 10: return (true, 0, 0, 2, 0, 0, 1);// up
                default: throw new InvalidOperationException("Invalid sprite index");
            }
        }

        private struct CollisionData
        {
            public bool HasTriangleCollision { get; set; }
            public int XFrom { get; set; }
            public int YFrom { get; set; }
            public int XTo { get; set; }
            public int YTo { get; set; }
            public int XAdd { get; set; }
            public int YAdd { get; set; }
        }
    }
}
