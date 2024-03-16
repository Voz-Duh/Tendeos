using System;

namespace XnaGame.Content.Utlis
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class ContentLoadAttribute : Attribute
    {
        public string Name { get; }
        public bool FromVariable { get; }

        public ContentLoadAttribute(string name, bool fromVariable = false)
        {
            Name = name;
            FromVariable = fromVariable;
        }
    }
}