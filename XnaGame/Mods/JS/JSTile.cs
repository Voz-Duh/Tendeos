using Jint;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using XnaGame.Content;
using XnaGame.Utils;
using XnaGame.Utils.Graphics;
using XnaGame.World.Content;

namespace XnaGame.Mods.JS
{
    public class JSTile : Tile
    {
        private readonly string path;
        private readonly Engine engine;

        public JSTile(Mod mod, string path, SpriteBatch batch, GraphicsDevice graphicsDevice, ContentManager content)
        {
            engine = new Engine()
                .SetValue("originOne", (byte)Origin.One)
                .SetValue("originZero", (byte)Origin.Zero)
                .SetValue("originCenter", (byte)Origin.Center)
                .SetValue("vec2", (float x, float y) => new Vec2(x, y))
                .SetValue("rgb", (byte r, byte g, byte b) => new Color(r, g, b))
                .SetValue("rgba", (byte r, byte g, byte b, byte a) => new Color(r, g, b, a))
                .SetValue("loadSprite", (string path) =>
                {
                    FileStream fileStream = new FileStream($"{mod.Path}/{path}.png", FileMode.Open);
                    Texture2D spriteAtlas = Texture2D.FromStream(graphicsDevice, fileStream);
                    fileStream.Dispose();
                    return new Sprite(spriteAtlas);
                })
                .SetValue("dloadSprite", (string path) => Sprite.Load(content, path))
                .SetValue("loadTile", (string name) => Tiles.Get($"{mod.Tag}.{name}"))
                .SetValue("dloadTile", Tiles.Get)
                .SetValue("loadBiome", (string name) => Biomes.Get($"{mod.Tag}.{name}"))
                .SetValue("dloadBiome", Biomes.Get)
                .SetValue("loadEntity", (string name) => Entities.Get($"{mod.Tag}.{name}"))
                .SetValue("dloadEntity", Entities.Get)
                .SetValue("loadItem", (string name) => Items.Get($"{mod.Tag}.{name}"))
                .SetValue("dloadItem", Items.Get)
                .SetValue("loadEffect", (string name) => Effects.Get($"{mod.Tag}.{name}"))
                .SetValue("dloadEffect", Effects.Get)
                .SetValue("loadStructure", (string name) => Structures.Get($"{mod.Tag}.{name}"))
                .SetValue("dloadStructure", Structures.Get)
                .SetValue("loadLiquid", (string name) => Liquids.Get($"{mod.Tag}.{name}"))
                .SetValue("dloadLiquid", Liquids.Get)
                .SetValue("__drawRect__", (Sprite sprite, Vec2 pos, float rot, float scale, byte originx, byte originy) => batch.Rect(sprite, pos, rot, scale, 0, (Origin)originx, (Origin)originy))
                .Execute("function drawRect(sprite, pos, rot = 0, scale = 1, originx = 1, originy = 1) { __drawRect__(sprite, pos, rot, scale, originx, originy) }")
                .SetValue("__drawColorRect__", (Sprite sprite, Color color, Vec2 pos, float rot, float scale, byte originx, byte originy) => batch.Rect(sprite, color, pos, rot, scale, 0, (Origin)originx, (Origin)originy))
                .Execute("function drawColorRect(sprite, color, pos, rot = 0, scale = 1, originx = 1, originy = 1) { __drawColorRect__(sprite, color, pos, rot, scale, originx, originy) }")
                .SetValue("__drawText__", (DynamicSpriteFontScaled font, string text, Vec2 pos, float scale, byte originx, byte originy) => batch.Text(font, text, pos, scale, 0, (Origin)originx, (Origin)originy))
                .Execute("function drawText(font, text, pos, scale = 1, originx = 1, originy = 1) { __drawText__(sprite, pos, rot, scale, originx, originy) }")
                .SetValue("__drawColorText__", (DynamicSpriteFontScaled font, Color color, string text, Vec2 pos, float scale, byte originx, byte originy) => batch.Text(font, color, text, pos, scale, 0, (Origin)originx, (Origin)originy))
                .Execute("function drawColorText(font, color, text, pos, scale = 1, originx = 1, originy = 1) { __drawColorText__(sprite, color, pos, rot, scale, originx, originy) }");
            this.path = path;
        }

        public void Init() => engine.Execute(File.ReadAllText(path));
    }
}
