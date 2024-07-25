using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Tendeos.Utils.Graphics
{
    /// <summary>
    /// Represents a structure that allows manipulating the pixels of a texture.
    /// It provides methods to draw textures on the texture and update the texture's data.
    /// </summary>
    public readonly struct TextureBuilder
    {
        private readonly Color[] data;
        private readonly int width;
        public readonly Texture2D texture;

        /// <summary>
        /// Gets or sets the color of a pixel at the specified coordinates.
        /// </summary>
        /// <param name="x">The x-coordinate of the pixel.</param>
        /// <param name="y">The y-coordinate of the pixel.</param>
        /// <returns>The color of the pixel.</returns>
        public Color this[int x, int y]
        {
            get => data[x + y * width];
            set => data[x + y * width] = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextureBuilder"/> struct.
        /// </summary>
        /// <param name="texture">The texture to build on.</param>
        public TextureBuilder(Texture2D texture)
        {
            width = texture.Width;
            data = new Color[width * texture.Height];
            texture.GetData(data);
            this.texture = texture;
        }

        /// <summary>
        /// Draws a texture on the current texture at the specified position.
        /// </summary>
        /// <param name="x">The x-coordinate of the top-left corner of the texture.</param>
        /// <param name="y">The y-coordinate of the top-left corner of the texture.</param>
        /// <param name="texture">The texture to draw.</param>
        public void Draw(int x, int y, Texture2D texture)
        {
            int width = texture.Width;
            int height = texture.Height;

            Color[] data = new Color[width * height];
            texture.GetData(data);

            int i;
            for (int j = 0; j < width; j++)
            for (i = 0; i < height; i++)
                this[x + j, y + i] = data[j + i * width];
        }

        public void Draw(int x, int y, int width, int height, byte[] data)
        {
            int i, l;
            for (int j = 0; j < width; j++)
            for (i = 0; i < height; i++)
            {
                l = (j + i * width) * 4;
                this[x + j, y + i] = new Color(data[l], data[l + 1], data[l + 2], data[l + 3]);
            }
        }


        /// <summary>
        /// Updates the texture data with the changes made to the <see cref="TextureBuilder"/>.
        /// </summary>
        /// <remarks>
        /// This method is used to apply the changes made to the texture's pixels.
        /// After calling this method, the changes will be visible when the texture is used in the game.
        /// </remarks>
        public void Update() => texture.SetData(data);
    }
}