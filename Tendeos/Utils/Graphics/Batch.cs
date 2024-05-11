using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Tendeos.Utils.Graphics
{
    public class Batch
    {
        private GraphicsDevice GraphicsDevice;

        private VertexBuffer vertexBuffer;

        private BasicEffect basicEffect;
        private readonly static Matrix world = Matrix.CreateTranslation(0, 0, 0);
        private readonly static Matrix view = Matrix.CreateLookAt(new Vector3(0, 0, 10), new Vector3(0, 0, 0), new Vector3(0, 1, 0));

        private VertexPositionColorNormalTexture[] vertices;
        private int primitives;
        private readonly RasterizerState rasterizerState = new RasterizerState()
        {
            CullMode = CullMode.None
        };

        private PrimitiveType primitiveType;
        public Color Color { private get; set; } = Color.White;
        public Vec2 UV { private get; set; } = Vec2.Zero;
        public Vector3 Normal { private get; set; } = Vector3.Zero;

        public BlendState BlendState;

        public bool Batching { get; private set; }

        public Batch(GraphicsDevice GraphicsDevice)
        {
            this.GraphicsDevice = GraphicsDevice;
            basicEffect = new BasicEffect(GraphicsDevice);
        }

        public void Begin(PrimitiveType primitiveType, int size = 4086, Matrix? matrix = null)
        {
            if (Batching) throw new NotSupportedException("You cannot Begin batch while other batch not ended.");
            Batching = true;
            primitives = 0;
            this.primitiveType = primitiveType;
            basicEffect.World = matrix ?? world;
            basicEffect.View = view;
            basicEffect.Projection = Matrix.CreateOrthographicOffCenter(
                GraphicsDevice.Viewport.X, GraphicsDevice.Viewport.X + GraphicsDevice.Viewport.Width,
                GraphicsDevice.Viewport.Y + GraphicsDevice.Viewport.Height, GraphicsDevice.Viewport.Y,
                0.01f, 1000);
            basicEffect.VertexColorEnabled = true;
            BlendState = BlendState.AlphaBlend;

            if (vertices == null || vertices.Length != size) vertices = new VertexPositionColorNormalTexture[size];
        }

        public void Vertex(Vector3 vector) => Vertex3(vector.X, vector.Y, vector.Z);

        public void Vertex(Vec2 vector) => Vertex3(vector.X, vector.Y, 0);

        public void Vertex3(float x, float y, float z)
        {
            vertices[primitives] = new VertexPositionColorNormalTexture(new Vector3(x, y, z), Color, Normal, UV);
            primitives++;
        }

        public void DrawCircle(float x, float y, float radius, int details = 15)
        {
            float rad = 0;
            float step = 1f / details * MathF.PI * 2;
            for (int i = 0; i <= details; i++)
            {
                rad += step;
                Vertex3(x, y, 0);
                Vertex3(x + MathF.Sin(rad) * radius, y + MathF.Cos(rad) * radius, 0);
            }
        }

        public void End()
        {
            if (!Batching) throw new NotSupportedException("Trying to End not begined batch.");
            Batching = false;

            if (primitives == 0) return;

            if (vertexBuffer == null || vertexBuffer.VertexCount != vertices.Length)
            {
                vertexBuffer?.Dispose();

                vertexBuffer = new VertexBuffer(GraphicsDevice, typeof(VertexPositionColorNormalTexture), vertices.Length, BufferUsage.WriteOnly);
            }
            vertexBuffer.SetData(vertices);

            GraphicsDevice.SetVertexBuffer(vertexBuffer);

            GraphicsDevice.RasterizerState = rasterizerState;
            if (BlendState != null) GraphicsDevice.BlendState = BlendState;

            foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.DrawPrimitives(primitiveType, 0,
                    primitiveType switch
                    {
                        PrimitiveType.TriangleList => primitives / 3,
                        PrimitiveType.PointList => primitives,
                        PrimitiveType.LineList => primitives / 2,
                        PrimitiveType.TriangleStrip => primitives - 2,
                        PrimitiveType.LineStrip => primitives - 1,
                    });
            }
        }

        public (VertexBuffer, T[]) CreateBuffer<T>(int vertexCount) where T : struct =>
            (new VertexBuffer(GraphicsDevice, typeof(T), vertexCount, BufferUsage.WriteOnly), new T[vertexCount]);

        public (VertexBuffer, T[]) UpdateBuffer<T>(VertexBuffer vertexBuffer, T[] vertexes, int vertexCount) =>
            vertexBuffer.VertexCount == vertexCount ? (vertexBuffer, vertexes) : (new VertexBuffer(GraphicsDevice, vertexBuffer.VertexDeclaration, vertexCount, BufferUsage.WriteOnly), new T[vertexCount]);

        public void Draw(VertexBuffer vertexBuffer, PrimitiveType primitiveType, int validVertexes, Matrix? matrix = null)
        {
            if (validVertexes == 0) return;

            basicEffect.World = matrix ?? world;
            basicEffect.View = view;
            basicEffect.Projection = Matrix.CreateOrthographicOffCenter(
                GraphicsDevice.Viewport.X, GraphicsDevice.Viewport.X + GraphicsDevice.Viewport.Width,
                GraphicsDevice.Viewport.Y + GraphicsDevice.Viewport.Height, GraphicsDevice.Viewport.Y,
                0.01f, 1000);
            basicEffect.VertexColorEnabled = true;

            GraphicsDevice.SetVertexBuffer(vertexBuffer);

            GraphicsDevice.RasterizerState = rasterizerState;
            if (BlendState != null) GraphicsDevice.BlendState = BlendState;
            else GraphicsDevice.BlendState = BlendState.AlphaBlend;

            foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.DrawPrimitives(primitiveType, 0,
                    primitiveType switch
                    {
                        PrimitiveType.TriangleList => validVertexes / 3,
                        PrimitiveType.PointList => validVertexes,
                        PrimitiveType.LineList => validVertexes / 2,
                        PrimitiveType.TriangleStrip => validVertexes - 2,
                        PrimitiveType.LineStrip => validVertexes - 1,
                    });
            }
        }
    }
}
