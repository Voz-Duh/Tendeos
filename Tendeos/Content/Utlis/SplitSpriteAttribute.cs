using System;

namespace Tendeos.Content.Utlis
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
    public class SplitSpriteAttribute : Attribute
    {
        public int Rows { get; }
        public int Columns { get; }
        public int Padding { get; }
        public int Ignore { get; }


        public SplitSpriteAttribute(int rows, int collums)
        {
            Rows = rows;
            Columns = collums;
            Padding = 0;
            Ignore = 0;
        }

        public SplitSpriteAttribute(int rows, int collums, int padding)
        {
            Rows = rows;
            Columns = collums;
            Padding = padding;
            Ignore = 0;
        }

        public SplitSpriteAttribute(int rows, int collums, int padding, int ignore)
        {
            Rows = rows;
            Columns = collums;
            Padding = padding;
            Ignore = ignore;
        }
    }
}