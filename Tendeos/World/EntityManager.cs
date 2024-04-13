using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Tendeos.Content;
using Tendeos.Inventory;
using Tendeos.Physical;
using Tendeos.Physical.Content;
using Tendeos.Utils;

namespace Tendeos.World
{
    public static class EntityManager
    {
        private static (Action<SpriteBatch> draw, Action update) entities = (b => { }, () => { });

        public static T GetEntity<T>() where T : Entity
        {
            foreach (var entity in entities.update.GetInvocationList())
                if (entity.Target is T t)
                    return t;
            return null;
        }

        public static T[] GetEntities<T>() where T : Entity
        {
            List<T> list = new List<T>();
            foreach (var entity in entities.update.GetInvocationList())
                if (entity.Target is T t)
                    list.Add(t);
            return list.ToArray();
        }

        public static void Add(Entity entity)
        {
            entities.update += entity.Update;
            entities.draw += entity.Draw;
        }
        public static void Remove(Entity entity)
        {
            entities.update -= entity.Update;
            entities.draw -= entity.Draw;
        }

        public static void Draw(SpriteBatch spriteBatch) => entities.draw(spriteBatch);

        public static void Update() => entities.update();

        public static void Clear() => entities = (b => { }, () => { });

        public static void ToByte(ByteBuffer buffer)
        {
            List<IItem> itemTypes = new List<IItem>();
            List<Item> items = new List<Item>();
            foreach (Delegate del in entities.update.GetInvocationList())
            {
                if (del.Target is Item item)
                {
                    if (!itemTypes.Contains(item.item.Item1)) itemTypes.Add(item.item.Item1);
                    items.Add(item);
                }
            }

            buffer.Append(itemTypes.Count);
            foreach (IItem item in itemTypes)
                buffer.Append(item.Tag);
            buffer.Append(items.Count);
            foreach (Item item in items)
                buffer.Append(itemTypes.IndexOf(item.item.Item1)).Append(item.item.Item2).Append(item.position.X).Append(item.position.Y).Append(item.velocity.X).Append(item.velocity.Y);
        }

        public static void FromByte(ByteBuffer buffer)
        {
            buffer.Read(out int tlen);
            IItem[] items = new IItem[tlen];
            for (int i = 0; i < tlen; i++)
                items[i] = Items.Get(buffer.ReadString());

            buffer.Read(out int len);
            for (int i = 0; i < len; i++)
            {
                Item item = new Item((items[buffer.ReadInt()], buffer.ReadInt()), new Vec2(buffer.ReadFloat(), buffer.ReadFloat()));
                buffer.Read(out item.velocity.X).Read(out item.velocity.Y);
            }
        }
    }
}
