using System;

namespace Tendeos.Utils.SaveSystem
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class ToByteAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class FromByteAttribute : Attribute { }
}
