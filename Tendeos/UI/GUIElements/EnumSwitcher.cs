using System;
using Tendeos.Utils;
using Tendeos.Utils.Graphics;

namespace Tendeos.UI.GUIElements
{
    public class EnumSwitcher<T> : Button
        where T : struct, Enum
    {
        protected int index;
        protected readonly Func<T> get;
        protected readonly Action<T> set;

        public EnumSwitcher(Vec2 anchor, FRectangle rectangle, Font font, Style style, Func<T> get, Action<T> set, GUIElement[] childs = null) :
            base(anchor, rectangle, null, style, null, childs)
        {
            Array values = typeof(T).GetEnumValues();
            action = () =>
            {
                for (int i = 0; i < values.Length; i++)
                    if (((T) values.GetValue(i)).Equals(get()))
                    {
                        index = i;
                        break;
                    }

                index++;
                if (index == values.LongLength) index = 0;
                set((T) values.GetValue(index));
            };

            icon = Icon.From((batch, rect) => batch.Text(font, Localization.Translate(get().ToString()), rect.Center));
            this.get = get;
            this.set = set;
        }
    }
}