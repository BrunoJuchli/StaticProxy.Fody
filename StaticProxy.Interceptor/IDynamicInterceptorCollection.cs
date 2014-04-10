using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

[SuppressMessage("Microsoft.Design", "CA1040:AvoidEmptyInterfaces", Justification = "Some IoC's require this because standard collections (arrays, IEnumerable, ICollection, IList,...) cannot be bound.")]
public interface IDynamicInterceptorCollection : IEnumerable<IDynamicInterceptor>
{
}
