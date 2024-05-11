using Jint;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using System.IO;
using Tendeos.Content;
using Tendeos.Utils.Graphics;
using Tendeos.Utils;
using Jint.Native;

namespace Tendeos.Modding
{
    public class JSModScript : IModScript
    {
        private const string defaultCode = "function drawRect(sprite,pos,rot=0,scale=1,originx=1,originy=1){__drawRect__(sprite,pos,rot,scale,originx,originy)}function drawColorRect(sprite,color,pos,rot=0,scale=1,originx=1,originy=1){__drawColorRect__(sprite,color,pos,rot,scale,originx,originy)}function drawText(text,pos,scale=1,originx=1,originy=1){__drawText__(pos,rot,scale,originx,originy)}function drawColorText(color,text,pos,scale=1,originx=1,originy=1){__drawColorText__(color,pos,rot,scale,originx,originy)}";
        private readonly Engine engine;
        private readonly Mod mod;
        private readonly string path, name;
        private bool valid;

        public JSModScript(Mod mod, SpriteBatch batch, ContentManager content, string name, string path)
        {
            valid = false;
            this.mod = mod;
            this.name = name;
            this.path = path;

            engine = new Engine()
            #region quick constructors
                .SetValue("vec2", (float x, float y) => new Vec2(x, y))
                .SetValue("rgb", (byte r, byte g, byte b) => new Color(r, g, b))
                .SetValue("rgba", (byte r, byte g, byte b, byte a) => new Color(r, g, b, a))
            #endregion
            #region objects
                .SetValue("mod", mod)
                .SetValue("misValue", Mod.misValue)
                .SetValue("origin", Mod.origin)
            #endregion
            #region content
                .SetValue("sprite", (string path) => Sprite.Load(content, path))
                .SetValue("loadSprite", (string path) => Sprite.Load(batch.GraphicsDevice, Path.Combine(mod.Path, $"{path}.png")))
                .SetValue("mods", Mods.Get)
                .SetValue("tile", Tiles.Get)
                .SetValue("biome", Biomes.Get)
                .SetValue("entity", Entities.Get)
                .SetValue("item", Items.Get)
                .SetValue("effect", Effects.Get)
                .SetValue("structure", Structures.Get)
                .SetValue("liquid", Liquids.Get)
            #endregion
            #region draw
                .SetValue("__drawRect__", (Sprite sprite, Vec2 pos, float rot, float scale, byte originx, byte originy) => batch.Rect(sprite, pos, rot, scale, 0, (Origin)originx, (Origin)originy))
                .SetValue("__drawColorRect__", (Sprite sprite, Color color, Vec2 pos, float rot, float scale, byte originx, byte originy) => batch.Rect(sprite, color, pos, rot, scale, 0, (Origin)originx, (Origin)originy))
                .SetValue("__drawText__", (string text, Vec2 pos, float scale, byte originx, byte originy) => batch.Text(Core.Font, text, pos, scale, (Origin)originx, (Origin)originy))
                .SetValue("__drawColorText__", (Color color, string text, Vec2 pos, float scale, byte originx, byte originy) => batch.Text(Core.Font, color, text, pos, scale, (Origin)originx, (Origin)originy))
            #endregion
            #region system
                .SetValue("messageBox", (string message) => MessageBox.Show("JSMessageBox", message, MessageBox.Type.Info))
                .Execute(defaultCode);
            #endregion
        }

        public bool has(string name)
        {
            if (!valid) Init();
            return engine.GetValue(name) != JsValue.Undefined;
        }

        public object invoke(string name, params object[] args)
        {
            if (!valid) Init();
            JsValue[] parameters = new JsValue[args.Length];
            for (int i = 0; i < args.Length; i++)
            {
                parameters[i] = JsValue.FromObject(engine, args[i]);
            }
            return engine.GetValue(name).Call(parameters).ToObject();
        }

        public object get(string name)
        {
            if (!valid) Init();
            return engine.GetValue(name).ToObject();
        }

        public IModMethod function(string name)
        {
            if (!valid) Init();
            return new JSModMethod(engine, engine.GetValue(name));
        }

        public void Init()
        {
            if (mod.Objects.TryGetValue(name, out MISObject value)) engine.SetValue("mis", value);
            engine.Execute(File.ReadAllText(path));
            valid = true;
        }

        public void Add(string name, object obj) => engine.SetValue(name, obj);

        public class JSModMethod : IModMethod
        {
            private readonly Engine engine;
            private readonly JsValue value;

            public JSModMethod(Engine engine, JsValue value)
            {
                this.engine = engine;
                this.value = value;
            }

            public object call(params object[] args)
            {
                JsValue[] parameters = new JsValue[args.Length];
                for (int i = 0; i < args.Length; i++)
                {
                    parameters[i] = JsValue.FromObject(engine, args[i]);
                }
                return value.Call(parameters).ToObject();
            }
        }
    }
}
