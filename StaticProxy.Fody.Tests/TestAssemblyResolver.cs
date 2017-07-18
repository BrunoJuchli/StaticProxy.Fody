using System;
using System.IO;
using Mono.Cecil;

namespace StaticProxy.Fody.Tests
{
    public class TestAssemblyResolver : DefaultAssemblyResolver
    {
        public TestAssemblyResolver(string searchDirectoryPath)
        {
            AddSearchDirectory(searchDirectoryPath);
        }
    }
}
