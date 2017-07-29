using System.Collections.Generic;

public class DynamicInterceptorCollection : List<IDynamicInterceptor>, IDynamicInterceptorCollection {
    public DynamicInterceptorCollection() { }

    public DynamicInterceptorCollection(IEnumerable<IDynamicInterceptor> interceptors)
        : base(interceptors)
    { }

}
