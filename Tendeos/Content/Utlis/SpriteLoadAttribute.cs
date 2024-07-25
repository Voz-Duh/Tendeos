using System;

namespace Tendeos.Content.Utlis
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class SpriteLoadAttribute : Attribute
    {
        public string Texture { get; }
        public bool FromVariable { get; }

        public SpriteLoadAttribute(string texture, bool fromVariable = false)
        {
            Texture = texture;
            FromVariable = fromVariable;
        }
    }
}