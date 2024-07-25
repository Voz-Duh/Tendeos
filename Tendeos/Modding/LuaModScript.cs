using NLua;
using System.IO;
using Microsoft.Xna.Framework;
using Tendeos.Content;
using Tendeos.Utils;
using Tendeos.Utils.Graphics;

namespace Tendeos.Modding
{
    public class LuaModScript : IModScript
    {
        private const string defaultCode =
            "function drawRect(sprite,pos,rot,scale,originx,originy)rot=rot or 0;scale=scale or 1;originx=originx or 1;originy=originy or 1;__drawRect__(sprite,pos,rot,scale,originx,originy);end function drawColorRect(color,sprite,pos,rot,scale,originx,originy)rot=rot or 0;scale=scale or 1;originx=originx or 1;originy=originy or 1;__drawColorRect__(color,sprite,pos,rot,scale,originx,originy);end function drawText(text,pos,scale,originx,originy)scale=scale or 1;originx=originx or 1;originy=originy or 1;__drawText__(text,pos,scale,originx,originy);end function drawColorText(color,text,pos,scale,originx,originy)scale=scale or 1;originx=originx or 1;originy=originy or 1;__drawColorText__(color,text,pos,scale,originx,originy);end";

        private readonly Lua engine;
        private readonly Mod mod;
        private readonly string path, name;
        private bool valid;

        public LuaModScript(Mod mod, SpriteBatch batch, Assets assets, string path, string name)
        {
            valid = false;
            this.mod = mod;
            this.path = path;
            this.name = name;

            engine = new Lua();

            #region quick constructors

            engine.SetObjectToPath("vec2", (float x, float y) => new Vec2(x, y));
            engine.SetObjectToPath("rgb", (byte r, byte g, byte b) => new Color(r, g, b));
            engine.SetObjectToPath("rgba", (byte r, byte g, byte b, byte a) => new Color(r, g, b, a));

            #endregion

            #region objects

            engine.SetObjectToPath("mod", mod);
            engine.SetObjectToPath("misValue", Mod.misValue);

            #endregion

            #region content

            engine.SetObjectToPath("sprite", (string arg0) => assets.GetSprite(arg0.Replace("@", name)));
            engine.SetObjectToPath("get_mod", Mods.Get);
            engine.SetObjectToPath("tile", Tiles.Get);
            engine.SetObjectToPath("biome", Biomes.Get);
            engine.SetObjectToPath("entity", Entities.Get);
            engine.SetObjectToPath("item", Items.Get);
            engine.SetObjectToPath("effect", Effects.Get);
            engine.SetObjectToPath("structure", Structures.Get);
            engine.SetObjectToPath("liquid", Liquids.Get);

            #endregion

            #region draw

            engine.SetObjectToPath("__drawRect__",
                (Sprite sprite, Vec2 pos, float rot, float scale, float originx, float originy) =>
                    batch.Rect(sprite, pos, scale, rot, originx, originy));
            engine.SetObjectToPath("__drawColorRect__",
                (Color color, Sprite sprite, Vec2 pos, float rot, float scale, float originx, float originy) =>
                    batch.Rect(color, sprite, pos, scale, rot, originx, originy));
            engine.SetObjectToPath("__drawText__",
                (string text, Vec2 pos, float scale, float originx, float originy) =>
                    batch.Text(Core.Font, text, pos, scale, 0, originx, originy));
            engine.SetObjectToPath("__drawColorText__",
                (Color color, string text, Vec2 pos, float scale, float originx, float originy) =>
                    batch.Text(color, Core.Font, text, pos, scale, 0, originx, originy));

            #endregion

            #region system

            engine.SetObjectToPath("messageBox",
                (string message) => MessageBox.Show("LuaMessageBox", message, MessageBox.Type.Info));
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