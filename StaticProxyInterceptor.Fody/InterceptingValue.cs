using System;

public class InterceptingValue
{
    public InterceptingValue(Type type, object value)
    {
        this.Type = type;
        this.Value = value;
    }

    public Type Type { get; private set; }

    public object Value { get; set; }
}