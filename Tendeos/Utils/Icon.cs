
using System;
using Microsoft.Xna.Framework.Graphics;
using Tendeos.UI;

namespace Tendeos.Utils
{
    public class Icon
    {
        private readonly Action<SpriteBatch, FRectangle> action;
        private readonly Action<SpriteBatch, FRectangle, GUIElement> actionSelf;
        private Icon(Action<SpriteBatch, FRectangle, GUIElement> action) => actionSelf = action;
        private Icon(Action<SpriteBatch, FRectangle> action) => this.action = action;
        public void Invoke(SpriteBatch spriteBatch, FRectangle rectangle, GUIElement self)
        {
            if (actionSelf == null) action(spriteBatch, rectangle);
            else actionSelf(spriteBatch, rectangle, self);
        }
        public static Icon From(Action<SpriteBatch, FRectangle, GUIElement> action) => new(action);
        public static Icon From(Action<SpriteBatch, FRectangle> action) => new(action);
    }
    public class Icon<T>
    {
        private readonly Action<SpriteBatch, FRectangle, T> action;
        private readonly Action<SpriteBatch, FRectangle, T, GUIElement> actionSelf;
        private Icon(Action<SpriteBatch, FRectangle, T, GUIElement> action) => actionSelf = action;
        private Icon(Action<SpriteBatch, FRectangle, T> action) => this.action = action;
        public void Invoke(SpriteBatch spriteBatch, FRectangle rectangle, T addative, GUIElement self)
        {
            if (actionSelf == null) action(spriteBatch, rectangle, addative);
            else actionSelf(spriteBatch, rectangle, addative, self);
        }
        public static Icon<T> From(Action<SpriteBatch, FRectangle, T, GUIElement> action) => new(action);
        public static Icon<T> From(Action<SpriteBatch, FRectangle, T> action) => new(action);
    }
}
