using System;
using System.Runtime.CompilerServices;
using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tendeos.Utils.Graphics.FontStash;

namespace Tendeos.Utils.Graphics
{
    public class SpriteBatch
    {
        public struct Vertex2AtlasDraw : IVertexType
        {
            public readonly Vector4 Position;
            public readonly Color Color;
            public readonly Vector4 ScaleXY_OffsetZW;
            public readonly Vector2 TextureCoordinates;
            public readonly Vector4 WorldPositionXY_SinCosZW;

            private static readonly VertexDeclaration vertexDeclaration = new VertexDeclaration
            (
                new VertexElement(0, VertexElementFormat.Vector4, VertexElementUsage.Position, 0),
                new VertexElement(4 * 4, VertexElementFormat.Color, VertexElementUsage.Color, 0),
                new VertexElement(4 * 4 + 4, VertexElementFormat.Vector4, VertexElementUsage.Normal, 0),
                new VertexElement(4 * 4 + 4 + 4 * 4, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate,
                    0),
                new VertexElement(4 * 4 + 4 + 4 * 4 + 4 * 2, VertexElementFormat.Vector4,
                    VertexElementUsage.TextureCoordinate, 1)
            );

            public readonly VertexDeclaration VertexDeclaration => vertexDeclaration;

            public Vertex2AtlasDraw(Vector2 position, Color color, Vector2 textureCoordinates, Vector2 worldPosition,
                Vector2 scale, Vector2 origin, float sine, float cosine)
            {
                Position = new Vector4(position, 0, 0);
                Color = color;
                ScaleXY_OffsetZW = new Vector4(scale, origin.X, origin.Y);
                WorldPositionXY_SinCosZW = new Vector4(worldPosition.X, worldPosition.Y, sine, cosine);
                TextureCoordinates = textureCoordinates;
            }
        }

        private readonly RasterizerState RasterizerState;

        private Renderer renderer;

        private GraphicsDevice GraphicsDevice;

        private VertexBuffer vertexBuffer;

        private readonly static Matrix world = Matrix.CreateTranslation(0, 0, 0);

        private readonly static Matrix view =
            Matrix.CreateLookAt(new Vector3(0, 0, 10), new Vector3(0, 0, 0), new Vector3(0, 1, 0));

        private Vertex2AtlasDraw[] vertices;
        private int primitives;

        public Color Color { get; set; } = Color.White;
        public bool FlipX { get; set; } = false;
        public bool FlipY { get; set; } = false;

        public bool Batching { get; private set; }

        private Shader shader;
        private float textureWidth, textureHeight, lastSine, lastCosine, lastAngle = float.PositiveInfinity;

        public SpriteBatch(GraphicsDevice GraphicsDevice, Assets.Atlas atlas)
        {
            RasterizerState = new RasterizerState()
            {
                CullMode = CullMode.None
            };
            this.GraphicsDevice = GraphicsDevice;
            renderer = new Renderer(this, atlas);
        }

        public void Begin(Shader shader, Texture2D texture, int size = 4086, Matrix? matrix = null)
        {
            if (Batching) throw new NotSupportedException("You cannot Begin batch while other batch not ended.");
            Batching = true;
            primitives = 0;
            shader.Parameters["World"].SetValue(matrix ?? world);
            shader.Parameters["View"].SetValue(view);
            shader.Parameters["Projection"].SetValue(Matrix.CreateOrthographicOffCenter(
                GraphicsDevice.Viewport.X, GraphicsDevice.Viewport.X + GraphicsDevice.Viewport.Width,
                GraphicsDevice.Viewport.Y + GraphicsDevice.Viewport.Height, GraphicsDevice.Viewport.Y,
                0.01f, 1000));
            shader.Parameters["SpriteTexture"].SetValue(texture);
            textureWidth = texture.Width;
            textureHeight = texture.Height;
            this.shader = shader;

            if (vertices == null || vertices.Length != size * 6) vertices = new Vertex2AtlasDraw[size * 6];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Rect(Sprite sprite, FRectangle rectangle) =>
            Rect(Color.White, sprite, rectangle);

        public void Rect(Color color, Sprite sprite, FRectangle rectangle)
        {
            float width = rectangle.Width;
            float height = rectangle.Height;

            Vector4 rect = new Vector4(sprite.Rect.X / textureWidth, sprite.Rect.Y / textureHeight,
                sprite.Rect.Width / textureWidth, sprite.Rect.Height / textureHeight);
            Color useColor = new Color(color.ToVector4() * Color.ToVector4());


            vertices[primitives++] = new Vertex2AtlasDraw(
                new Vector2(0), useColor,
                new Vector2(rect.X, rect.Y),
                rectangle.Location, new Vector2(1), new Vector2(0), 0, 1);
            vertices[primitives++] = new Vertex2AtlasDraw(
                new Vector2(0, height), useColor,
                new Vector2(rect.X, rect.Y + rect.W),
                rectangle.Location, new Vector2(1), new Vector2(0), 0, 1);
            vertices[primitives++] = new Vertex2AtlasDraw(
                new Vector2(width, 0), useColor,
                new Vector2(rect.X + rect.Z, rect.Y),
                rectangle.Location, new Vector2(1), new Vector2(0), 0, 1);


            vertices[primitives++] = new Vertex2AtlasDraw(
                new Vector2(width, 0), useColor,
                new Vector2(rect.X + rect.Z, rect.Y),
                rectangle.Location, new Vector2(1), new Vector2(0), 0, 1);
            vertices[primitives++] = new Vertex2AtlasDraw(
                new Vector2(0, height), useColor,
                new Vector2(rect.X, rect.Y + rect.W),
                rectangle.Location, new Vector2(1), new Vector2(0), 0, 1);
            vertices[primitives++] = new Vertex2AtlasDraw(
                new Vector2(width, height), useColor,
                new Vector2(rect.X + rect.Z, rect.Y + rect.W),
                rectangle.Location, new Vector2(1), new Vector2(0), 0, 1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Rect(Sprite sprite, Vec2 position, float scale = 1, float rotation = 0, float xOrigin = 0.5f,
            float yOrigin = 0.5f, bool flipX = false, bool flipY = false) =>
            Rect(Color.White, sprite, position, new Vec2(scale), rotation, xOrigin, yOrigin, flipX, flipY);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Rect(Color color, Sprite sprite, Vec2 position, float scale = 1, float rotation = 0,
            float xOrigin = 0.5f, float yOrigin = 0.5f, bool flipX = false, bool flipY = false) =>
            Rect(color, sprite, position, new Vec2(scale), rotation, xOrigin, yOrigin, flipX, flipY);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Rect(Sprite sprite, Vec2 position, Vec2 scale, float rotation = 0, float xOrigin = 0.5f,
            float yOrigin = 0.5f, bool flipX = false, bool flipY = false) =>
            Rect(Color.White, sprite, position, scale, rotation, xOrigin, yOrigin, flipX, flipY);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Rect(Color color, Sprite sprite, Vec2 position, Vec2 scale, float rotation = 0,
            float xOrigin = 0.5f, float yOrigin = 0.5f, bool flipX = false, bool flipY = false) =>
            Rect(color, sprite.Rect, position, scale, rotation, xOrigin, yOrigin, flipX, flipY);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Rect(Rectangle area, Vec2 position, float scale = 1, float rotation = 0, float xOrigin = 0.5f,
            float yOrigin = 0.5f, bool flipX = false, bool flipY = false) =>
            Rect(Color.White, area, position, new Vec2(scale), rotation, xOrigin, yOrigin, flipX, flipY);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Rect(Color color, Rectangle area, Vec2 position, float scale = 1, float rotation = 0,
            float xOrigin = 0.5f, float yOrigin = 0.5f, bool flipX = false, bool flipY = false) =>
            Rect(color, area, position, new Vec2(scale), rotation, xOrigin, yOrigin, flipX, flipY);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Rect(Rectangle area, Vec2 position, Vec2 scale, float rotation = 0, float xOrigin = 0.5f,
            float yOrigin = 0.5f, bool flipX = false, bool flipY = false) =>
            Rect(Color.White, area, position, scale, rotation, xOrigin, yOrigin, flipX, flipY);

        public void Rect(Color color, Rectangle area, Vec2 position, Vec2 scale, float rotation = 0,
            float xOrigin = 0.5f, float yOrigin = 0.5f, bool flipX = false, bool flipY = false)
        {
            if (rotation != lastAngle)
            {
                lastAngle = rotation;
                rotation = MathHelper.ToRadians(rotation);
                (lastSine, lastCosine) = MathF.SinCos(rotation);
            }

            Vector4 draw = new Vector4(0, 0, area.Width, area.Height);
            Vector4 rect = new Vector4(area.X / textureWidth, area.Y / textureHeight, area.Width / textureWidth,
                area.Height / textureHeight);
            if (flipX != FlipX) (draw.X, draw.Z) = (draw.Z, draw.X);
            if (flipY != FlipY) (draw.Y, draw.W) = (draw.W, draw.Y);
            Color useColor = new Color(color.ToVector4() * Color.ToVector4());

            Vector2 origin = new Vector2(xOrigin * -area.Width, yOrigin * -area.Height);

            vertices[primitives++] = new Vertex2AtlasDraw(
                new Vector2(draw.X, draw.Y), useColor,
                new Vector2(rect.X, rect.Y),
                position, scale, origin, lastSine, lastCosine);
            vertices[primitives++] = new Vertex2AtlasDraw(
                new Vector2(draw.X, draw.W), useColor,
                new Vector2(rect.X, rect.Y + rect.W),
                position, scale, origin, lastSine, lastCosine);
            vertices[primitives++] = new Vertex2AtlasDraw(
                new Vector2(draw.Z, draw.Y), useColor,
                new Vector2(rect.X + rect.Z, rect.Y),
                position, scale, origin, lastSine, lastCosine);


            vertices[primitives++] = new Vertex2AtlasDraw(
                new Vector2(draw.Z, draw.Y), useColor,
                new Vector2(rect.X + rect.Z, rect.Y),
                position, scale, origin, lastSine, lastCosine);
            vertices[primitives++] = new Vertex2AtlasDraw(
                new Vector2(draw.X, draw.W), useColor,
                new Vector2(rect.X, rect.Y + rect.W),
                position, scale, origin, lastSine, lastCosine);
            vertices[primitives++] = new Vertex2AtlasDraw(
                new Vector2(draw.Z, draw.W), useColor,
                new Vector2(rect.X + rect.Z, rect.Y + rect.W),
                position, scale, origin, lastSine, lastCosine);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Rect(Sprite sprite, Rectangle source, Vec2 position, float scale = 1, float rotation = 0,
            float xOrigin = 0.5f, float yOrigin = 0.5f, bool flipX = false, bool flipY = false) =>
            Rect(Color.White, sprite, source, position, new Vec2(scale), rotation, xOrigin, yOrigin, flipX, flipY);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Rect(Color color, Sprite sprite, Rectangle source, Vec2 position, float scale = 1,
            float rotation = 0, float xOrigin = 0.5f, float yOrigin = 0.5f, bool flipX = false, bool flipY = false) =>
            Rect(color, sprite, source, position, new Vec2(scale), rotation, xOrigin, yOrigin, flipX, flipY);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Rect(Sprite sprite, Rectangle source, Vec2 position, Vec2 scale, float rotation = 0,
            float xOrigin = 0.5f, float yOrigin = 0.5f, bool flipX = false, bool flipY = false) =>
            Rect(Color.White, sprite, source, position, scale, rotation, xOrigin, yOrigin, flipX, flipY);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Rect(Color color, Sprite sprite, Rectangle source, Vec2 position, Vec2 scale, float rotation = 0,
            float xOrigin = 0.5f, float yOrigin = 0.5f, bool flipX = false, bool flipY = false) =>
            Rect(color, sprite.Rect, source, position, scale, rotation, xOrigin, yOrigin, flipX, flipY);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Rect(Rectangle area, Rectangle source, Vec2 position, float scale = 1, float rotation = 0,
            float xOrigin = 0.5f, float yOrigin = 0.5f, bool flipX = false, bool flipY = false) =>
            Rect(Color.White, area, source, position, new Vec2(scale), rotation, xOrigin, yOrigin, flipX, flipY);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Rect(Color color, Rectangle area, Rectangle source, Vec2 position, float scale = 1,
            float rotation = 0, float xOrigin = 0.5f, float yOrigin = 0.5f, bool flipX = false, bool flipY = false) =>
            Rect(color, area, source, position, new Vec2(scale), rotation, xOrigin, yOrigin, flipX, flipY);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Rect(Rectangle area, Rectangle source, Vec2 position, Vec2 scale, float rotation = 0,
            float xOrigin = 0.5f, float yOrigin = 0.5f, bool flipX = false, bool flipY = false) =>
            Rect(Color.White, area, source, position, scale, rotation, xOrigin, yOrigin, flipX, flipY);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Rect(Color color, Rectangle area, Rectangle source, Vec2 position, Vec2 scale, float rotation = 0,
            float xOrigin = 0.5f, float yOrigin = 0.5f, bool flipX = false, bool flipY = false) =>
            Rect(color, new Rectangle(area.X + source.X, area.Y + source.Y, source.Width, source.Height), position,
                scale, rotation, xOrigin, yOrigin, flipX, flipY);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Text(Font font, string text, Vec2 position, float scale = 1, float rotation = 0,
            float xOrigin = 0.5f, float yOrigin = 0.5f, FontSystemEffect effect = FontSystemEffect.None,
            int effectAmount = 0) =>
            Text(Color.White, font, text, position, new Vec2(scale), rotation, xOrigin, yOrigin, effect, effectAmount);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Text(Font font, string text, Vec2 position, Vec2 scale, float rotation = 0, float xOrigin = 0.5f,
            float yOrigin = 0.5f, FontSystemEffect effect = FontSystemEffect.None, int effectAmount = 0) =>
            Text(Color.White, font, text, position, scale, rotation, xOrigin, yOrigin, effect, effectAmount);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Text(Color color, Font font, string text, Vec2 position, float scale = 1, float rotation = 0,
            float xOrigin = 0.5f, float yOrigin = 0.5f, FontSystemEffect effect = FontSystemEffect.None,
            int effectAmount = 0) =>
            Text(color, font, text, position, new Vec2(scale), rotation, xOrigin, yOrigin, effect, effectAmount);

        public void Text(Color color, Font font, string text, Vec2 position, Vec2 scale, float rotation = 0,
            float xOrigin = 0.5f, float yOrigin = 0.5f, FontSystemEffect effect = FontSystemEffect.None,
            int effectAmount = 0)
        {
            if (rotation == lastAngle)
            {
                lastAngle = rotation;
                rotation = MathHelper.ToRadians(rotation);
                lastCosine = MathF.Cos(rotation);
                lastSine = MathF.Sin(rotation);
            }

            Vec2 offset = font.MeasureString(text, scale) * new Vec2(xOrigin, yOrigin);
            offset = new Vec2(
                offset.X * lastCosine + offset.Y * -lastSine,
                offset.X * lastSine + offset.Y * lastCosine);
            font.Dynamic.DrawText(renderer, text, position - offset, new FSColor(color.R, color.G, color.B, color.A),
                rotation, Vec2.Zero, scale * font.Scale, effect: effect, effectAmount: effectAmount);
        }

        public void End()
        {
            if (!Batching) throw new NotSupportedException("Trying to End not begined batch.");
            Batching = false;

            if (primitives == 0) return;

            if (vertexBuffer == null || vertexBuffer.VertexCount != vertices.Length)
            {
                vertexBuffer?.Dispose();

                vertexBuffer = new VertexBuffer(GraphicsDevice, typeof(Vertex2AtlasDraw), vertices.Length,
                    BufferUsage.WriteOnly);
            }

            vertexBuffer.SetData(vertices);

            GraphicsDevice.SetVertexBuffer(vertexBuffer);

            GraphicsDevice.RasterizerState = RasterizerState;
            GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
            GraphicsDevice.BlendState = BlendState.AlphaBlend;

            foreach (EffectPass pass in shader.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.DrawPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, 0,
                    primitives / 3);
            }
        }
    }
}