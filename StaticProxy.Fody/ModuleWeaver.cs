using System;

using Mono.Cecil;
using StaticProxy.Fody;

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
        LogInfo = s => { };
        LogWarning = s => { };

        // todo remove
        // Debugger.Launch();

        try
        {
            WeavingInformation.Initialize();

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