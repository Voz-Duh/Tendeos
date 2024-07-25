using System;
using Tendeos.Utils;
using Tendeos.Utils.Graphics;

namespace Tendeos.UI.GUIElements
{
    public class SwitchButton : Button
    {
        protected bool on;

        public SwitchButton(Vec2 anchor, FRectangle rectangle, Style style, Action actionOff, Icon iconOff,
            Action actionOn, Icon iconOn, GUIElement[] childs = null) : base(anchor, rectangle, null, style, null, childs)
        {
            on = false;
            action = () =>
            {
                if (on)
                {
                    actionOff();
                    on = false;
                }
                else
                {
                    actionOn();
                    on = true;
                }
            };

            icon = Icon.From((b, r, s) =>
            {
                if (on) iconOn.Invoke(b, r, s);
                else iconOff.Invoke(b, r, s);
            });
        }

        public SwitchButton(Vec2 anchor, FRectangle rectangle, Style style, Action actionOff, Sprite iconOff,
            Action actionOn, Sprite iconOn) : base(anchor, rectangle, null, style, null)
        {
            on = false;
            action = () =>
            {
                if (on)
                {
                    actionOff();
                    on = false;
                }
                else
                {
                    actionOn();
                    on = true;
                }
            };

            icon = Icon.From((b, r) =>
            {
                if (on) b.Rect(iconOn, r.Center);
                else b.Rect(iconOff, r.Center);
            });
        }
    }
}