using XMatrix = Microsoft.Xna.Framework.Matrix;
using XnaGame.Utils;

namespace XnaGame.PEntities
{
    public class Transform : ITransform
    {
        public bool flipX;
        public float rotation;
        public Vec2 position;
        public float worldRotation => Local2World(0);
        public Vec2 worldPosition => Local2World(Vec2.Zero);

        public Transform parent;

        public Transform(Vec2 position, float rotation)
        {
            this.position = position;
            this.rotation = rotation;
        }

        public Transform(Transform parent, Vec2 position, float rotation)
        {
            this.parent = parent;
            this.position = position;
            this.rotation = rotation;
        }

        public Transform(Vec2 position, float rotation, bool flipX)
        {
            this.position = position;
            this.rotation = rotation;
            this.flipX = flipX;
        }

        public Transform(Transform parent, Vec2 position, float rotation, bool flipX)
        {
            this.parent = parent;
            this.position = position;
            this.rotation = rotation;
            this.flipX = flipX;
        }

        public float Local2World(float degrees) => parent?.Local2World(degrees + rotation) ?? (degrees + rotation);
        public float World2Local(float degrees) => parent?.World2Local(degrees - rotation) ?? (degrees - rotation);

        public Vec2 Local2World(Vec2 point) => Vec2.Transform(point, Matrix);
        public Vec2 World2Local(Vec2 point) => Vec2.Transform(point, XMatrix.Invert(Matrix));

        private XMatrix Matrix => (parent?.Matrix ?? XMatrix.Identity) * (XMatrix.CreateTranslation(flipX ? -position.X : position.X, position.Y, 0) * XMatrix.CreateRotationZ(rotation) * XMatrix.CreateScale(1));
    }
}
