using XMatrix = Microsoft.Xna.Framework.Matrix;
using XnaGame.Utils;

namespace XnaGame.Entities
{
    public class Transform : ITransform
    {
        public bool flipX;
        public float rotation;
        public FVector2 position;
        public float worldRotation => Local2World(0);
        public FVector2 worldPosition => Local2World(FVector2.Zero);

        public Transform parent;

        public Transform(FVector2 position, float rotation)
        {
            this.position = position;
            this.rotation = rotation;
        }

        public Transform(Transform parent, FVector2 position, float rotation)
        {
            this.parent = parent;
            this.position = position;
            this.rotation = rotation;
        }

        public Transform(FVector2 position, float rotation, bool flipX)
        {
            this.position = position;
            this.rotation = rotation;
            this.flipX = flipX;
        }

        public Transform(Transform parent, FVector2 position, float rotation, bool flipX)
        {
            this.parent = parent;
            this.position = position;
            this.rotation = rotation;
            this.flipX = flipX;
        }

        public float Local2World(float degrees) => parent?.Local2World(degrees + rotation) ?? (degrees + rotation);
        public float World2Local(float degrees) => parent?.World2Local(degrees - rotation) ?? (degrees - rotation);

        public FVector2 Local2World(FVector2 point) => FVector2.Transform(point, Matrix);
        public FVector2 World2Local(FVector2 point) => FVector2.Transform(point, XMatrix.Invert(Matrix));

        private XMatrix Matrix => (parent?.Matrix ?? XMatrix.Identity) * (XMatrix.CreateTranslation(flipX ? -position.X : position.X, position.Y, 0) * XMatrix.CreateRotationZ(rotation) * XMatrix.CreateScale(1));
    }
}
