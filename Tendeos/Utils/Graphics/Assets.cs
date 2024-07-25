using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using Tendeos.Modding;
using Microsoft.Xna.Framework;
using System.Text;

namespace Tendeos.Utils.Graphics
{
    public class Assets
    {
        public string AssetsPath { get; }
        private readonly Dictionary<string, Sprite> sprites;
        private readonly Dictionary<string, Shader> shaders;
        private readonly Dictionary<string, MISObject> misObjects;
        public Atlas atlas { get; private set; }
        private GraphicsDevice graphicsDevice;
        private readonly Dictionary<string, Texture2D> textures;
        private readonly Font font;

        public Assets(GraphicsDevice graphicsDevice, Font font, string assetsPath, int atlasWidth, int atlasHeight)
        {
            this.graphicsDevice = graphicsDevice;
            atlas = new Atlas(graphicsDevice, atlasWidth, atlasHeight);
            shaders = new Dictionary<string, Shader>();
            textures = new Dictionary<string, Texture2D>();
            misObjects = new Dictionary<string, MISObject>();
            sprites = new Dictionary<string, Sprite>();
            AssetsPath = assetsPath;
            this.font = font;
            AddFrom(AssetsPath);
        }

        public void AddFrom(string path)
        {
            if (!Directory.Exists(path)) return;
            GetAssetsFromDirectory(path, path.Length + 1);
            foreach (var (key, value) in textures.OrderByDescending(entry =>
                         Math.Min(entry.Value.Width, entry.Value.Height)))
            {
                sprites.Add(key, atlas.AddTexture(value, key, 1));
            }

            textures.Clear();
        }

        private void GetAssetsFromDirectory(string directory, int mainDirectoryPathLength)
        {
            foreach (string file in Directory.GetFiles(directory))
            {
                string name = file[mainDirectoryPathLength..];
                name = ValidPath(name[..name.LastIndexOf('.')]);
                switch (Path.GetExtension(file))
                {
                    case ".png":
                        using (Stream stream = File.OpenRead(file))
                        {
                            Texture2D texture = Texture2D.FromStream(graphicsDevice, stream);
                            textures[name] = texture;
                        }

                        break;
                    case ".shdc":
                        shaders[name] = new Shader(graphicsDevice, File.ReadAllBytes(file));
                        break;
                    case ".ttf":
                        font?.Load(file);
                        break;
                    case ".mis":
                        misObjects[name] = MIS.Generate(file);
                        break;
                    case ".cmis":
                        misObjects[name] = MIS.Decompile(file);
                        break;
                }
            }

            foreach (string dir in Directory.GetDirectories(directory))
            {
                GetAssetsFromDirectory(dir, mainDirectoryPathLength);
            }
        }

        public static string ValidPath(string path) => path.Replace('/', Path.DirectorySeparatorChar)
            .Replace('\\', Path.DirectorySeparatorChar);

        public Sprite GetSprite(string name) => sprites[ValidPath(name)];
        public Shader GetShader(string name) => shaders[ValidPath(name)];
        public MISObject GetMIS(string name) => misObjects[ValidPath(name)];

        public bool HasSprite(string name) => sprites.ContainsKey(ValidPath(name));
        public bool HasShader(string name) => shaders.ContainsKey(ValidPath(name));
        public bool HasMIS(string name) => misObjects.ContainsKey(ValidPath(name));

        public Dictionary<string, Sprite> GetSpriteDictionary() => sprites;
        public Dictionary<string, Shader> GetShaderDictionary() => shaders;
        public Dictionary<string, MISObject> GetMISDictionary() => misObjects;

        public byte[] LoadFileBytes(string filePath)
        {
            string fullPath = Path.Combine(AssetsPath, filePath);
            return File.ReadAllBytes(fullPath);
        }

        public string LoadFileText(string filePath)
        {
            string fullPath = Path.Combine(AssetsPath, filePath);
            return File.ReadAllText(fullPath);
        }

        public void LoadSpriteData(string filePath, Dictionary<string, Sprite> to)
        {
            string fullPath = Path.Combine(AssetsPath, $"{filePath}.sd");
            Sprite sprite = sprites[ValidPath(filePath)];
            
            foreach (var (key, x, y, w, h) in MIS.Generate(fullPath).GetAllParametersAs<int, int, int, int>())
                to[key] = new Sprite(sprite, new Rectangle(x, y, w, h));
        }

        public class Atlas
        {
            private class Node
            {
                public Rectangle rect;
                public Node[] child;

                public Node(int x, int y, int width, int height)
                {
                    rect = new Rectangle(x, y, width, height);
                    child = null;
                }

                public Node Insert(int width, int height, int padding)
                {
                    if (child != null)
                    {
                        Node newNode = child[0].Insert(width, height, padding);

                        if (newNode != null)
                        {
                            return newNode;
                        }

                        newNode = child[1].Insert(width, height, padding);

                        if (newNode != null)
                        {
                            return newNode;
                        }

                        return child[2].Insert(width, height, padding);
                    }
                    else
                    {
                        if (width > rect.Width || height > rect.Height)
                        {
                            return null;
                        }

                        int dw = rect.Width - width;
                        int dh = rect.Height - height;

                        child = new Node[3]
                        {
                            new Node(rect.X + width, rect.Y, dw, height),
                            new Node(rect.X, rect.Y + height, width, dh),
                            new Node(rect.X + width, rect.Y + height, dw, dh)
                        };

                        return new Node(rect.X, rect.Y, width, height);
                    }
                }
            }

            private TextureBuilder textureBuilder;
            public readonly Texture2D texture;
            private Node root;
            private readonly Dictionary<string, Rectangle> rectangleMap;
            private uint allocations;

            public Atlas(GraphicsDevice graphics, int width, int height)
            {
                texture = new Texture2D(graphics, width, height);
                textureBuilder = new TextureBuilder(texture);

                root = new Node(0, 0, width, height);
                rectangleMap = new Dictionary<string, Rectangle>();
            }

            public Sprite AddTexture(Texture2D texture, string name, int padding)
            {
                int width = texture.Width;
                int height = texture.Height;
                Node node = root.Insert(width, height, padding) ?? throw new IndexOutOfRangeException("Atlas is full");

                rectangleMap.Add(name, node.rect);
                textureBuilder.Draw(node.rect.X, node.rect.Y, texture);
                textureBuilder.Update();

                return new Sprite(this.texture, node.rect);
            }

            public Rectangle Allocate(int width, int height, int padding)
            {
                Node node = root.Insert(width, height, padding) ?? throw new IndexOutOfRangeException("Atlas is full");

                rectangleMap.Add($"ALLOCATED_{allocations++}", node.rect);

                return node.rect;
            }

            public void DrawInAllocated(Rectangle rect, Rectangle bounds, byte[] data)
            {
                textureBuilder.Draw(rect.X + bounds.X, rect.Y + bounds.Y, bounds.Width, bounds.Height, data);
                textureBuilder.Update();
            }

            public void End()
            {
                root = null;
                textureBuilder.Update();
                GC.Collect();
            }

            public MISObject ToMISObject()
            {
                StringBuilder atlas = new StringBuilder();

                foreach (KeyValuePair<string, Rectangle> e in rectangleMap.ToArray())
                {
                    atlas.Append(e.Key).Append(':')
                        .Append(e.Value.X).Append(',')
                        .Append(e.Value.Y).Append(',')
                        .Append(e.Value.Width).Append(',')
                        .Append(e.Value.Height).Append(';');
                }

                return MIS.GenerateVirtual(atlas.ToString(), "atlas");
            }
        }
    }
}