using NLua;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Tendeos.Content;
using Tendeos.Utils;
using Tendeos.Utils.Graphics;

namespace Tendeos.Modding
{
    public class LuaModScript : IModScript
    {
        private const string defaultCode = "function drawRect(sprite,pos,rot,scale,originx,originy)rot=rot or 0;scale=scale or 1;originx=originx or 1;originy=originy or 1;__drawRect__(sprite,pos,rot,scale,originx,originy);end function drawColorRect(sprite,color,pos,rot,scale,originx,originy)rot=rot or 0;scale=scale or 1;originx=originx or 1;originy=originy or 1;__drawColorRect__(sprite,color,pos,rot,scale,originx,originy);end function drawText(font,text,pos,scale,originx,originy)scale=scale or 1;originx=originx or 1;originy=originy or 1;__drawText__(sprite,pos,rot,scale,originx,originy);end function drawColorText(color,text,pos,scale,originx,originy)scale=scale or 1;originx=originx or 1;originy=originy or 1;__drawColorText__(sprite,color,pos,rot,scale,originx,originy);end";
        private readonly Lua engine;
        private readonly Mod mod;
        private readonly string path, name;
        private bool valid;

        public LuaModScript(Mod mod, SpriteBatch batch, ContentManager content, string name, string path)
        {
            valid = false;
            this.mod = mod;
            this.name = name;
            this.path = path;

            engine = new Lua();
            #region quick constructors
            engine.SetObjectToPath("vec2", (float x, float y) => new Vec2(x, y));
            engine.SetObjectToPath("rgb", (byte r, byte g, byte b) => new Color(r, g, b));
            engine.SetObjectToPath("rgba", (byte r, byte g, byte b, byte a) => new Color(r, g, b, a));
            #endregion
            #region objects
            engine.SetObjectToPath("mod", mod);
            engine.SetObjectToPath("misValue", Mod.misValue);
            engine.SetObjectToPath("origin", Mod.origin);
            #endregion
            #region content
            engine.SetObjectToPath("sprite", (string arg0) => Sprite.Load(content, arg0.Replace("@", path)));
            engine.SetObjectToPath("loadSprite", (string arg0) => Sprite.Load(batch.GraphicsDevice, Path.Combine(mod.Path, $"{arg0.Replace("@", path)}.png")));
            engine.SetObjectToPath("mods", Mods.Get);
            engine.SetObjectToPath("tile", Tiles.Get);
            engine.SetObjectToPath("biome", Biomes.Get);
            engine.SetObjectToPath("entity", Entities.Get);
            engine.SetObjectToPath("item", Items.Get);
            engine.SetObjectToPath("effect", Effects.Get);
            engine.SetObjectToPath("structure", Structures.Get);
            engine.SetObjectToPath("liquid", Liquids.Get);
            #endregion
            #region draw
            engine.SetObjectToPath("__drawRect__", (Sprite sprite, Vec2 pos, float rot, float scale, byte originx, byte originy) => batch.Rect(sprite, pos, rot, scale, 0, (Origin)originx, (Origin)originy));
            engine.SetObjectToPath("__drawColorRect__", (Sprite sprite, Color color, Vec2 pos, float rot, float scale, byte originx, byte originy) => batch.Rect(sprite, color, pos, rot, scale, 0, (Origin)originx, (Origin)originy));
            engine.SetObjectToPath("__drawText__", (string text, Vec2 pos, float scale, byte originx, byte originy) => batch.Text(Core.Font, text, pos, scale, (Origin)originx, (Origin)originy));
            engine.SetObjectToPath("__drawColorText__", (Color color, string text, Vec2 pos, float scale, byte originx, byte originy) => batch.Text(Core.Font, color, text, pos, scale, (Origin)originx, (Origin)originy));
            #endregion
            #region system
            engine.SetObjectToPath("messageBox", (string message) => Debug.MessageBox(0, message, "LuaMessageBox", 0));
            engine.DoString(defaultCode);
            #endregion
        }

        public bool has(string name)
        {
            if (!valid) Init();
            return engine.GetObjectFromPath(name) != null;
        }

        public object invoke(string name, params object[] args)
        {
            if (!valid) Init();
            return engine.GetFunction(name).Call(args);
        }

        public object get(string name)
        {
            if (!valid) Init();
            return engine.GetObjectFromPath(name);
        }

        public IModMethod function(string name)
        {
            if (!valid) Init();
            return new LuaModMethod(engine.GetFunction(name));
        }

        public void Init()
        {
            if (mod.Objects.TryGetValue(name, out MISObject value)) engine.SetObjectToPath("mis", value);
            engine.DoString(File.ReadAllText(path), path);
            valid = true;
        }

        public void Add(string name, object obj) => engine.SetObjectToPath(name, obj);

        public class LuaModMethod : IModMethod
        {
            private readonly LuaFunction value;

            public LuaModMethod(LuaFunction value) => this.value = value;

            public object call(params object[] args) => value.Call(args);
        }
    }
}
