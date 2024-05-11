using Tendeos.World.Shadows;

namespace Tendeos.Utils
{
    public struct FRectangle
    {
        private static readonly FRectangle zeroRect = new FRectangle(0f, 0f, 0f, 0f);

        public static FRectangle Zero => zeroRect;

        public Vec2 Location
        {
            get => new Vec2(X, Y);
            set => (X, Y) = value;
        }
        public Vec2 Size
        {
            get => new Vec2(Width, Height);
            set => (Width, Height) = value;
        }

        public float Left => Location.X;
        public float Right => Location.X + Size.X;

        public float Top => Location.Y + Size.Y;
        public float Bottom => Location.Y;

        public float X;
        public float Y;

        public float Width;
        public float Height;

        public Vec2 Center => Location + Size / 2;

        public static bool operator ==(FRectangle value1, FRectangle value2)
        {
            if (value1.Location == value2.Location)
            {
                return value1.Size == value2.Size;
            }

            return false;
        }

        public static bool operator !=(FRectangle value1, FRectangle value2)
        {
            if (value1.Location == value2.Location)
            {
                return value1.Size != value2.Size;
            }

            return true;
        }

        public FRectangle(Vec2 location, Vec2 size)
        {
            (X, Y) = location;
            (Width, Height) = size;
        }

        public FRectangle(float x, float y, float width, float height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public bool Contains(Vec2 point) =>
            point.X >= Left &&
            point.Y >= Bottom &&
            point.X <= Right &&
            point.Y <= Top;

        public bool Contains(float x, float y) =>
            x >= Left &&
            y >= Bottom &&
            x <= Right &&
            y <= Top;

        public void Translate(Vec2 location) => (X, Y) = location;

        public static FRectangle operator +(FRectangle left, FRectangle right) => new FRectangle(left.Location + right.Location, left.Size);
    }
}
