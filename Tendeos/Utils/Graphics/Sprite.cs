﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;

namespace Tendeos.Utils.Graphics
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

        public Color[] Data
        {
            get
            {
                Color[] data = new Color[Rect.Height * Rect.Width];
                Texture.GetData(0, Rect, data, 0, data.Length);
                return data;
            }
        }

        public Sprite[] Split(int rows, int columns, int padding = 0, int ignore = 0)
        {
            int subTextureWidth = (Rect.Width - (rows - 1) * padding) / rows;
            int subTextureHeight = (Rect.Height - (columns - 1) * padding) / columns;

            Sprite[] subTextures = new Sprite[columns * rows - ignore];

            for (int col = 0; col < columns; col++)
            {
                for (int row = 0; row < rows; row++)
                {
                    if (col * rows + row >= columns * rows - ignore) break;

                    int x = row * (subTextureWidth + padding),
                        y = col * (subTextureHeight + padding);
                    Rectangle subTextureRect = new(Rect.X + x, Rect.Y + y, subTextureWidth, subTextureHeight);

                    subTextures[col * rows + row] = new Sprite(Texture, subTextureRect);
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

        public static implicit operator Texture2D(Sprite sprite) => sprite.Texture;
    }
}