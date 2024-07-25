using System.Collections.Generic;
using System.Data;
using System.IO;
using Tendeos.Modding;
using Tendeos.Modding.Content;
using Tendeos.Utils.Graphics;
using Tendeos.Utils.SaveSystem;
using Tendeos.World.Content;

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

        public static void Init(SpriteBatch spriteBatch, Assets assets)
        {
            string modsPath = Path.Combine(Settings.AppData, "mods");
            if (!Directory.Exists(modsPath))
            {
                Directory.CreateDirectory(modsPath);
                return;
            }

            foreach (string next in Directory.GetDirectories(modsPath))
            {
                string misPath = Path.Combine(next, ".mis");
                if (!File.Exists(misPath)) continue;
                MISObject modObject = MIS.Generate(misPath);
                if (modObject.type == "mod")
                {
                    string tag = "";
                    string script = "";
                    string name = tag;
                    string description = $"{tag}_description";
                    int atlasWidth = 4080;
                    int atlasHeight = 4080;
                    modObject.Chain()
                        .Require("tag", (MISKey key) => tag = key.value)
                        .Check("script", (string path) => script = path)
                        .Check("name", (string key) => name = key)
                        .Check("description", (string key) => name = key)
                        .Check("atlasSize", (double s) => atlasWidth = atlasHeight = (int) s)
                        .Check("atlasSize", (double w, double h) => (atlasWidth, atlasHeight) = ((int) w, (int) h));
                    assets.AddFrom(next);
                    Mod mod = new Mod(next, spriteBatch, assets)
                    {
                        Tag = tag,
                        Name = name,
                        Description = description
                    };
                    if (Loaded.TryAdd(tag, mod))
                    {
                        if (mod.Scripts.TryGetValue(script, out IModScript modScript))
                        {
                            mod.mainScript = modScript;
                            modScript.Init();
                        }
                    }
                    else throw new DuplicateNameException($"Mod {next[(modsPath.Length + 1)..]}: duplicate.");
                }
            }
        }

        public static void Start()
        {
            foreach (Mod mod in Loaded.Values)
            {
                foreach (var (path, mis) in mod.assets.GetMISDictionary())
                {
                    string tag = path;
                    string script = tag;
                    mis.Chain()
                        .Check("tag", (MISKey arg0) => tag = arg0.value)
                        .Check("script", (string arg0) => script = arg0);
                    IModScript objScript = mod.Scripts.TryGetValue(script, out IModScript value) ? value : null;
                    string stmp;
                    switch (mis.type)
                    {
                        case "tile":
                            ModTile tile = new ModTile(objScript)
                            {
                                Tag = tag,
                                Description = tag,
                            };
                            tile.SetDrop(tile);
                            stmp = tag;
                            mis.Chain()
                                .Check("name", (string arg0) => tile.Name = arg0)
                                .Check("description", (string arg0) => tile.Description = arg0)
                                .Check("health", (double arg0) => tile.Health = (float) arg0)
                                .Check("hardness", (double arg0) => tile.Hardness = (byte) arg0)
                                .Check("have_collision", (bool arg0) => tile.Collision = arg0)
                                .Check("max_count", (double arg0) => tile.MaxCount = (int) arg0)
                                .Check("shadow", (double arg0) => tile.ShadowIntensity = (float) arg0)
                                .Check("shadow", (double arg0, bool arg1) =>
                                {
                                    tile.ShadowIntensity = (float) arg0;
                                    tile.ShadowAvailable = arg1;
                                })
                                .Check("shadow", (bool arg0, double arg1) =>
                                {
                                    tile.ShadowIntensity = (float) arg1;
                                    tile.ShadowAvailable = arg0;
                                })
                                .Check("shadow", (bool arg0) => tile.ShadowAvailable = arg0)
                                .Check("drop_item", (MISRange arg0) => tile.DropCount = (int) arg0.from..(int) arg0.to)
                                .Check("drop_item", (double arg0) => tile.DropCount = (int) arg0..(int) arg0)
                                .Check("item_sprite", (string arg0) => stmp = arg0);
                            tile.SetItemSprite(mod.assets.GetSprite(stmp.Replace("@", tag)));
                            mod.Tiles[tag] = tile;
                            break;
                        case "auto_tile":
                            ModAutoTile auto_tile = new ModAutoTile(objScript)
                            {
                                Tag = tag,
                                Description = tag,
                            };
                            auto_tile.SetDrop(auto_tile);
                            stmp = $"{tag}_item";
                            mis.Chain()
                                .Check("name", (string arg0) => auto_tile.Name = arg0)
                                .Check("description", (string arg0) => auto_tile.Description = arg0)
                                .Check("health", (double arg0) => auto_tile.Health = (float) arg0)
                                .Check("hardness", (double arg0) => auto_tile.Hardness = (byte) arg0)
                                .Check("have_collision", (bool arg0) => auto_tile.Collision = arg0)
                                .Check("max_count", (double arg0) => auto_tile.MaxCount = (int) arg0)
                                .Check("shadow", (double arg0) => auto_tile.ShadowIntensity = (float) arg0)
                                .Check("shadow", (double arg0, bool arg1) =>
                                {
                                    auto_tile.ShadowIntensity = (float) arg0;
                                    auto_tile.ShadowAvailable = arg1;
                                })
                                .Check("shadow", (bool arg0, double arg1) =>
                                {
                                    auto_tile.ShadowIntensity = (float) arg1;
                                    auto_tile.ShadowAvailable = arg0;
                                })
                                .Check("shadow", (bool arg0) => auto_tile.ShadowAvailable = arg0)
                                .Check("drop_item",
                                    (MISRange arg0) => auto_tile.DropCount = (int) arg0.from..(int) arg0.to)
                                .Check("drop_item", (double arg0) => auto_tile.DropCount = (int) arg0..(int) arg0)
                                .Check("item_sprite", (string arg0) => stmp = arg0);
                            auto_tile.SetItemSprite(mod.assets.GetSprite(stmp.Replace("@", tag)));
                            stmp = tag;
                            mis.Check("sprite", (string arg0) => stmp = arg0);
                            auto_tile.SetSprites(mod.assets.GetSprite(stmp.Replace("@", tag)).Split(4, 4, 1));
                            mod.Tiles[tag] = auto_tile;
                            break;
                    }
                }
            }
        }

        public static Mod Get(string value) => Loaded[value];
    }
}