using System;
using System.Runtime.Serialization;

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