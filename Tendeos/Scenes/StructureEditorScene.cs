using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NativeFileDialogSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Tendeos.Content;
using Tendeos.Modding;
using Tendeos.UI.GUIElements;
using Tendeos.Utils;
using Tendeos.Utils.Graphics;
using Tendeos.Utils.Input;
using Tendeos.Utils.SaveSystem;
using Tendeos.World;
using Tendeos.World.Content;

namespace Tendeos.Scenes
{
    public class StructureEditorScene : Scene, IMap
    {
        private static TileData NullTileReference = new TileData();
        private Batch batch;
        private int width, height;
        private float tileSize;
        private TileData[][] tiles, walls;
        private ITile selected;
        private Sprite ignore;
        private Toggle hideOnWallPlacingToggle;
        
        public int Width => 0;
        public int Height => 0;
        public float TileSize => tileSize;
        public int ChunkSize => 0;
        public int FullWidth => width;
        public int FullHeight => height;

        public StructureEditorScene(Core game, float tileSize) : base(game)
        {
            batch = new Batch(game.GraphicsDevice);
            game.camera.Position = Vec2.Zero;
            this.tileSize = tileSize;
            width = 1;
            height = 1;
            ApplyResize();
        }

        public override void Clear()
        {
            GUI.Clear();
        }

        public override void Init()
        {
            Texture2D ignoreTexture = new Texture2D(Game.GraphicsDevice, 8, 8);
            Color t = Color.Transparent, w = Color.White;
            ignoreTexture.SetData(
                new Color[] {
                    t,w,w,t,w,w,w,t,
                    t,w,w,t,w,w,w,w,
                    t,t,t,t,t,t,w,w,
                    t,w,w,t,t,t,t,t,
                    t,w,w,t,t,w,w,w,
                    t,w,w,t,t,t,t,w,
                    t,w,w,w,w,w,w,w,
                    w,w,w,w,w,w,w,t,
                }
            );
            ignore = new Sprite(ignoreTexture);
        }

        public override void InitGUI()
        {
            InputField widthField = new InputField(Vec2.Zero, new Vec2(30, 0), 30, Core.InputFieldStyle, Core.UnsignedNumbers);
            widthField.AddText("1");
            InputField heightField = new InputField(Vec2.Zero, new Vec2(30, 11), 30, Core.InputFieldStyle, Core.UnsignedNumbers);
            heightField.AddText("1");
            Dictionary<string, ITile> allTiles = Tiles.All;
            GUI
            .Add(new TextLabel(Vec2.Zero, new FRectangle(0, 0, 30, 10), "<width>:".WithTranslates(), Core.Font))
            .Add(new TextLabel(Vec2.Zero, new FRectangle(0, 11, 30, 10), "<height>:".WithTranslates(), Core.Font))
            .Add(new Button(Vec2.Zero, new FRectangle(0, 22, 60, 10),
            () =>
            {
                if (string.IsNullOrWhiteSpace(widthField.Text))
                {
                    widthField.AddText("1");
                    width = 1;
                }
                else
                {
                    width = int.Parse(widthField.Text);
                    if (width < 1)
                    {
                        width = 1;
                        widthField.ClearText();
                        widthField.AddText("1");
                    }
                }

                if (string.IsNullOrWhiteSpace(heightField.Text))
                {
                    heightField.AddText("1");
                    height = 1;
                }
                else
                {
                    height = int.Parse(heightField.Text);
                    if (height < 1)
                    {
                        height = 1;
                        heightField.ClearText();
                        heightField.AddText("1");
                    }
                }
                ApplyResize();
            }, Core.ButtonStyle, Core.Text2Icon("apply")))
            .Add(new Button(Vec2.Zero, new FRectangle(61, 0, 20, 10),
            () =>
            {
                DialogResult result = Dialog.FileSave("cmis", Settings.AppData);
                if (result.IsError)
                {
                    GUI.Add(new WindowFiller(Game.camera, Core.WindowFillerStyle, true).Add(new TextLabel(Vec2.Zero, new FRectangle(0, 0, 0.1f, 0.1f), result.ErrorMessage, Core.Font, 1.1f)));
                    return;
                }
                if (!result.IsCancelled)
                {
                    using FileStream stream = File.Create(result.Path.EndsWith("cmis") ? result.Path : $"{result.Path}.cmis");
                    StringBuilder stringBuilder = new StringBuilder().Append("struct:[");
                    bool first, firstx = true;
                    int y;
                    for (int x = 0; x < width; x++)
                    {
                        if (firstx) firstx = false;
                        else stringBuilder.Append(',');
                        stringBuilder.Append('[');
                        first = true;
                        for (y = 0; y < height; y++)
                        {
                            if (first) first = false;
                            else stringBuilder.Append(',');
                            stringBuilder.Append('[')
                                .Append(allTiles.FirstOrDefault(e => e.Value == walls[x][y].Tile).Key)
                                .Append(',')
                                .Append(allTiles.FirstOrDefault(e => e.Value == tiles[x][y].Tile).Key)
                                .Append(']');
                        }
                        stringBuilder.Append(']');
                    }
                    stringBuilder.Append("];");
                    MIS.GenerateVirtual(stringBuilder.ToString(), "struct_editor").CompileTo(stream);
                }
            }, Core.ButtonStyle, Core.Text2Icon("export")))
            .Add(new Button(Vec2.Zero, new FRectangle(61, 11, 20, 10),
            () =>
            {
                DialogResult result = Dialog.FileOpen("cmis", Settings.AppData);
                if (result.IsError)
                {
                    GUI.Add(new WindowFiller(Game.camera, Core.WindowFillerStyle, true).Add(new TextLabel(Vec2.Zero, new FRectangle(0, 0, 0.1f, 0.1f), result.ErrorMessage, Core.Font, 1.1f)));
                    return;
                }
                if (!result.IsCancelled)
                {
                    MISObject obj = MIS.Decompile(File.ReadAllBytes(result.Path));
                        int i, j;
                    obj.Require("struct", (MISArray array) =>
                    {
                        height = array.Length;
                        for (i = 0; i < array.Length; i++)
                            width = Math.Max(Width, array.Get<MISArray>(i).Length);
                        tiles = new TileData[array.Length][];
                        walls = new TileData[array.Length][];
                        for (i = 0; i < array.Length; i++)
                        {
                            MISArray row = array.Get<MISArray>(i);
                            tiles[i] = new TileData[width];
                            walls[i] = new TileData[width];
                            for (j = 0; j < row.Length; j++)
                            {
                                MISArray obj = row.Get<MISArray>(j);
                                walls[i][j] = new TileData(Tiles.Get(obj.Get<MISKey>(0).value));
                                tiles[i][j] = new TileData(Tiles.Get(obj.Get<MISKey>(1).value));
                            }
                        }
                    });
                    ref TileData data = ref NullTileReference;
                    for (i = 0; i < height; i++)
                        for (j = 0; j < width; j++)
                        {
                            data = ref walls[i][j];
                            data.Tile?.Start(false, this, i, j, ref data);
                            data = ref tiles[i][j];
                            data.Tile?.Start(true, this, i, j, ref data);
                        }
                }
            }, Core.ButtonStyle, Core.Text2Icon("import")))
            .Add(new Button(new Vec2(1, 0), new FRectangle(0, 0, 30, 10), () => Game.Scene = GameScene.Menu, Core.ButtonStyle, Core.Text2Icon("exit")))
            .Add(widthField).Add(heightField)
            .Add(new ScrollButtons<ITile>(Vec2.Zero, new FRectangle(0, 33, Core.ScrollButtonsStyle.ScrollSliderStyle.Sprites[0].Rect.Height + 8, 54), Core.ScrollButtonsStyle,
            (item) =>
            {
                selected = item;
            },
            Icon<ITile>.From((batch, rect, item, self) =>
            {
                InventoryContainer.DrawItemInfoBox(batch, (item, 0), rect.Location, self.MouseOn);
            }), 6, allTiles.Values.ToArray()))
            .Add(new TextLabel(Vec2.Zero, new FRectangle(0, 87, 50, 10), "<show_only_walls>:".WithTranslates(), Core.Font))
            .Add(hideOnWallPlacingToggle = new Toggle(Vec2.Zero, new Vec2(50, 87), Core.ToggleStyle));
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Game.GraphicsDevice.Clear(Color.Black);

            float h = tileSize / 2;
            int x, y;
            TileData data;
            for (x = 0; x < width; x++)
                for (y = 0; y < height; y++)
                {
                    if (walls[x][y].Tile is TileTag) spriteBatch.Rect(ignore, Tile.dark, new Vec2(x * tileSize + h, y * tileSize + h));
                    else
                    {
                        data = walls[x][y];
                        data.Tile?.Draw(spriteBatch, false, this, x, y, new Vec2(x * tileSize + h, y * tileSize + h), data);
                    }
                    if (!(hideOnWallPlacingToggle.Value && Mouse.RightDown))
                        if (tiles[x][y].Tile is TileTag) spriteBatch.Rect(ignore, new Vec2(x * tileSize + h, y * tileSize + h));
                        else
                        {
                            data = tiles[x][y];
                            data.Tile?.Draw(spriteBatch, true, this, x, y, new Vec2(x * tileSize + h, y * tileSize + h), data);
                        }
                }
            float w = width * tileSize;
            h = height * tileSize;

            batch.Begin(PrimitiveType.LineList, 8+width*height*2, Game.camera.GetViewMatrix());

            batch.Color = Color.Gray;
            for (x = 1; x < width; x++)
            {
                batch.Vertex3(x * tileSize, h, 0);
                batch.Vertex3(x * tileSize, 0, 0);
            }

            for (x = 1; x < height; x++)
            {
                batch.Vertex3(w, x * tileSize, 0);
                batch.Vertex3(0, x * tileSize, 0);
            }
            
            batch.Color = Color.Red;
            batch.Vertex3(0, 0, 0);
            batch.Vertex3(w, 0, 0);

            batch.Vertex3(w, 0, 0);
            batch.Vertex3(w, h, 0);

            batch.Vertex3(w, h, 0);
            batch.Vertex3(0, h, 0);

            batch.Vertex3(0, h, 0);
            batch.Vertex3(0, 0, 0);

            batch.End();
        }

        public override void Update()
        {
            if (Keyboard.IsDown(Keys.D)) Game.camera.Position += new Vec2(Time.Delta * 30, 0);
            if (Keyboard.IsDown(Keys.A)) Game.camera.Position -= new Vec2(Time.Delta * 30, 0);
            if (Keyboard.IsDown(Keys.S)) Game.camera.Position += new Vec2(0, Time.Delta * 30);
            if (Keyboard.IsDown(Keys.W)) Game.camera.Position -= new Vec2(0, Time.Delta * 30);
            var (x, y) = ((int)(Mouse.Position.X / tileSize), (int)(Mouse.Position.Y / tileSize));
            if (x >= 0 && x < width && y >= 0 && y < height)
            {
                if (Mouse.LeftDown)
                {
                    SetTile(true, selected, x, y);
                }
                if (Mouse.RightDown)
                {
                    SetTile(false, selected, x, y);
                }
            }
        }

        public void ApplyResize()
        {
            Array.Resize(ref tiles, width);
            Array.Resize(ref walls, width);
            for (int i = 0; i < width; i++)
            {
                if (tiles[i] == null)
                {
                    tiles[i] = new TileData[height];
                    walls[i] = new TileData[height];
                }
                else
                {
                    Array.Resize(ref tiles[i], height);
                    Array.Resize(ref walls[i], height);
                }
            }
        }

        public bool TryPlaceTile(bool top, ITile tile, int x, int y)
        {
            throw new System.NotImplementedException();
        }

        public bool TryPlaceTile(bool top, ITile tile, (int x, int y) position)
        {
            throw new System.NotImplementedException();
        }

        public bool PlaceTile(bool top, ITile tile, int x, int y)
        {
            throw new System.NotImplementedException();
        }

        public bool PlaceTile(bool top, ITile tile, (int x, int y) position)
        {
            throw new System.NotImplementedException();
        }

        public bool TrySetTile(bool top, ITile tile, int x, int y)
        {
            throw new System.NotImplementedException();
        }

        public bool TrySetTile(bool top, ITile tile, (int x, int y) position)
        {
            throw new System.NotImplementedException();
        }

        public bool CanSetTile(bool top, int x, int y)
        {
            throw new System.NotImplementedException();
        }

        public bool CanSetTile(bool top, (int x, int y) position)
        {
            throw new System.NotImplementedException();
        }

        public void MineTile(bool top, int x, int y, float power)
        {
            throw new System.NotImplementedException();
        }

        public void MineTile(bool top, (int x, int y) position, float power)
        {
            throw new System.NotImplementedException();
        }

        public void MineTile(bool top, int x, int y, float power, float radius)
        {
            throw new System.NotImplementedException();
        }

        public void MineTile(bool top, (int x, int y) position, float power, float radius)
        {
            throw new System.NotImplementedException();
        }

        public void SetTile(bool top, ITile tile, int x, int y)
        {
            if (tile is ReferenceTile || x < 0 || x >= width || y < 0 || y >= height) return;

            ref TileData data = ref NullTileReference;
            if (top)
            {
                tiles[x][y] = new TileData(tile);
                data = ref tiles[x][y];
            }
            else
            {
                walls[x][y] = new TileData(tile);
                data = ref walls[x][y];
            }
            tile?.Start(top, this, x, y, ref data);

            data = GetTile(top, x + 1, y);
            data.Tile?.Changed(top, this, x + 1, y, ref data);

            data = GetTile(top, x - 1, y);
            data.Tile?.Changed(top, this, x - 1, y, ref data);

            data = GetTile(top, x, y + 1);
            data.Tile?.Changed(top, this, x, y + 1, ref data);

            data = GetTile(top, x, y - 1);
            data.Tile?.Changed(top, this, x, y - 1, ref data);
        }

        public void SetTile(bool top, ITile tile, (int x, int y) position) =>
            SetTile(top, tile, position.x, position.y);

        public IChunk GetChunk(int x, int y)
        {
            throw new System.NotImplementedException();
        }

        public Rectangle? GetTileQuadtree(int x, int y)
        {
            throw new System.NotImplementedException();
        }

        public IChunk GetTileChunk(int x, int y)
        {
            throw new System.NotImplementedException();
        }

        public ref TileData GetTile(bool top, int x, int y)
        {
            if (x < 0 || x >= width || y < 0 || y >= height) return ref NullTileReference;

            return ref (top ? tiles : walls)[x][y];
        }

        public ref TileData GetTile(bool top, (int x, int y) position) =>
            ref GetTile(top, position.x, position.y);

        public ref TileData GetUnrefTile(bool top, int x, int y)
        {
            throw new System.NotImplementedException();
        }

        public ref TileData GetUnrefTile(bool top, (int x, int y) position)
        {
            throw new System.NotImplementedException();
        }

        public (int x, int y) Cell2Chunk(int x, int y)
        {
            throw new System.NotImplementedException();
        }

        public (int x, int y) Cell2Chunk((int x, int y) position)
        {
            throw new System.NotImplementedException();
        }

        public (int x, int y) World2Cell(float x, float y)
        {
            throw new System.NotImplementedException();
        }

        public (int x, int y) World2Cell(Vec2 position)
        {
            throw new System.NotImplementedException();
        }

        public Vec2 Cell2World(int x, int y)
        {
            throw new System.NotImplementedException();
        }

        public Vec2 Cell2World((int x, int y) position)
        {
            throw new System.NotImplementedException();
        }

        public void ToByte(ByteBuffer buffer)
        {
            throw new System.NotImplementedException();
        }

        public void FromByte(ByteBuffer buffer)
        {
            throw new System.NotImplementedException();
        }

        public void SetTileData<T>(bool top, int x, int y, Func<TileData, TileData> action) where T : ITile
        {
            throw new NotImplementedException();
        }

        public void SetTileData<T>(bool top, (int x, int y) position, Func<TileData, TileData> action) where T : ITile
        {
            throw new NotImplementedException();
        }
    }
}
