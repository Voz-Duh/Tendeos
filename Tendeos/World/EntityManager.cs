using System.Collections.Generic;
using Tendeos.Content;
using Tendeos.Inventory;
using Tendeos.Physical;
using Tendeos.Physical.Content;
using Tendeos.Utils;
using Tendeos.Utils.Graphics;

namespace Tendeos.World
{
    public static class EntityManager
    {
        public static SafeList<Entity> Entities { get; } = new SafeList<Entity>((e, i) => e[i] = null);

        public static T GetEntity<T>() where T : Entity
        {
            for (uint i = 0; i < Entities.Max; i++)
            {
                if (Entities[i] is T t)
                {
                    return t;
                }
            }

            return null;
        }

        public static T[] GetEntities<T>() where T : Entity
        {
            List<T> list = new List<T>();
            for (uint i = 0; i < Entities.Max; i++)
            {
                if (Entities[i] is T t)
                {
                    list.Add(t);
                }
            }

            return list.ToArray();
        }

        public static void Add(Entity entity)
        {
            entity.ID = Entities.Add(entity);
        }

        public static void Remove(Entity entity)
        {
            Entities.Destroy(entity.ID);
        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            for (uint i = 0; i < Entities.Max; i++)
            {
                Entities[i]?.Draw(spriteBatch);
            }
        }

        public static void Update()
        {
            for (uint i = 0; i < Entities.Max; i++)
            {
                Entities[i]?.Update();
            }
        }

        public static void Clear() => Entities.Clear();

        public static void ToByte(ByteBuffer buffer)
        {
            List<IItem> itemTypes = new List<IItem>();
            List<Item> items = new List<Item>();

            for (uint i = 0; i < Entities.Max; i++)
            {
                if (Entities[i] is Item item)
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
                buffer.Append(itemTypes.IndexOf(item.item.Item1)).Append(item.item.Item2).Append(item.position.X)
                    .Append(item.position.Y).Append(item.velocity.X).Append(item.velocity.Y);
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
                Item item = new Item((items[buffer.ReadInt()], buffer.ReadInt()),
                    new Vec2(buffer.ReadFloat(), buffer.ReadFloat()));
                buffer.Read(out item.velocity.X).Read(out item.velocity.Y);
            }
        }

        public static class ViewlessDisposeLoop
        {
            public static float DisposeRange;
            private static ThreadLoop viewlessDisposeThreadLoop = new(Loop, 1000*60*5);

            private static void Loop(float _)
            {
                foreach (var (i, entity) in Entities.IndexEnumerable)
                {
                    // TODO: Multiplayer check.
                    if (Vec2.Distance(entity.Position, Core.Player.Position) <= Core.ViewRadius)
                    Remove(Entities[i]);
                }
            }

            public static void Start()
            {
                viewlessDisposeThreadLoop.Start();
            }

            public static void End()
            {
                viewlessDisposeThreadLoop.Abort();
            }
        }
    }
}