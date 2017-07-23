namespace StaticProxy.Fody.Tests
{
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Microsoft.Build.Utilities;
    using FluentAssertions;

    public static class Verifier
    {
        static string exePath;
        static bool peverifyFound;

        static Verifier()
        {
            var sdkPath = Path.GetFullPath(Path.Combine(ToolLocationHelper.GetPathToDotNetFrameworkSdk(TargetDotNetFrameworkVersion.VersionLatest), "..\\.."));
            exePath = Directory.GetFiles(sdkPath, "peverify.exe", SearchOption.AllDirectories).LastOrDefault();

            if(exePath == null)
            {
                exePath = @"C:\Program Files (x86)\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.7 Tools\PEVerify.exe";
            }

            peverifyFound = File.Exists(exePath);
            if (!peverifyFound)
            {
#if (!DEBUG)
                throw new System.Exception("PEVerify.exe could not be found");
#else
                Debug.WriteLine("Warning: PEVerify.exe could not be found. Skipping test.");
#endif
            }
        }
        public static void Verify(string beforeAssemblyPath, string afterAssemblyPath)
        {
            if (!peverifyFound)
            {
                return;
            }
            Debug.WriteLine(afterAssemblyPath);
            var before = Validate(beforeAssemblyPath);
            var after = Validate(afterAssemblyPath);
            var message = $"Failed processing {Path.GetFileName(afterAssemblyPath)}\r\n{after}";

            if(!TrimLineNumbers(after).Equals(TrimLineNumbers(before)))
            {
                throw new System.Exception(after);
            }
        }

        public static string Validate(string assemblyPath2)
        {

            var process = Process.Start(new ProcessStartInfo(exePath, "\"" + assemblyPath2 + "\"")
            {
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            });

            process.WaitForExit(10000);
            return process.StandardOutput.ReadToEnd().Trim().Replace(assemblyPath2, "");
        }

        static string TrimLineNumbers(string foo)
        {
            return Regex.Replace(foo, @"0x.*]", "");
        }
    }
}