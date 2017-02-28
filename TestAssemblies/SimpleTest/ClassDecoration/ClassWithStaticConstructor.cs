using System.Collections.Generic;

namespace SimpleTest.ClassDecoration
{
    /// <summary>
    /// there is no constructor defined and this is a non-static class
    /// so default constructor (ctor) will be created
    /// </summary>
    [StaticProxy]
    class ClassWithStaticConstructor
    {
        /// <summary>
        /// creates a static constructor (cctor)
        /// </summary>
        private static HashSet<int> hashSet = new HashSet<int>()
        {
            1,
            2,
            3
        };
    }
}
