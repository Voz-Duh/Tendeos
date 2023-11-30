using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;

namespace XnaGame.Utils.Graphics
{
    public class Batch
    {
        GraphicsDevice GraphicsDevice;

        VertexBuffer vertexBuffer;

        BasicEffect basicEffect;
        readonly Matrix world = Matrix.CreateTranslation(0, 0, 0);
        readonly Matrix view = Matrix.CreateLookAt(new Vector3(0, 0, 10), new Vector3(0, 0, 0), new Vector3(0, 1, 0));

        VertexPositionColorNormalTexture[] vertices;
        int primitives;

        PrimitiveType primitiveType;
        public Color Color { private get; set; } = Color.White;
        public FVector2 UV { private get; set; } = FVector2.Zero;
        public Vector3 Normal { private get; set; } = Vector3.Zero;

        public bool Batching { get; private set; }

        public Batch(GraphicsDevice GraphicsDevice)
        {
            this.GraphicsDevice = GraphicsDevice;
            basicEffect = new BasicEffect(GraphicsDevice);
        }

        public void Begin(PrimitiveType primitiveType, int size = 4086, Matrix? projection = null)
        {
            if (Batching) throw new NotSupportedException("You cannot Begin batch while other batch not ended.");
            Batching = true;
            primitives = 0;
            this.primitiveType = primitiveType;
            basicEffect.World = world;
            basicEffect.View = view;
            basicEffect.Projection = projection ?? Matrix.CreateOrthographicOffCenter(
                GraphicsDevice.Viewport.X, GraphicsDevice.Viewport.X + GraphicsDevice.Viewport.Width,
                GraphicsDevice.Viewport.Y + GraphicsDevice.Viewport.Height, GraphicsDevice.Viewport.Y,
                0.01f, 1000);
            basicEffect.VertexColorEnabled = true;

            vertices = new VertexPositionColorNormalTexture[size];
        }

        public void Vertex(Vector3 vector) => Vertex3(vector.X, vector.Y, vector.Z);

        public void Vertex(FVector2 vector) => Vertex3(vector.X, vector.Y, 0);

        public void Vertex3(float x, float y, float z)
        {
            vertices[primitives] = new VertexPositionColorNormalTexture(new Vector3(x, y, z), Color, Normal, UV);
            primitives++;
        }

        public void End()
        {
            if (!Batching) throw new NotSupportedException("Trying to End not begined batch.");
            Batching = false;

            vertexBuffer?.Dispose();

            if (primitives == 0) return;

            vertexBuffer = new VertexBuffer(GraphicsDevice, typeof(VertexPositionColorNormalTexture), vertices.Length, BufferUsage.WriteOnly);
            vertexBuffer.SetData(vertices);

            GraphicsDevice.SetVertexBuffer(vertexBuffer);

            RasterizerState rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            GraphicsDevice.RasterizerState = rasterizerState;

            foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.DrawPrimitives(primitiveType, 0,
                    primitiveType switch
                    {
                        PrimitiveType.TriangleList => primitives / 3,
                        PrimitiveType.PointList => primitives,
                        PrimitiveType.LineList => primitives / 2,
                    });
            }
        }
    }
}
