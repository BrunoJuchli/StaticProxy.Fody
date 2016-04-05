using System.Reflection;

namespace StaticProxy.Interceptor.Reflection
{
    using System;

    internal class TypeInformation : ITypeInformation
    {
        public bool IsNullable(Type t)
        {
            if (t.GetTypeInfo().IsValueType)
            {
                if (t == typeof(void))
                {
                    return true;
                }

                // returns true if its a Nullabe<T> of a value type and false if its not a nullable value type
                return Nullable.GetUnderlyingType(t) != null;
            }
            
            // is a reference type and thus nullable
            return true;
        }
    }
}