using Microsoft.Xna.Framework;
using System;
using Tendeos.Utils.Graphics;
using Tendeos.World;
using Tendeos.World.Content;

namespace Tendeos.Utils
{
    /// <summary>
    /// </summary>
    /// <returns>True to break.</returns>
    public delegate bool RaycastDelegate(Collider collider, Vec2 point, Vec2 normal, float distance);

    public delegate void RaycastMapDelegate(int x, int y, Vec2 point, Vec2 normal, float distance);

    public record struct RaycastHitInfo(Collider collider, Vec2 point, Vec2 normal, float distance) : IComparable<RaycastHitInfo>
    {
        public int CompareTo(RaycastHitInfo o)
        {
            if (o.distance < distance) return 1;
            return 0;
        }
    }

    public static class Physics
    {
        public const uint VELOCITY_ITERATIONS = 4;
        public const float VELOCITY_DIFFERENT = 1f / VELOCITY_ITERATIONS;
        
        public static readonly SafeList<ColliderInfo> colliders = new SafeList<ColliderInfo>();
        private static readonly RaycastHitInfo[] colliderRays = new RaycastHitInfo[256];
        private static uint hitsCount;

        public static Vec2 Gravity { get; set; } = new Vec2(0, 9.8f);
        public static float Meter { get; set; }
        public static float TileSize
        {
            get => tileSize;
            set
            {
                tileSize = value;
                tileD2Size = value/2;
            }
        }
        private static float tileSize;
        private static float tileD2Size; 
        public static IMap Map { get; set; }

        static Physics()
        {
            for (int i = 0; i < colliderRays.Length; i++)
            {
                colliderRays[i].distance = float.MaxValue;   
            }
        }

        public static Collider Create(float width, float height, float evenness, float elastic)
        {
            var collider = new Collider() { Size = new(width, height), evenness = evenness, elastic = elastic, index = colliders.Alloc() };
            colliders[collider.index] = new ColliderInfo() { collider = collider };
            return collider;
        }

        public static void Destroy(Collider collider)
        {
            colliders[collider.index] = default;
            colliders.Free(collider.index);
        }

        #region RAYCASTS
        public static void Raycast(RaycastDelegate action, Vec2 origin, Vec2 direction, float maxDistance, bool map = false)
        {
            float rdistance = origin.X + direction.X * maxDistance;
            float
                mx = Math.Min(origin.X, rdistance),
                px = Math.Max(origin.X, rdistance);
            rdistance = origin.Y + direction.Y * maxDistance;
            float
                my = Math.Min(origin.Y, rdistance),
                py = Math.Max(origin.Y, rdistance);
            int y;
            hitsCount = 0;
            for (y = 0; y < colliders.Max; y++)
            {
                ColliderInfo info = colliders[y];
                if (info.collider != null && info.collider.Overlap(mx, px, my, py)
                    && Raycast(info.collider, origin, direction, maxDistance, out Vec2 point, out Vec2 normal, out float distance))
                {
                    colliderRays[hitsCount] = new RaycastHitInfo(info.collider, point, normal, distance);
                    hitsCount++;
                }
            }

            if (map)
            {
                Vec2 rpoint = Vec2.Zero;
                Vec2 rnormal = Vec2.Zero;
                rdistance = float.MaxValue;

                var (mcx, mcy) = Map.World2Cell(new Vec2(mx, my));
                var (pcx, pcy) = Map.World2Cell(new Vec2(px, py));
                TileData tile;
                for (int x = mcx; x <= pcx; x++)
                    for (y = mcy; y <= pcy; y++)
                    {
                        tile = Map.GetTile(true, x, y);
                        if (tile.Tile is ReferenceTile)
                            tile = Map.GetTile(true, (int)tile.GetU32(0), (int)tile.GetU32(32));
                        if ((tile.Tile?.Collision ?? false)
                            && RaycastMap(x, y, origin, direction, maxDistance, tile, out Vec2 point, out Vec2 normal, out float distance)
                            && distance < rdistance)
                        {
                            rdistance = distance;
                            rnormal = normal;
                            rpoint = point;
                        }
                    }
                if (rdistance != float.MaxValue)
                {
                    colliderRays[hitsCount] = new RaycastHitInfo(null, rpoint, rnormal, rdistance);
                    hitsCount++;
                }
            }

            if (hitsCount == 0) return;

            Array.Sort(colliderRays);
            bool done = false;
            for (int i = 0; i < hitsCount; i++)
            {
                RaycastHitInfo info = colliderRays[i];

                if (!done && action(info.collider, info.point, info.normal, info.distance))
                    done = true;
                
                colliderRays[i].distance = float.MaxValue;
            }
        }

        public static void RaycastMap(RaycastMapDelegate action, Vec2 origin, Vec2 direction, float maxDistance)
        {
            Vec2 rpoint = Vec2.Zero;
            Vec2 rnormal = Vec2.Zero;
            float rdistance = origin.X + direction.X * maxDistance;
            float
                mx = Math.Min(origin.X, rdistance),
                px = Math.Max(origin.X, rdistance);
            rdistance = origin.Y + direction.Y * maxDistance;
            float
                my = Math.Min(origin.Y, rdistance),
                py = Math.Max(origin.Y, rdistance);
            rdistance = float.MaxValue;

            var (mcx, mcy) = Map.World2Cell(new Vec2(mx, my));
            var (pcx, pcy) = Map.World2Cell(new Vec2(px, py));
            int y, rx = 0, ry = 0;
            TileData tile;
            for (int x = mcx; x <= pcx; x++)
                for (y = mcy; y <= pcy; y++)
                {
                    tile = Map.GetTile(true, x, y);
                    if (tile.Tile is ReferenceTile)
                        tile = Map.GetTile(true, (int)tile.GetU32(0), (int)tile.GetU32(32));
                    if ((tile.Tile?.Collision ?? false)
                        && RaycastMap(x, y, origin, direction, maxDistance, tile, out Vec2 point, out Vec2 normal, out float distance)
                        && distance < rdistance)
                    {
                        rdistance = distance;
                        rnormal = normal;
                        rpoint = point;
                        rx = x;
                        ry = y;
                    }
                }
            if (rdistance != float.MaxValue)
            {
                action(rx, ry, rpoint, rnormal, rdistance);
            }
        }

        public static void Raycast(RaycastDelegate action, Vec2 origin, Vec2 direction, bool map = false)
        {
            float l = direction.Length();
            Raycast(action, origin, direction / l, l, map);
        }

        public static void RaycastMap(RaycastMapDelegate action, Vec2 origin, Vec2 direction)
        {
            float l = direction.Length();
            RaycastMap(action, origin, direction / l, l);
        }

        public static void Linecast(RaycastDelegate action, Vec2 from, Vec2 to, bool map = false)
        {
            to -= from;
            float len = to.Length();
            to /= len;
            Raycast(action, from, to, len, map);
        }

        public static void LinecastMap(RaycastMapDelegate action, Vec2 from, Vec2 to)
        {
            to -= from;
            float len = to.Length();
            to /= len;
            RaycastMap(action, from, to, len);
        }

        private static bool Raycast(Collider collider, Vec2 origin, Vec2 direction, float maxDistance, out Vec2 point, out Vec2 normal, out float distance)
        {
            float a = (origin.X - (collider.position.X + collider.halfSize.X)) / direction.X;
            float b = (origin.X - (collider.position.X - collider.halfSize.X)) / direction.X;
            float c = (origin.Y - (collider.position.Y + collider.halfSize.Y)) / direction.Y;
            float d = (origin.Y - (collider.position.Y - collider.halfSize.Y)) / direction.Y;

            point = Vec2.Zero;
            distance = float.MaxValue;
            normal = Vec2.Zero;

            float tMax = Math.Max(Math.Min(a, b), Math.Min(c, d));
            if (tMax > 0) return false;

            float tMin = Math.Min(Math.Max(a, b), Math.Max(c, d));
            if (-tMin > maxDistance || tMin < tMax) return false;

            point = origin - direction * tMin;
            distance = -tMin;
            if (a == tMin) normal = Vec2.UnitX;
            else if (b == tMin) normal = -Vec2.UnitX;
            else if (c == tMin) normal = Vec2.UnitY;
            else normal = -Vec2.UnitY;
            return true;
        }

        private static bool RaycastMap(int x, int y, Vec2 origin, Vec2 direction, float maxDistance, TileData data, out Vec2 point, out Vec2 normal, out float distance)
        {
            if (data.HasTriangleCollision) return RaycastTriangleTile(x, y, origin, direction, maxDistance, data, out point, out normal, out distance);
            else return RaycastAABBBTile(x, y, origin, direction, maxDistance, data, out point, out normal, out distance);
        }

        private static bool RaycastAABBBTile(int x, int y, Vec2 origin, Vec2 direction, float maxDistance, TileData data, out Vec2 point, out Vec2 normal, out float distance)
        {
            float a = (origin.X - (x * tileSize + data.CollisionXTo switch { 1 => tileD2Size, 2 => tileSize, _ => 0 })) / direction.X;
            float b = (origin.X - (x * tileSize + data.CollisionXFrom switch { 1 => tileD2Size, 2 => tileSize, _ => 0 })) / direction.X;
            float c = (origin.Y - (y * tileSize + data.CollisionYTo switch { 1 => tileD2Size, 2 => tileSize, _ => 0 })) / direction.Y;
            float d = (origin.Y - (y * tileSize + data.CollisionYFrom switch { 1 => tileD2Size, 2 => tileSize, _ => 0 })) / direction.Y;


            point = Vec2.Zero;
            distance = float.MaxValue;
            normal = Vec2.Zero;

            float tMax = Math.Max(Math.Min(a, b), Math.Min(c, d));
            if (tMax > 0) return false;

            float tMin = Math.Min(Math.Max(a, b), Math.Max(c, d));
            if (-tMin > maxDistance || tMax > 0 || tMin < tMax) return false;

            point = origin - direction * tMin;
            distance = -tMin;
            if (a == tMin) normal = Vec2.UnitX;
            else if (b == tMin) normal = -Vec2.UnitX;
            else if (c == tMin) normal = Vec2.UnitY;
            else normal = -Vec2.UnitY;
            return true;
        }
        
        private static bool RaycastTriangleTile(int x, int y, Vec2 origin, Vec2 direction, float maxDistance, TileData data, out Vec2 point, out Vec2 normal, out float distance)
        {
            Vec2 a = new Vec2(
                x * TileSize + data.CollisionXFrom switch { 1 => tileD2Size, 2 => tileSize, _ => 0 },
                y * TileSize + data.CollisionYFrom switch { 1 => tileD2Size, 2 => tileSize, _ => 0 });
            Vec2 b = new Vec2(
                x * TileSize + data.CollisionXTo switch { 1 => tileD2Size, 2 => tileSize, _ => 0 },
                y * TileSize + data.CollisionYTo switch { 1 => tileD2Size, 2 => tileSize, _ => 0 });
            Vec2 c = new Vec2(
                x * TileSize + data.CollisionXAdd switch { 1 => tileD2Size, 2 => tileSize, _ => 0 },
                y * TileSize + data.CollisionYAdd switch { 1 => tileD2Size, 2 => tileSize, _ => 0 });

            point = Vec2.Zero;
            normal = Vec2.Zero;

            float ar = RaycastEdge(a, b, origin, direction, ref normal);
            float br = RaycastEdge(b, c, origin, direction, ref normal);
            float cr = RaycastEdge(c, a, origin, direction, ref normal);

            distance = MathF.Min(ar, MathF.Min(br, cr));

            if (distance > maxDistance) return false;
            point = origin + direction * distance;

            return true;
        }

        
        private static float RaycastEdge(Vec2 a, Vec2 b, Vec2 origin, Vec2 direction, ref Vec2 normal)
        {
            Vec2 edgeNormal = new Vec2(-(b.Y - a.Y), b.X - a.X);
            edgeNormal.Normalize();

            float dot = Vec2.Dot(edgeNormal, direction);
            if (dot < 0) return float.MaxValue;
            float cross = Vec2.Cross(Vec2.Normalize(b - a), b - origin);
            if (cross < 0) return float.MaxValue;
            normal = -edgeNormal;
            return cross / dot;
        }
        #endregion

        public static void Process(float delta)
        {
            IColliderEvents events;
            int i, j, x, y, mx, my, px, py;
            Vec2 rnormal, rvel, vel;
            float rpush, bx, by;
            ColliderInfo info;
            TileData tile;
            for (i = 0; i < colliders.Max; i++)
            {
                info = colliders[i];
                if (info.collider != null && !info.collider.sleep)
                {
                    events = info.collider;
                    info.last = info.current;
                    info.current = false;
                    info.collider.velocity += Gravity * delta * Meter;
                    for (j = 0; j < VELOCITY_ITERATIONS; j++)
                    {
                        info.current = false;
                        rnormal = Vec2.Zero;
                        rpush = float.MinValue;
                        rvel = Vec2.Zero;

                        info.collider.position += info.collider.velocity * delta * VELOCITY_DIFFERENT;

                        (mx, my) = Map.World2Cell(info.collider.position - info.collider.halfSize);
                        (px, py) = Map.World2Cell(info.collider.position + info.collider.halfSize);
                        for (x = mx; x <= px; x++)
                            for (y = my; y <= py; y++)
                            {
                                tile = Map.GetTile(true, x, y);
                                if (tile.Tile is ReferenceTile)
                                    tile = Map.GetTile(true, (int)tile.GetU32(0), (int)tile.GetU32(32));
                                if (tile.Tile?.Collision ?? false)
                                {
                                    bx = x * TileSize; by = y * TileSize;
                                    vel = info.collider.velocity;
                                    if (info.collider.Process(bx, by, tile, tileD2Size, tileSize, out Vec2 normal, out float push, ref vel) && push > rpush)
                                    {
                                        rpush = push;
                                        rvel = vel;
                                        rnormal = normal;
                                        info.current = true;
                                        if (info.current)
                                        {
                                            events.OnCollision(x, y, tile);
                                            if (!info.last)
                                                events.OnCollisionEnter(x, y, tile);
                                        }
                                        else if (info.last)
                                            events.OnCollisionExit(x, y, tile);
                                    }
                                }
                            }
                        if (info.current)
                        {
                            info.collider.position += rnormal * rpush;
                            info.collider.velocity = rvel;
                        }
                    }
                }
            }
        }
    }

    public struct ColliderInfo
    {
        public Collider collider;
        public bool last, current;
    }

    public delegate void OnCollisionDelegate(int x, int y, TileData tile);

    public interface IColliderEvents
    {
        OnCollisionDelegate OnCollisionEnter { get; }
        OnCollisionDelegate OnCollisionExit { get; }
        OnCollisionDelegate OnCollision { get; }
    }

    public class Collider : IColliderEvents
    {
        public uint index;
        public Vec2 halfSize;
        public Vec2 Size
        {
            get => halfSize * 2;
            set => halfSize = value / 2;
        }

        OnCollisionDelegate IColliderEvents.OnCollisionEnter => onCollisionEnter;
        OnCollisionDelegate IColliderEvents.OnCollisionExit => onCollisionExit;
        OnCollisionDelegate IColliderEvents.OnCollision => onCollision;

        public Vec2 velocity, position;
        public object tag;
        public float evenness, elastic;
        public bool sleep;

        private OnCollisionDelegate onCollisionEnter = (a, b, c) => { };
        public event OnCollisionDelegate OnCollisionEnter { add => onCollisionEnter += value; remove => onCollisionEnter -= value; }

        private OnCollisionDelegate onCollisionExit = (a, b, c) => { };
        public event OnCollisionDelegate OnCollisionExit { add => onCollisionExit += value; remove => onCollisionExit -= value; }

        private OnCollisionDelegate onCollision = (a, b, c) => { };
        public event OnCollisionDelegate OnCollision { add => onCollision += value; remove => onCollision -= value; }

        public bool Overlap(float mx, float px, float my, float py)
        {
            return
                position.X - halfSize.X <= px &&
                position.X + halfSize.X >= mx &&
                position.Y - halfSize.Y <= py &&
                position.Y + halfSize.Y >= my;
        }

        public bool Process(float x, float y, TileData data, float tileD2Size, float tileSize, out Vec2 normal, out float pushRange, ref Vec2 velocity)
        {
            if (data.HasTriangleCollision) return ProcessTriangle(
                new Vec2(
                    x + data.CollisionXFrom switch { 1 => tileD2Size, 2 => tileSize, _ => 0 },
                    y + data.CollisionYFrom switch { 1 => tileD2Size, 2 => tileSize, _ => 0 }),
                new Vec2(
                    x + data.CollisionXTo switch { 1 => tileD2Size, 2 => tileSize, _ => 0 },
                    y + data.CollisionYTo switch { 1 => tileD2Size, 2 => tileSize, _ => 0 }),
                new Vec2(
                    x + data.CollisionXAdd switch { 1 => tileD2Size, 2 => tileSize, _ => 0 },
                    y + data.CollisionYAdd switch { 1 => tileD2Size, 2 => tileSize, _ => 0 }),
                position - halfSize, position + halfSize,
                out normal, out pushRange, ref velocity
            );
            return ProcessAABB(
                new Vector4(
                    x + data.CollisionXFrom switch { 1 => tileD2Size, 2 => tileSize, _ => 0 },
                    y + data.CollisionYFrom switch { 1 => tileD2Size, 2 => tileSize, _ => 0 },
                    x + data.CollisionXTo switch { 1 => tileD2Size, 2 => tileSize, _ => 0 },
                    y + data.CollisionYTo switch { 1 => tileD2Size, 2 => tileSize, _ => 0 }
                ),
                out normal, out pushRange, ref velocity
            );
        }

        public bool ProcessAABB(Vector4 other, out Vec2 normal, out float pushRange, ref Vec2 velocity)
        {
            float
                mox = other.Z - (position.X - halfSize.X),
                pox = position.X + halfSize.X - other.X,
                moy = other.W - (position.Y - halfSize.Y),
                poy = position.Y + halfSize.Y - other.Y;

            normal = -Vec2.UnitY;
            pushRange = poy;
            if (pushRange > pox)
            {
                normal = -Vec2.UnitX;
                pushRange = pox;
            }
            if (pushRange > moy)
            {
                normal = Vec2.UnitY;
                pushRange = moy;
            }
            if (pushRange > mox)
            {
                normal = Vec2.UnitX;
                pushRange = mox;
            }

            if (mox > 0 && pox > 0 && moy > 0 && poy > 0)
            {
                velocity = velocity * Vec2.Abs(new Vec2(normal.Y, normal.X)) * evenness - velocity * Vec2.Abs(normal) * elastic;
                return true;
            }
            return false;
        }

        public bool ProcessTriangle(Vec2 a, Vec2 b, Vec2 c, Vec2 min, Vec2 max, out Vec2 normal, out float pushRange, ref Vec2 velocity)
        {
            normal = Vec2.Zero;
            pushRange = float.MaxValue;
            
            CheckEdge(a, b, min, max, ref normal, ref pushRange);
            CheckEdge(b, c, min, max, ref normal, ref pushRange);
            CheckEdge(c, a, min, max, ref normal, ref pushRange);
            CheckCorner(a, b, c, min, max, ref normal, ref pushRange);
            CheckCorner(b, c, a, min, max, ref normal, ref pushRange);
            CheckCorner(c, a, b, min, max, ref normal, ref pushRange);

            if (pushRange > 0)
            {
                velocity = velocity * Vec2.Abs(new Vec2(normal.Y, normal.X)) * evenness - velocity * Vec2.Abs(normal) * elastic;
                return true;
            }
            return false;
        }

        private void CheckEdge(Vec2 edgeA, Vec2 edgeB, Vec2 min, Vec2 max, ref Vec2 normal, ref float pushRange)
        {
            Vec2 edgeNormal = new Vec2(edgeB.Y - edgeA.Y, -(edgeB.X - edgeA.X));

            float edgeDot =
                MathF.Max(
                    Vec2.Dot(edgeNormal, edgeB - min),
                    MathF.Max(
                        Vec2.Dot(edgeNormal, edgeB - new Vec2(max.X, min.Y)),
                        MathF.Max(
                            Vec2.Dot(edgeNormal, edgeB - new Vec2(min.X, max.Y)),
                            Vec2.Dot(edgeNormal, edgeB - max)
                        )
                    )
                );

            edgeDot = edgeDot / edgeNormal.Length();
            edgeNormal.Normalize();

            if (edgeDot < pushRange)
            {
                pushRange = edgeDot;
                normal = edgeNormal;
            }
        }

        private void CheckCorner(Vec2 corner, Vec2 otherA, Vec2 otherB, Vec2 min, Vec2 max, ref Vec2 normal, ref float pushRange)
        {
            Vec2 cornerNormal = new Vec2(-(otherB.Y - otherA.Y), otherB.X - otherA.X);

            float edgeDot =
                MathF.Max(
                    Vec2.Dot(cornerNormal, corner - min),
                    MathF.Max(
                        Vec2.Dot(cornerNormal, corner - new Vec2(max.X, min.Y)),
                        MathF.Max(
                            Vec2.Dot(cornerNormal, corner - new Vec2(min.X, max.Y)),
                            Vec2.Dot(cornerNormal, corner - max)
                        )
                    )
                );

            edgeDot = edgeDot / cornerNormal.Length();
            cornerNormal.Normalize();

            if (edgeDot < pushRange)
            {
                pushRange = edgeDot;
                normal = cornerNormal;
            }
        }
    }
}