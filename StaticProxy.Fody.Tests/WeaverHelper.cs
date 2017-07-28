using Mono.Cecil;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace StaticProxy.Fody.Tests
{
    public class WeaverHelper
    {
        private string assemblyPath;

        public WeaverHelper(string assemblyName)
        {
            Console.WriteLine("Current directory = {0}", Environment.CurrentDirectory);

            this.assemblyPath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, assemblyName + ".dll"));
        }

        public Assembly Weave()
        {
            var newAssembly = this.assemblyPath.Replace(".dll", "2.dll");

            var assemblyFileName = Path.GetFileName(newAssembly);

            Assembly assembly = AppDomain.CurrentDomain
                .GetAssemblies()
                .Where(x => !x.IsDynamic)
                .FirstOrDefault(a => Path.GetFileName(a.CodeBase) == assemblyFileName);

            if (assembly != null)
                return assembly;


            File.Copy(this.assemblyPath, newAssembly, true);
            File.Copy(this.assemblyPath.Replace(".dll", ".pdb"), newAssembly.Replace(".dll", ".pdb"), true);


            var assemblyResolver = new TestAssemblyResolver(Path.GetDirectoryName(this.assemblyPath));
            var moduleDefinition = ModuleDefinition.ReadModule(newAssembly, new ReaderParameters
            {
                AssemblyResolver = assemblyResolver,
                ReadSymbols = true,
                InMemory = true
            });
            var weavingTask = new ModuleWeaver
            {
                ModuleDefinition = moduleDefinition,
                AssemblyResolver = assemblyResolver,
                LogInfo = Console.WriteLine,
                LogWarning = Console.WriteLine,
            };

            weavingTask.Execute();

            moduleDefinition.Write(newAssembly, new WriterParameters
            {
                WriteSymbols = true
            });

            Verifier.Verify(this.assemblyPath, newAssembly);

            return Assembly.LoadFile(newAssembly);
        }
    }
}