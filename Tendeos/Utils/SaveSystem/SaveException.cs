using System;

namespace Tendeos.Utils.SaveSystem
{

    [Serializable]
    public class SaveException : Exception
    {
        public SaveException() { }
        public SaveException(string message) : base(message) { }
        public SaveException(string message, Exception inner) : base(message, inner) { }
        protected SaveException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
