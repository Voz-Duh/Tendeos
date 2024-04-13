using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Tendeos.Modding;
using Tendeos.Modding.Content;
using Tendeos.Utils.Graphics;
using Tendeos.Utils.SaveSystem;

namespace Tendeos.Content
{
    public static class Mods
    {
        public static Dictionary<string, Mod> Loaded = new Dictionary<string, Mod>();

        public static void AddToScripts(string name, object value)
        {
            foreach (Mod mod in Loaded.Values)
                mod.AddToScripts(name, value);
        }

        public static void InvokeInMains(string name, params object[] value)
        {
            foreach (Mod mod in Loaded.Values)
                if (mod.mainScript?.has(name) ?? false)
                    mod.mainScript.invoke(name, value);
        }

        public static void Init(SpriteBatch spriteBatch, ContentManager content)
        {
            string modsPath = Path.Combine(Settings.AppData, "mods");
            foreach (string next in Directory.GetDirectories(modsPath))
            {
                Mod mod = new Mod(next, spriteBatch, content);
                if (mod.Objects.TryGetValue("", out MISObject modObject))
                    if (modObject.type == "mod")
                    {
                        string tag = "";
                        string script = "";
                        string name = tag;
                        string description = $"{tag}_description";
                        modObject
                            .Require("tag", (MISKey key) => tag = key.value)
                            .Check("script", (string path) => script = path)
                            .Check("name", (string key) => name = key)
                            .Check("description", (string key) => name = key);
                        mod.Tag = tag;
                        mod.Name = name;
                        mod.Description = description;
                        if (Loaded.TryAdd(tag, mod))
                        {
                            if (mod.Scripts.TryGetValue(script, out IModScript modScript))
                            {
                                mod.mainScript = modScript;
                                modScript.Init();
                            }
                        }
                        else throw new DuplicateNameException($"Mod {next[(modsPath.Length + 1)..]}: the \".MIS\" file have not \"mod\" type.");
                    }
                    else throw new InvalidDataException($"Mod {next[(modsPath.Length + 1)..]}: the \".MIS\" file have not \"mod\" type.");
                else throw new FileNotFoundException($"Mod {next[(modsPath.Length+1)..]} do not have \".MIS\" file.");
            }
        }

        public static void Start(GraphicsDevice graphicsDevice)
        {
            foreach (Mod mod in Loaded.Values)
            {
                foreach (var (path, mis) in mod.Objects)
                {
                    string tag = path;
                    string script = path;
                    mis.Check("tag", (MISKey arg0) => tag = arg0.value)
                        .Check("script", (string arg0) => script = arg0);
                    IModScript objScript = mod.Scripts.TryGetValue(script, out IModScript value) ? value : null;
                    string stmp;
                    switch (mis.type)
                    {
                        case "tile":
                            ModTile tile = new ModTile(objScript)
                            {
                                Tag = path,
                                Description = path,
                            };
                            tile.Drop = tile;
                            stmp = path;
                            mis.Check("name", (string arg0) => tile.Name = arg0)
                                .Check("description", (string arg0) => tile.Description = arg0)
                                .Check("health", (double arg0) => tile.Health = (float)arg0)
                                .Check("hardness", (double arg0) => tile.Hardness = (byte)arg0)
                                .Check("have_collision", (bool arg0) => tile.Collision = arg0)
                                .Check("max_count", (double arg0) => tile.MaxCount = (int)arg0)
                                .Check("shadow", (double arg0) => tile.ShadowIntensity = (float)arg0)
                                .Check("shadow", (double arg0, bool arg1) => {
                                    tile.ShadowIntensity = (float)arg0;
                                    tile.ShadowAvailable = arg1;
                                })
                                .Check("shadow", (bool arg0, double arg1) => {
                                    tile.ShadowIntensity = (float)arg1;
                                    tile.ShadowAvailable = arg0;
                                })
                                .Check("shadow", (bool arg0) => tile.ShadowAvailable = arg0)
                                .Check("drop_item", (MISRange arg0) => tile.DropCount = (int)arg0.from..(int)arg0.to)
                                .Check("drop_item", (double arg0) => tile.DropCount = (int)arg0..(int)arg0)
                                .Check("item_sprite", (string arg0) => stmp = arg0);
                            tile.ItemSprite = Sprite.Load(graphicsDevice, Path.Combine(mod.Path, $"{stmp.Replace("@", path)}.png"));
                            mod.Tiles[tag] = tile;
                            break;
                        case "auto_tile":
                            ModAutoTile auto_tile = new ModAutoTile(objScript)
                            {
                                Tag = path,
                                Description = path,
                            };
                            auto_tile.Drop = auto_tile;
                            stmp = $"{path}_item";
                            mis.Check("name", (string arg0) => auto_tile.Name = arg0)
                                .Check("description", (string arg0) => auto_tile.Description = arg0)
                                .Check("health", (double arg0) => auto_tile.Health = (float)arg0)
                                .Check("hardness", (double arg0) => auto_tile.Hardness = (byte)arg0)
                                .Check("have_collision", (bool arg0) => auto_tile.Collision = arg0)
                                .Check("max_count", (double arg0) => auto_tile.MaxCount = (int)arg0)
                                .Check("shadow", (double arg0) => auto_tile.ShadowIntensity = (float)arg0)
                                .Check("shadow", (double arg0, bool arg1) => {
                                    auto_tile.ShadowIntensity = (float)arg0;
                                    auto_tile.ShadowAvailable = arg1;
                                })
                                .Check("shadow", (bool arg0, double arg1) => {
                                    auto_tile.ShadowIntensity = (float)arg1;
                                    auto_tile.ShadowAvailable = arg0;
                                })
                                .Check("shadow", (bool arg0) => auto_tile.ShadowAvailable = arg0)
                                .Check("drop_item", (MISRange arg0) => auto_tile.DropCount = (int)arg0.from..(int)arg0.to)
                                .Check("drop_item", (double arg0) => auto_tile.DropCount = (int)arg0..(int)arg0)
                                .Check("item_sprite", (string arg0) => stmp = arg0);
                            auto_tile.ItemSprite = Sprite.Load(graphicsDevice, Path.Combine(mod.Path, $"{stmp.Replace("@", path)}.png"));
                            stmp = path;
                            mis.Check("sprite", (string arg0) => stmp = arg0);
                            auto_tile.sprites = Sprite.Load(graphicsDevice, Path.Combine(mod.Path, $"{stmp.Replace("@", path)}.png")).Split(4, 4, 1);
                            mod.Tiles[tag] = auto_tile;
                            break;
                    }
                }
            }
        }

        public static Mod Get(string value) => Loaded[value];
    }
}
