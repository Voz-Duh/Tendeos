using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using XnaGame.World;

namespace XnaGame.Utils
{
    /// <summary>
    /// </summary>
    /// <returns>True to break.</returns>
    public delegate bool RaycastDelegate(Collider collider, Vec2 point, Vec2 normal, float distance);

    public delegate void RaycastMapDelegate(Vec2 point, Vec2 normal, float distance);

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
        static readonly Queue<uint> free = new Queue<uint>();
        static uint length;
        static readonly RaycastHitInfo[] colliderRays = new RaycastHitInfo[256];
        static uint hitsCount;
        static readonly ColliderInfo[] colliders = new ColliderInfo[255];
        public const uint VELOCITY_ITERATIONS = 4;
        public const float VELOCITY_DIFFERENT = 1f / VELOCITY_ITERATIONS;
        public static Vec2 gravity = new Vec2(0, 9.8f);
        public static float meter, tileSize;
        public static IMap map;

        public static Collider Create(float width, float height, float evenness, float elastic)
        {
            var col = new Collider() { Size = new(width, height), evenness = evenness, elastic = elastic };
            if (free.TryDequeue(out uint f))
            {
                colliders[f].collider = col;
                col.index = f;
            }
            else
            {
                colliders[length].collider = col;
                col.index = length;
                length++;
            }
            return col;
        }

        public static void Destroy(Collider collider)
        {
            colliders[collider.index].collider = null;
            colliders[collider.index].last = colliders[collider.index].current = false;
            free.Enqueue(collider.index);
        }

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
            hitsCount = 0;
            foreach (ColliderInfo info in colliders)
                if (info.collider != null && info.collider.Overlap(mx, px, my, py)
                    && Raycast(info.collider, origin, direction, maxDistance, out Vec2 point, out Vec2 normal, out float distance))
                {
                    colliderRays[hitsCount] = new RaycastHitInfo(info.collider, point, normal, distance);
                    hitsCount++;
                }
            if (map)
            {
                Vec2 rpoint = Vec2.Zero;
                Vec2 rnormal = Vec2.Zero;
                rdistance = float.MinValue;

                var (mcx, mcy) = Physics.map.World2Cell(new Vec2(mx, my));
                var (pcx, pcy) = Physics.map.World2Cell(new Vec2(px, py));
                for (int x = mcx; x <= pcx; x++)
                    for (int y = mcy; y <= pcy; y++)
                        if (Physics.map.GetTile(true, x, y).Tile != null
                            && RaycastMap(x, y, origin, direction, maxDistance, out Vec2 point, out Vec2 normal, out float distance)
                            && distance > rdistance)
                        {
                            rdistance = distance;
                            rnormal = normal;
                            rpoint = point;
                        }
                if (rdistance != float.MinValue)
                {
                    colliderRays[hitsCount] = new RaycastHitInfo(null, rpoint, rnormal, rdistance);
                    hitsCount++;
                }
            }

            if (hitsCount == 0) return;

            Array.Sort(colliderRays);
            for (int i = 0; i < hitsCount; i++)
            {
                RaycastHitInfo info = colliderRays[i];
                if (action(info.collider, info.point, info.normal, info.distance))
                    break;
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
            rdistance = float.MinValue;

            var (mcx, mcy) = map.World2Cell(new Vec2(mx, my));
            var (pcx, pcy) = map.World2Cell(new Vec2(px, py));
            for (int x = mcx; x <= pcx; x++)
                for (int y = mcy; y <= pcy; y++)
                    if (map.GetTile(true, x, y).Tile != null
                        && RaycastMap(x, y, origin, direction, maxDistance, out Vec2 point, out Vec2 normal, out float distance)
                        && distance > rdistance)
                    {
                        rdistance = distance;
                        rnormal = normal;
                        rpoint = point;
                    }
            if (rdistance != float.MinValue) action(rpoint, rnormal, rdistance);
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
            float ap = origin.X - (collider.position.X + collider.halfSize.X);
            float bp = origin.X - (collider.position.X - collider.halfSize.X);
            float cp = origin.Y - (collider.position.Y + collider.halfSize.Y);
            float dp = origin.Y - (collider.position.Y - collider.halfSize.Y);
            float a = ap / direction.X;
            float b = bp / direction.X;
            float c = cp / direction.Y;
            float d = dp / direction.Y;

            float tMin = Math.Min(Math.Max(a, b), Math.Max(c, d));
            float tMax = Math.Max(Math.Min(a, b), Math.Min(c, d));

            point = origin - direction * tMin;
            distance = tMin;
            if (a == tMin) normal = Vec2.UnitX;
            else if (b == tMin) normal = -Vec2.UnitX;
            else if (c == tMin) normal = Vec2.UnitY;
            else normal = -Vec2.UnitY;
            return tMax <= 0 && tMin <= maxDistance && tMin >= tMax;
        }

        private static bool RaycastMap(int x, int y, Vec2 origin, Vec2 direction, float maxDistance, out Vec2 point, out Vec2 normal, out float distance)
        {
            float ap = origin.X - (x * tileSize + tileSize);
            float bp = origin.X - x * tileSize;
            float cp = origin.Y - (y * tileSize + tileSize);
            float dp = origin.Y - y * tileSize;
            float a = ap / direction.X;
            float b = bp / direction.X;
            float c = cp / direction.Y;
            float d = dp / direction.Y;

            float tMin = Math.Min(Math.Max(a, b), Math.Max(c, d));
            float tMax = Math.Max(Math.Min(a, b), Math.Min(c, d));

            point = origin - direction * tMin;
            distance = tMin;
            if (a == tMin) normal = Vec2.UnitX;
            else if (b == tMin) normal = -Vec2.UnitX;
            else if (c == tMin) normal = Vec2.UnitY;
            else normal = -Vec2.UnitY;
            return tMax <= 0 && tMin <= maxDistance && tMin >= tMax;
        }

        public static void Process(float delta)
        {
            for (int i = 0; i < length; i++)
            {
                var info = colliders[i];
                if (info.collider != null && !info.collider.sleep)
                {
                    IColliderEvents events = info.collider;
                    info.last = info.current;
                    info.current = false;
                    info.collider.velocity += gravity * delta * meter;
                    for (int j = 0; j < VELOCITY_ITERATIONS; j++)
                    {
                        info.current = false;
                        Vec2 rnormal = Vec2.Zero;
                        float rpush = float.MinValue;
                        Vec2 rvel = Vec2.Zero;

                        info.collider.position += info.collider.velocity * delta * VELOCITY_DIFFERENT;

                        var (mx, my) = map.World2Cell(info.collider.position - info.collider.halfSize);
                        var (px, py) = map.World2Cell(info.collider.position + info.collider.halfSize);
                        for (int x = mx; x <= px; x++)
                            for (int y = my; y <= py; y++)
                            {
                                TileData tile = map.GetTile(true, x, y);
                                if (tile.Tile != null)
                                {
                                    float bx = x * tileSize, by = y * tileSize;
                                    Vec2 vel = info.collider.velocity;
                                    if (info.collider.Process(new Vector4(bx, by, bx + tileSize, by + tileSize), out Vec2 normal, out float push, ref vel) && push > rpush)
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

        public bool Process(Vector4 other, out Vec2 normal, out float pushRange, ref Vec2 velocity)
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
    }
}