using Microsoft.Xna.Framework.Content;
using System;
using System.IO;
using System.Reflection;
using Tendeos.Inventory;
using Tendeos.Physical;
using Tendeos.Utils;
using Tendeos.Utils.Graphics;
using Tendeos.World;
using Tendeos.World.Liquid;
using Tendeos.World.Structures;

namespace Tendeos.Content.Utlis
{
    public static class ContentAttributes
    {
        public static void Compute(Assets assets, Type type)
        {
            foreach (FieldInfo field in type.GetFields())
            {
                string name = field.Name;
                object obj = field.GetValue(null);
                if (obj != null) Compute(null, name, assets, obj, obj.GetType());
            }

            foreach (PropertyInfo property in type.GetProperties())
            {
                string name = property.Name;
                object obj = property.GetValue(null);
                if (obj != null) Compute(null, name, assets, obj, obj.GetType());
            }
        }

        private static void Compute(string folder, string name, Assets assets, object obj, Type type)
        {
            if (string.IsNullOrEmpty(folder))
            {
                PropertyInfo info = type.GetProperty("Folder");
                if (info != null && info.CanRead) folder = (string) info.GetValue(obj);
            }

            foreach (MethodInfo method in type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy))
            {
                GetNameAttribute attribute = method.GetCustomAttribute<GetNameAttribute>();
                if (attribute != null) method.Invoke(obj, new object[] {name});
            }

            foreach (FieldInfo field in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy))
            {
                SpriteLoadAttribute spriteLoad = field.GetCustomAttribute<SpriteLoadAttribute>();
                if (spriteLoad != null)
                {
                    object sprite = assets.GetSprite(Format(spriteLoad.FromVariable, spriteLoad.Texture, folder, name,
                        obj, type));

                    foreach (SplitSpriteAttribute split in field.GetCustomAttributes<SplitSpriteAttribute>())
                    {
                        sprite = SplitSpriteRecursively(sprite, split);
                    }

                    field.SetValue(obj, sprite);
                    continue;
                }

                ContentLoadAttribute attribute = field.GetCustomAttribute<ContentLoadAttribute>();
                if (attribute != null)
                {
                    string formated = Format(attribute.FromVariable, attribute.Name, "", name, obj, type);
                    if (formated == "") field.SetValue(obj, obj);
                    else if (typeof(Entity).IsAssignableFrom(field.FieldType))
                        field.SetValue(obj, Entities.Get(formated));
                    else if (typeof(Liquid).IsAssignableFrom(field.FieldType))
                        field.SetValue(obj, Liquids.Get(formated));
                    else if (typeof(Structure).IsAssignableFrom(field.FieldType))
                        field.SetValue(obj, Structures.Get(formated));
                    else if (typeof(IItem).IsAssignableFrom(field.FieldType)) field.SetValue(obj, Items.Get(formated));
                    else if (typeof(ITile).IsAssignableFrom(field.FieldType)) field.SetValue(obj, Tiles.Get(formated));
                    else if (typeof(Effect).IsAssignableFrom(field.FieldType))
                        field.SetValue(obj, Effects.Get(formated));
                    else throw new ContentLoadException($"Unavailable type of content \"{field.FieldType.Name}\".");
                    continue;
                }

                ContentLoadableAttribute loadable = field.GetCustomAttribute<ContentLoadableAttribute>();
                if (loadable != null)
                {
                    if (field.FieldType.IsArray)
                        foreach (object element in (object[]) field.GetValue(obj))
                            Compute(folder, name, assets, element, element.GetType());
                    else
                    {
                        object o = field.GetValue(obj);
                        Compute(folder, name, assets, o, o.GetType());
                    }

                    continue;
                }
            }

            foreach (PropertyInfo property in type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy))
            {
                if (!property.CanWrite) continue;
                
                if (property.Name == "Tag")
                {
                    property.SetValue(obj, name);
                    continue;
                }

                if (property.Name == "Name")
                {
                    property.SetValue(obj, name);
                    continue;
                }

                if (property.Name == "Description")
                {
                    property.SetValue(obj, $"{name}_description");
                    continue;
                }

                SpriteLoadAttribute spriteLoad = property.GetCustomAttribute<SpriteLoadAttribute>();
                if (spriteLoad != null)
                {
                    object sprite = assets.GetSprite(Format(spriteLoad.FromVariable, spriteLoad.Texture, folder, name,
                        obj, type));

                    foreach (SplitSpriteAttribute split in property.GetCustomAttributes<SplitSpriteAttribute>())
                    {
                        sprite = SplitSpriteRecursively(sprite, split);
                    }

                    property.SetValue(obj, sprite);
                    continue;
                }

                ContentLoadAttribute attribute = property.GetCustomAttribute<ContentLoadAttribute>();
                if (attribute != null)
                {
                    string formated = Format(attribute.FromVariable, attribute.Name, "", name, obj, type);
                    if (formated == "") property.SetValue(obj, obj);
                    else if (property.PropertyType.IsAssignableFrom(typeof(Entity)))
                        property.SetValue(obj, Entities.Get(formated));
                    else if (property.PropertyType.IsAssignableFrom(typeof(Liquid)))
                        property.SetValue(obj, Liquids.Get(formated));
                    else if (property.PropertyType.IsAssignableFrom(typeof(Structure)))
                        property.SetValue(obj, Structures.Get(formated));
                    else if (property.PropertyType.IsAssignableFrom(typeof(IItem)))
                        property.SetValue(obj, Items.Get(formated));
                    else if (property.PropertyType.IsAssignableFrom(typeof(ITile)))
                        property.SetValue(obj, Tiles.Get(formated));
                    else if (property.PropertyType.IsAssignableFrom(typeof(Effect)))
                        property.SetValue(obj, Effects.Get(formated));
                    else throw new ContentLoadException("Unavailable type of content.");
                    continue;
                }

                ContentLoadableAttribute loadable = property.GetCustomAttribute<ContentLoadableAttribute>();
                if (loadable != null)
                {
                    if (property.PropertyType.IsArray)
                        foreach (object element in (object[]) property.GetValue(obj))
                            Compute(folder, name, assets, element, element.GetType());
                    else
                    {
                        object o = property.GetValue(obj);
                        Compute(folder, name, assets, o, o.GetType());
                    }

                    continue;
                }
            }

            type.GetMethod("OnContentLoaded")?.Invoke(obj, Array.Empty<object>());
        }

        private static object SplitSpriteRecursively(object sprite, SplitSpriteAttribute split)
        {
            if (sprite.GetType().IsArray)
            {
                Array array = (Array) sprite;
                Type elementType = sprite.GetType();
                Array newArray = Array.CreateInstance(elementType, array.Length);

                for (int i = 0; i < array.Length; i++)
                {
                    object element = array.GetValue(i);
                    newArray.SetValue(SplitSpriteRecursively(element, split), i);
                }

                return newArray;
            }
            else
            {
                return ((Sprite) sprite).Split(split.Rows, split.Columns, split.Padding, split.Ignore);
            }
        }

        private static string Format(bool fromVariable, string name, string folder, string objName, object obj,
            Type type) =>
            (fromVariable ? (string) (type.GetProperty(name)?.GetValue(obj) ?? type.GetField(name)?.GetValue(obj) ?? "") : name).Replace("@",
                string.IsNullOrEmpty(folder) ? objName : Path.Combine(folder, objName));
    }
}