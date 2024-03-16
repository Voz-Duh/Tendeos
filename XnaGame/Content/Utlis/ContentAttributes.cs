using Microsoft.Xna.Framework.Content;
using System;
using System.IO;
using System.Reflection;
using XnaGame.Inventory;
using XnaGame.Physical;
using XnaGame.Utils;
using XnaGame.Utils.Graphics;
using XnaGame.World;
using XnaGame.World.Liquid;
using XnaGame.World.Structures;

namespace XnaGame.Content.Utlis
{
    public static class ContentAttributes
    {
        public static void Compute(ContentManager content, Type type)
        {
            foreach (FieldInfo field in type.GetFields())
            {
                string name = field.Name;
                object obj = field.GetValue(null);
                if (obj != null) Compute(null, name, content, obj, obj.GetType());
            }
            foreach (PropertyInfo property in type.GetProperties())
            {
                string name = property.Name;
                object obj = property.GetValue(null);
                if (obj != null) Compute(null, name, content, obj, obj.GetType());
            }
        }

        private static void Compute(string folder, string name, ContentManager content, object obj, Type type)
        {
            if (string.IsNullOrEmpty(folder))
            {
                PropertyInfo info = type.GetProperty("Folder");
                if (info != null && info.CanRead) folder = (string)info.GetValue(obj);
            }
            foreach (MethodInfo method in type.GetMethods())
            {
                GetNameAttribute attribute = method.GetCustomAttribute<GetNameAttribute>();
                if (attribute != null) method.Invoke(obj, new object[] { name });
            }
            foreach (FieldInfo field in type.GetFields())
            {
                SpriteLoadAttribute spriteLoad = field.GetCustomAttribute<SpriteLoadAttribute>();
                if (spriteLoad != null)
                {
                    if (spriteLoad.Rows == -1 || spriteLoad.Collums == -1)
                        field.SetValue(obj, Sprite.Load(content, Format(spriteLoad.FromVariable, spriteLoad.Texture, folder, name, obj, type)));
                    else
                        field.SetValue(obj, Sprite.Load(content, Format(spriteLoad.FromVariable, spriteLoad.Texture, folder, name, obj, type)).Split(spriteLoad.Rows, spriteLoad.Collums, spriteLoad.Padding, spriteLoad.Ignore));
                    continue;
                }
                ContentLoadAttribute attribute = field.GetCustomAttribute<ContentLoadAttribute>();
                if (attribute != null)
                {
                    string formated = Format(attribute.FromVariable, attribute.Name, "", name, obj, type);
                    if (formated == "") field.SetValue(obj, obj);
                    else if (typeof(Entity).IsAssignableFrom(field.FieldType)) field.SetValue(obj, Entities.Get(formated));
                    else if (typeof(Liquid).IsAssignableFrom(field.FieldType)) field.SetValue(obj, Liquids.Get(formated));
                    else if (typeof(Structure).IsAssignableFrom(field.FieldType)) field.SetValue(obj, Structures.Get(formated));
                    else if (typeof(IItem).IsAssignableFrom(field.FieldType)) field.SetValue(obj, Items.Get(formated));
                    else if (typeof(ITile).IsAssignableFrom(field.FieldType)) field.SetValue(obj, Tiles.Get(formated));
                    else if (typeof(Effect).IsAssignableFrom(field.FieldType)) field.SetValue(obj, Effects.Get(formated));
                    else throw new ContentLoadException($"Unavailable type of content \"{field.FieldType.Name}\".");
                    continue;
                }
                ContentLoadableAttribute loadable = field.GetCustomAttribute<ContentLoadableAttribute>();
                if (loadable != null)
                {
                    if (field.FieldType.IsArray)
                        foreach (object element in (object[])field.GetValue(obj))
                            Compute(folder, name, content, element, element.GetType());
                    else
                    {
                        object o = field.GetValue(obj);
                        Compute(folder, name, content, o, o.GetType());
                    }
                    continue;
                }
            }
            foreach (PropertyInfo property in type.GetProperties())
            {
                if (property.Name == "Name" && property.CanWrite)
                {
                    property.SetValue(obj, name);
                    continue;
                }
                if (property.Name == "Description" && property.CanWrite)
                {
                    property.SetValue(obj, $"{name}_description");
                    continue;
                }
                SpriteLoadAttribute spriteLoad = property.GetCustomAttribute<SpriteLoadAttribute>();
                if (spriteLoad != null)
                {
                    if (spriteLoad.Rows == -1 || spriteLoad.Collums == -1)
                        property.SetValue(obj, Sprite.Load(content, Format(spriteLoad.FromVariable, spriteLoad.Texture, folder, name, obj, type)));
                    else
                        property.SetValue(obj, Sprite.Load(content, Format(spriteLoad.FromVariable, spriteLoad.Texture, folder, name, obj, type)).Split(spriteLoad.Rows, spriteLoad.Collums, spriteLoad.Padding, spriteLoad.Ignore));
                    continue;
                }
                ContentLoadAttribute attribute = property.GetCustomAttribute<ContentLoadAttribute>();
                if (attribute != null)
                {
                    string formated = Format(attribute.FromVariable, attribute.Name, "", name, obj, type);
                    if (formated == "") property.SetValue(obj, obj);
                    else if (property.PropertyType.IsAssignableFrom(typeof(Entity))) property.SetValue(obj, Entities.Get(formated));
                    else if (property.PropertyType.IsAssignableFrom(typeof(Liquid))) property.SetValue(obj, Liquids.Get(formated));
                    else if (property.PropertyType.IsAssignableFrom(typeof(Structure))) property.SetValue(obj, Structures.Get(formated));
                    else if (property.PropertyType.IsAssignableFrom(typeof(IItem))) property.SetValue(obj, Items.Get(formated));
                    else if (property.PropertyType.IsAssignableFrom(typeof(ITile))) property.SetValue(obj, Tiles.Get(formated));
                    else if (property.PropertyType.IsAssignableFrom(typeof(Effect))) property.SetValue(obj, Effects.Get(formated));
                    else throw new ContentLoadException("Unavailable type of content.");
                    continue;
                }
                ContentLoadableAttribute loadable = property.GetCustomAttribute<ContentLoadableAttribute>();
                if (loadable != null)
                {
                    if (property.PropertyType.IsArray)
                        foreach (object element in (object[])property.GetValue(obj))
                            Compute(folder, name, content, element, element.GetType());
                    else
                    {
                        object o = property.GetValue(obj);
                        Compute(folder, name, content, o, o.GetType());
                    }
                    continue;
                }
            }
        }

        private static string Format(bool fromVariable, string name, string folder, string objName, object obj, Type type) =>
            (fromVariable ? (string)type.GetProperty(name).GetValue(obj) ?? "" : name).Replace("@", string.IsNullOrEmpty(folder) ? objName : Path.Combine(folder, objName));
    }
}
