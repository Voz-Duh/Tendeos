using nkast.Aether.Physics2D.Collision.Shapes;
using nkast.Aether.Physics2D.Common;
using nkast.Aether.Physics2D.Dynamics;

namespace XnaGame.Utils
{
    public static class BodyHelpers
    {
        public static Fixture CreateSmoothRectangle(this Body body, float smooth, float width, float height, float density, Vector2 offset)
        {
            width /= 2;
            height /= 2;
            Vertices vertices = new Vertices(8)
            {
                new Vector2(-width, -height+smooth),
                new Vector2(-width+smooth, -height),
                new Vector2(width-smooth, -height),
                new Vector2(width, -height+smooth),
                new Vector2(width, height-smooth),
                new Vector2(width-smooth, height),
                new Vector2(-width+smooth, height),
                new Vector2(-width, height-smooth)
            };
            vertices.Translate(ref offset);
            PolygonShape shape = new PolygonShape(vertices, density);
            return body.CreateFixture(shape);
        }
    }
}
