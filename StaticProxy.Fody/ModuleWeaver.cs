using System;

using Mono.Cecil;

namespace StaticProxy.Fody
{
    // Logging/Log can be done like this: https://github.com/Fody/PropertyChanged/blob/8d7adc36a78f9f398e4725c45dc2207ff083111e/PropertyChanged.Fody/ModuleWeaver.cs#L8

    public class ModuleWeaver
    {
        public ModuleWeaver()
        {
            Instance = this;
        }

        public static ModuleWeaver Instance { get; private set; }

        public ModuleDefinition ModuleDefinition { get; set; }

        public IAssemblyResolver AssemblyResolver { get; set; }

        public string AddinDirectoryPath { get; set; }

        public Action<string> LogInfo { get; set; }

        public Action<string> LogWarning { get; set; }

        public void Execute()
        {
            // todo remove
            // Debugger.Launch();

            try
            {
                WeavingInformation.Initialize();

                ProxyWeaver.Execute();
            }
            catch (Exception ex)
            {
                this.LogWarning(ex.StackTrace);

                throw;
            }
        }
    }
}