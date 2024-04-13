namespace Tendeos.Utils
{
    public struct FRectangle
    {
        private static readonly FRectangle zeroRect = new FRectangle(0f, 0f, 0f, 0f);

        public static FRectangle Zero => zeroRect;

        public Vec2 Location { get; set; }
        public Vec2 Size { get; set; }

        public float Left => Location.X;
        public float Right => Location.X + Size.X;

        public float Top => Location.Y + Size.Y;
        public float Bottom => Location.Y;

        public float X => Location.X;
        public float Y => Location.Y;

        public float Width => Size.X;
        public float Height => Size.Y;

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
            Location = location;
            Size = size;
        }

        public FRectangle(float x, float y, float width, float height)
        {
            Location = new Vec2(x, y);
            Size = new Vec2(width, height);
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

        public void Translate(Vec2 location) => Location = location;

        public static FRectangle operator +(FRectangle left, FRectangle right) => new FRectangle(left.Location + right.Location, left.Size);
    }
}
