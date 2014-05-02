using System;
using System.Runtime.Serialization;

namespace StaticProxy.Fody
{
    [Serializable]
    public class WeavingException : Exception
    {
        public WeavingException(string message)
            : base(message)
        {
        }

        protected WeavingException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}