using System;
using System.Collections.Generic;
using Tendeos.Utils;
using Tendeos.Utils.SaveSystem;

namespace Tendeos.Inventory
{
    public abstract class Inventory
    {
        public static (IItem item, int count) Selected { get; set; }

        public (IItem item, int count)[] Items { get; private set; }
        public bool Opened { get; private set; } = false;

        public Inventory(int size)
        {
            Items = new (IItem, int)[size];
        }

        public virtual void Open(Vec2 offset)
        {
            Opened = true;
        }
        public virtual void Close(Vec2 position)
        {
            Opened = false;
        }

        public abstract Inventory Copy();

        public void Get(int index)
        {
            var item = Items[index];
            if (Selected.item == null && Items[index].item == null) return;
            if (Selected.item != item.item || item.count == item.item.MaxCount)
            {
                Items[index] = Selected;
                Selected = item;
            }
            else
            {
                int count = Items[index].count += Selected.count;
                if (count > item.item.MaxCount)
                {
                    Selected = (Selected.item, count - item.item.MaxCount);
                    Items[index].count = item.item.MaxCount;
                }
                else Selected = default;
            }
        }

        public void Get(int index, Type type)
        {
            var item = Items[index];
            if ((!Selected.item?.GetType().IsAssignableFrom(type)) ?? true) return;
            if (Selected.item == null && Items[index].item == null) return;
            if (Selected.item != item.item || item.count == item.item.MaxCount)
            {
                Items[index] = Selected;
                Selected = item;
            }
            else
            {
                int count = Items[index].count += Selected.count;
                if (count > item.item.MaxCount)
                {
                    Selected = (Selected.item, count - item.item.MaxCount);
                    Items[index].count = item.item.MaxCount;
                }
                else Selected = default;
            }
        }

        public int Add(IItem item, int count)
        {
            int i, с;
            for (i = 0; i < Items.Length; i++)
                if (Items[i].item == item && item != null)
                {
                    с = Items[i].count += count;
                    if (с > item.MaxCount)
                    {
                        count = с - item.MaxCount;
                        Items[i].count = item.MaxCount;
                    }
                    else return 0;
                }

            for (i = 0; i < Items.Length; i++)
                if (Items[i].item == null)
                {
                    if (count > item.MaxCount)
                    {
                        Items[i] = (item, item.MaxCount);
                        count -= item.MaxCount;
                    }
                    else
                    {
                        Items[i] = (item, count);
                        return 0;
                    }
                }
            return count;
        }

        public void Remove(IItem item, int count)
        {
            for (int i = 0; i < Items.Length; i++)
                if (Items[i].item == item)
                {
                    count -= Items[i].count;
                    if (count < 0)
                    {
                        Items[i].count = -count;
                        return;
                    }
                    else
                        Items[i] = default;
                }
        }

        public bool Contains(IItem item, int count)
        {
            int counter = 0;
            for (int i = 0; i < Items.GetLength(0); i++)
                if (Items[i].item == item)
                    counter += Items[i].count;
            return counter >= count;
        }

        [ToByte]
        public virtual void ToByte(ByteBuffer buffer)
        {
            int i;
            (IItem, int) data;
            List<IItem> items = new List<IItem>();
            for (i = 0; i < Items.Length; i++)
                if (!items.Contains((data = Items[i]).Item1))
                    items.Add(data.Item1);
            buffer.Append(items.Count);
            buffer.Append(Items.Length);
            for (i = 0; i < items.Count; i++)
                buffer.Append(items[i]?.Tag ?? "");
            for (i = 0; i < Items.Length; i++)
            {
                data = Items[i];
                buffer.Append((byte)items.IndexOf(data.Item1));
                buffer.Append(data.Item2);
            }
        }

        [FromByte]
        public virtual void FromByte(ByteBuffer buffer)
        {
            int i, l = buffer.ReadInt(), il = buffer.ReadInt();
            Items = new (IItem, int)[il];
            IItem[] items = new IItem[l];
            for (i = 0; i < l; i++)
            {
                string str = buffer.ReadString();
                items[i] = str == "" ? null : Tendeos.Content.Items.Get(str);
            }
            for (i = 0; i < il; i++)
            {
                Items[i].item = items[buffer.ReadByte()];
                Items[i].count = buffer.ReadInt();
            }
        }
    }
}
