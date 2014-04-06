using System;

using Mono.Cecil;
using StaticProxy.Fody;

public class ModuleWeaver
{
    public ModuleDefinition ModuleDefinition { get; set; }

    public IAssemblyResolver AssemblyResolver { get; set; }

    public Action<string> LogInfo { get; set; }

    public Action<string> LogWarning { get; set; }

    public void Execute()
    {
        LogInfo = s => { };
        LogWarning = s => { };

        // todo remove
        // Debugger.Launch();

        try
        {
            WeavingInformation.Initialize(this.ModuleDefinition, this.AssemblyResolver, this.LogInfo, this.LogWarning);

            ProxyWeaver.Execute();
        }
        catch (Exception ex)
        {
            // todo remove
            LogWarning(ex.StackTrace);

            throw;
        }
    }
}