using System;

namespace XnaGame.Content.Utlis
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class SpriteLoadAttribute : Attribute
    {
        public string Texture { get; }
        public bool FromVariable { get; }
        public int Rows { get; }
        public int Collums { get; }
        public int Padding { get; }
        public int Ignore { get; }

        public SpriteLoadAttribute(string texture, bool fromVariable = false)
        {
            Texture = texture;
            FromVariable = fromVariable;
            Rows = -1;
            Collums = -1;
            Padding = 0;
            Ignore = 0;
        }

        public SpriteLoadAttribute(string texture, int rows, int collums)
        {
            Texture = texture;
            FromVariable = false;
            Rows = rows;
            Collums = collums;
            Padding = 0;
            Ignore = 0;
        }

        public SpriteLoadAttribute(string texture, int rows, int collums, int padding)
        {
            Texture = texture;
            FromVariable = false;
            Rows = rows;
            Collums = collums;
            Padding = padding;
            Ignore = 0;
        }

        public SpriteLoadAttribute(string texture, int rows, int collums, int padding, int ignore)
        {
            Texture = texture;
            FromVariable = false;
            Rows = rows;
            Collums = collums;
            Padding = padding;
            Ignore = ignore;
        }

        public SpriteLoadAttribute(string texture, bool fromVariable, int rows, int collums)
        {
            Texture = texture;
            FromVariable = fromVariable;
            Rows = rows;
            Collums = collums;
            Padding = 0;
            Ignore = 0;
        }

        public SpriteLoadAttribute(string texture, bool fromVariable, int rows, int collums, int padding)
        {
            Texture = texture;
            FromVariable = fromVariable;
            Rows = rows;
            Collums = collums;
            Padding = padding;
            Ignore = 0;
        }

        public SpriteLoadAttribute(string texture, bool fromVariable, int rows, int collums, int padding, int ignore)
        {
            Texture = texture;
            FromVariable = fromVariable;
            Rows = rows;
            Collums = collums;
            Padding = padding;
            Ignore = ignore;
        }
    }
}