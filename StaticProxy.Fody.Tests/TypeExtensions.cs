namespace StaticProxy.Fody.Tests
{
    using System;
    using System.Linq.Expressions;

    public static class TypeExtensions
    {
        public static object GetDefaultValue(this Type type)
        {
            // Validate parameters.
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            // We want an Func<object> which returns the default.
            // Create that expression here.
            Expression<Func<object>> e = Expression.Lambda<Func<object>>(
                // Have to convert to object.
                Expression.Convert(
                // The default value, always get what the *code* tells us.
                    Expression.Default(type), typeof(object)
                )
            );

            // Compile and return the value.
            return e.Compile()();
        } 
    }
}