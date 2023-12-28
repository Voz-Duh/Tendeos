using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace XnaGame.Utils.Graphics
{
    public struct Sprite
    {
        public Texture2D Texture { get; init; }

        public int Width => Texture.Width;
        public int Height => Texture.Height;

        public Rectangle Rect { get; init; }

        public Sprite(Sprite sprite, Rectangle rect)
        {
            Texture = sprite.Texture;
            Rect = new Rectangle(sprite.Rect.Location + rect.Location, rect.Size);
        }

        public Sprite(Texture2D texture)
        {
            Texture = texture;
            Rect = new Rectangle(0, 0, texture.Width, texture.Height);
        }

        public Sprite(Texture2D texture, Rectangle rect)
        {
            Texture = texture;
            Rect = rect;
        }

        public Sprite[] Split(int columns, int rows, int padding = 0, int ignore = 0)
        {
            int subTextureWidth = (Rect.Width - (columns - 1) * padding) / columns;
            int subTextureHeight = (Rect.Height - (rows - 1) * padding) / rows;

            Sprite[] subTextures = new Sprite[rows * columns - ignore];

            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < columns; col++)
                {
                    if (row * columns + col >= rows * columns - ignore) break;

                    int x = col * (subTextureWidth + padding),
                        y = row * (subTextureHeight + padding);
                    Rectangle subTextureRect = new Rectangle(Rect.X + x, Rect.Y + y, subTextureWidth, subTextureHeight);

                    subTextures[row * columns + col] = new Sprite(Texture, subTextureRect);
                }
            }

            return subTextures;
        }

        public void SplitX(int x, out Sprite a, out Sprite b)
        {
            a = new Sprite(this, new Rectangle(Rect.X, Rect.Y, Rect.Width, x));
            b = new Sprite(this, new Rectangle(Rect.X, x, Rect.Width, Rect.Height - x));
        }

        public void SplitY(int y, out Sprite a, out Sprite b)
        {
            a = new Sprite(this, new Rectangle(Rect.X, Rect.Y, Rect.Width, y));
            b = new Sprite(this, new Rectangle(Rect.X, y, Rect.Width, Rect.Height - y));
        }

        public static Sprite Load(ContentManager content, string path) => new Sprite(content.Load<Texture2D>(path));

        public static implicit operator Texture2D(Sprite sprite) => sprite.Texture;
    }
}
