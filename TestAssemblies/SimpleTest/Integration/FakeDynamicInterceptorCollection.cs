namespace SimpleTest.Integration
{
    using System.Collections;
    using System.Collections.Generic;

    public class FakeDynamicInterceptorCollection : IDynamicInterceptorCollection
    {
        private readonly ICollection<IDynamicInterceptor> interceptors;

        public FakeDynamicInterceptorCollection(params IDynamicInterceptor[] interceptors)
        {
            this.interceptors = interceptors;
        }

        public IEnumerator<IDynamicInterceptor> GetEnumerator()
        {
            return this.interceptors.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}