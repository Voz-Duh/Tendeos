using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Tendeos.Utils;
using Tendeos.Utils.Graphics;
using Tendeos.Utils.Input;

namespace Tendeos.UI.GUIElements
{
    public class InputField : GUIElement
    {
        public string Text => text;

        public readonly Style style;
        protected readonly HashSet<char> limitation;
        protected int position, otherPosition;
        protected string text;

        public InputField(Vec2 anchor, Vec2 position, float length, Style style, HashSet<char> limitation = null, GUIElement[] childs = null) :
            base(anchor, new FRectangle(position, new Vec2(length, style.Height)), childs)
        {
            this.style = style;
            this.limitation = limitation;
            text = "";
        }

        public override void Draw(SpriteBatch spriteBatch, FRectangle rectangle)
        {
            DrawRectWindow(spriteBatch, style.Rectangle, rectangle);
            if (Selected == this)
            {
                if (position == otherPosition)
                    spriteBatch.Rect(style.Carriage,
                        rectangle.Location +
                        new Vec2(style.TextOffset + style.Font.MeasureString(text[..position], style.FontScale).X,
                            rectangle.Height / 2));
                else
                {
                    float
                        from = style.Font.MeasureString(text[..Math.Min(position, otherPosition)], style.FontScale).X,
                        to = style.Font.MeasureString(text[..Math.Max(position, otherPosition)], style.FontScale).X,
                        y = rectangle.Height / 2,
                        relativeHeight = style.Carriage.Rect.Height / 2;
                    spriteBatch.Rect(style.Carriage,
                        new FRectangle(rectangle.Location + new Vec2(style.TextOffset + from, y - relativeHeight),
                            new Vec2(to - from, style.Carriage.Rect.Height)));
                }
            }

            spriteBatch.Text(style.Font, text, rectangle.Location + new Vec2(style.TextOffset, rectangle.Height / 2),
                style.FontScale, 0, 0);
        }

        public override void Update(FRectangle rectangle)
        {
            base.Update(rectangle);

            if (MouseOn)
            {
                if (Mouse.LeftPressed)
                {
                    if (Keyboard.IsDown(Keys.RightShift) || Keyboard.IsDown(Keys.LeftShift))
                    {
                        if (Selected != this)
                        {
                            position = 0;
                            float from;
                            for (int i = 1; i <= text.Length; i++)
                            {
                                from = rectangle.X + style.TextOffset +
                                       style.Font.MeasureString(text[..(i - 1)], style.FontScale).X;
                                if (Mouse.GUIPosition.X >= from)
                                {
                                    position = i;
                                }
                            }
                        }
                    }
                    else
                    {
                        position = 0;
                        float from;
                        for (int i = 1; i <= text.Length; i++)
                        {
                            from = rectangle.X + style.TextOffset +
                                   style.Font.MeasureString(text[..(i - 1)], style.FontScale).X;
                            if (Mouse.GUIPosition.X >= from)
                            {
                                position = i;
                            }
                        }
                    }

                    Select(this);
                }

                if (Mouse.LeftDown)
                {
                    otherPosition = 0;
                    float from;
                    for (int i = 1; i <= text.Length; i++)
                    {
                        from = rectangle.X + style.TextOffset +
                               style.Font.MeasureString(text[..(i - 1)], style.FontScale).X;
                        if (Mouse.GUIPosition.X >= from)
                        {
                            otherPosition = i;
                        }
                    }
                }
            }
        }

        public void ClearText()
        {
            text = "";
            position = otherPosition = 0;
        }

        public void AddText(string text)
        {
            this.text += text;
            position = otherPosition = this.text.Length;
        }

        public void AddText(char text)
        {
            this.text += text;
            position = otherPosition = this.text.Length;
        }

        public void AddText(object text)
        {
            this.text += text.ToString();
            position = otherPosition = this.text.Length;
        }

        public override void Select()
        {
            Core.Game.Window.TextInput += GetInput;
            Core.Game.Window.KeyDown += GetKeyInput;
        }

        public override void Unselect()
        {
            Core.Game.Window.TextInput -= GetInput;
            Core.Game.Window.KeyDown -= GetKeyInput;
        }

        private void GetKeyInput(object sender, InputKeyEventArgs e)
        {
            bool ctrl;
            switch (e.Key.Valid())
            {
                case Keys.Delete:
                    if (position == otherPosition)
                    {
                        if (position < text.Length)
                        {
                            text = text.Remove(position, 1);
                        }
                    }
                    else
                    {
                        ClearSelected();
                    }

                    break;
                case Keys.Back:
                    if (position == otherPosition)
                    {
                        if (position > 0)
                        {
                            text = text.Remove(position - 1, 1);
                            position--;
                            otherPosition = position;
                        }
                    }
                    else
                    {
                        ClearSelected();
                    }

                    break;
                case Keys.Up:
                    otherPosition = position = 0;
                    break;
                case Keys.Down:
                    otherPosition = position = text.Length;
                    break;
                case Keys.Right:
                    ctrl = Keyboard.IsDown(Keys.RightControl) || Keyboard.IsDown(Keys.LeftControl);
                    if (Keyboard.IsDown(Keys.RightShift) || Keyboard.IsDown(Keys.LeftShift))
                    {
                        if (ctrl)
                        {
                            MoveCtrlRight(ref otherPosition);
                        }
                        else
                        {
                            MoveRight(ref otherPosition);
                        }
                    }
                    else if (position == otherPosition)
                    {
                        if (ctrl)
                        {
                            MoveCtrlRight(ref position);
                            otherPosition = position;
                        }
                        else
                        {
                            MoveRight(ref position);
                            otherPosition = position;
                        }
                    }
                    else
                    {
                        position = otherPosition = Math.Max(position, otherPosition);
                    }

                    break;
                case Keys.Left:
                    ctrl = Keyboard.IsDown(Keys.RightControl) || Keyboard.IsDown(Keys.LeftControl);
                    if (Keyboard.IsDown(Keys.RightShift) || Keyboard.IsDown(Keys.LeftShift))
                    {
                        if (ctrl)
                        {
                            MoveCtrlLeft(ref otherPosition);
                        }
                        else
                        {
                            MoveLeft(ref otherPosition);
                        }
                    }
                    else if (position == otherPosition)
                    {
                        if (ctrl)
                        {
                            MoveCtrlLeft(ref position);
                            otherPosition = position;
                        }
                        else
                        {
                            MoveLeft(ref position);
                            otherPosition = position;
                        }
                    }
                    else
                    {
                        position = otherPosition = Math.Min(position, otherPosition);
                    }

                    break;
                case Keys.C:
                    if (Keyboard.IsDown(Keys.RightControl) || Keyboard.IsDown(Keys.LeftControl))
                    {
                        if (position == otherPosition) Clipboard.Text = text;
                        else CopySelected();
                    }

                    break;
                case Keys.X:
                    if (Keyboard.IsDown(Keys.RightControl) || Keyboard.IsDown(Keys.LeftControl))
                    {
                        if (position == otherPosition)
                        {
                            Clipboard.Text = text;
                            text = "";
                        }
                        else
                        {
                            CopySelected();
                            ClearSelected();
                        }
                    }

                    break;
                case Keys.V:
                    if (Keyboard.IsDown(Keys.RightControl) || Keyboard.IsDown(Keys.LeftControl))
                    {
                        Paste();
                    }

                    break;
                case Keys.A:
                    if (Keyboard.IsDown(Keys.RightControl) || Keyboard.IsDown(Keys.LeftControl))
                    {
                        position = 0;
                        otherPosition = text.Length;
                    }

                    break;
            }
        }

        private void MoveLeft(ref int carriage)
        {
            if (carriage > 0)
            {
                carriage--;
            }
        }

        private void MoveRight(ref int carriage)
        {
            if (carriage < text.Length)
            {
                carriage++;
            }
        }

        private void MoveCtrlLeft(ref int carriage)
        {
            if (carriage == 0)
            {
                return;
            }

            bool have = char.IsLetterOrDigit(text[carriage - 1]);
            if (carriage == text.Length)
            {
                carriage--;
                if (have ? !char.IsLetterOrDigit(text[carriage]) : char.IsLetterOrDigit(text[carriage]))
                    return;
            }
            else if (have ? !char.IsLetterOrDigit(text[carriage - 1]) : char.IsLetterOrDigit(text[carriage - 1]))
            {
                carriage--;
                return;
            }

            for (; carriage > 0; carriage--)
            {
                if (have ? !char.IsLetterOrDigit(text[carriage - 1]) : char.IsLetterOrDigit(text[carriage - 1]))
                {
                    return;
                }
            }

            carriage = 0;
        }

        private void MoveCtrlRight(ref int carriage)
        {
            if (carriage == text.Length)
            {
                return;
            }

            bool have = char.IsLetterOrDigit(text[carriage]);
            if (have ? !char.IsLetterOrDigit(text[carriage]) : char.IsLetterOrDigit(text[carriage]))
            {
                carriage++;
                return;
            }

            for (; carriage < text.Length; carriage++)
            {
                if (have ? !char.IsLetterOrDigit(text[carriage]) : char.IsLetterOrDigit(text[carriage]))
                {
                    return;
                }
            }

            carriage = text.Length;
        }

        private void GetInput(object sender, TextInputEventArgs e)
        {
            Print(e.Character);
        }

        private void CopySelected()
        {
            int min = Math.Min(position, otherPosition),
                max = Math.Max(position, otherPosition);
            Clipboard.Text = text[min..max];
        }

        private void ClearSelected()
        {
            int min = Math.Min(position, otherPosition),
                max = Math.Max(position, otherPosition);
            text = text.Remove(min, max - min);
            otherPosition = position = min;
        }

        private void Print(char character)
        {
            int min = Math.Min(otherPosition, position),
                max = Math.Max(otherPosition, position);
            string result = text[..min] + character + text[max..];
            if (!char.IsControl(character) && !char.IsSurrogate(character) &&
                (limitation == null || limitation.Contains(character)) &&
                style.Font.MeasureString(result, style.FontScale).X + style.TextEndOffset + style.TextOffset <=
                Rectangle.Width)
            {
                text = result;
                otherPosition = position = min + 1;
            }
        }

        private bool Print(char character, int p)
        {
            string result = text[..p] + character + text[p..];
            if (!char.IsControl(character) && !char.IsSurrogate(character) &&
                (limitation == null || limitation.Contains(character)) &&
                style.Font.MeasureString(result, style.FontScale).X + style.TextEndOffset + style.TextOffset <=
                Rectangle.Width)
            {
                text = result;
                otherPosition = position = p + 1;
                return true;
            }

            return false;
        }

        private void Paste()
        {
            string text = Clipboard.Text;

            int min = Math.Min(otherPosition, position);
            ClearSelected();
            foreach (char c in text)
                if (!Print(c, min++))
                    break;
        }

        public class Style
        {
            public Sprite[] Rectangle { get; }
            public Sprite Carriage { get; }
            public Font Font { get; }
            public float FontScale { get; }
            public float TextOffset { get; }
            public float TextEndOffset { get; }
            public float Height { get; }

            public Style(Sprite rectangle, Sprite carriage, Font font, float fontScale, float textOffset,
                float textEndOffset, float height)
            {
                Rectangle = rectangle.Split(3, 3, 1);
                Carriage = carriage;
                Font = font;
                FontScale = fontScale;
                TextOffset = textOffset;
                TextEndOffset = textEndOffset;
                Height = height;
            }
        }
    }
}