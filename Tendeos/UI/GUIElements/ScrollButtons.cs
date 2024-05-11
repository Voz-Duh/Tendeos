using System;
using Tendeos.Utils;
using Tendeos.Utils.Input;

namespace Tendeos.UI.GUIElements
{
    public class ScrollButtons<T> : GUIElement
    {
        protected readonly ScrollButtonsStyle style;
        protected readonly Action<T> buttonAction;
        protected readonly Icon<T> buttonIcon;
        protected readonly int maxButtonCount;
        protected int scroll;
        protected readonly T[] elements;

        public ScrollButtons(Vec2 anchor, FRectangle rectangle, ScrollButtonsStyle style, Action<T> buttonAction, Icon<T> buttonIcon, int maxButtonCount, T[] elements) : base(anchor, rectangle)
        {
            this.style = style;
            this.buttonAction = buttonAction;
            this.buttonIcon = buttonIcon;
            this.maxButtonCount = maxButtonCount;
            this.elements = elements;
        }

        public override void Update(FRectangle rectangle)
        {
            base.Update(rectangle);

            if (MouseOn)
            {
                scroll = Math.Clamp(scroll - Mouse.Scroll, 0, Math.Max(elements.Length - maxButtonCount, 0));
            }
        }

        public override void OnAdd()
        {
            Add(new IntSlider(Vec2.Zero, Slider.Type.Up2Down, 0,
                0, rectangle.Height,
                style.ScrollSliderStyle,
                Math.Max(elements.Length - maxButtonCount, 0),
                () => scroll,
                v => scroll = v));
            float height = (rectangle.Height-maxButtonCount) / maxButtonCount;
            int length = Math.Min(maxButtonCount, elements.Length);
            for (int i = 0; i < length; i++)
            {
                int __i = i;
                Add(new Button(Vec2.Zero,
                    new FRectangle(
                        style.ScrollSliderStyle.Sprites[0].Rect.Height + 1, i * (height + 1),
                        rectangle.Width - style.ScrollSliderStyle.Sprites[0].Rect.Height, height
                    ), () => buttonAction(elements[__i + scroll]), style.ButtonStyle, Icon.From((batch, rect, self) => buttonIcon.Invoke(batch, rect, elements[__i + scroll], self))));
            }
        }

        public override void OnRemove() => Clear();
    }

    public class ScrollButtonsStyle
    {
        public Button.Style ButtonStyle { get; }
        public Slider.Style ScrollSliderStyle { get; }

        public ScrollButtonsStyle(Slider.Style scrollSliderStyle, Button.Style buttonStyle)
        {
            ButtonStyle = buttonStyle;
            ScrollSliderStyle = scrollSliderStyle;
        }
    }
}
