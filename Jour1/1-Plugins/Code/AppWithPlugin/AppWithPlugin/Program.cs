// Inspiré de https://docs.microsoft.com/en-us/dotnet/standard/assembly/unloadability
// et https://github.com/dotnet/samples/tree/master/core/extensions/AppWithPlugin

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using PluginBase;

namespace AppWithPlugin
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                if (args.Length == 1 && args[0] == "/d")
                {
                    Console.WriteLine("Waiting for any key...");
                    Console.ReadLine();
                }

                string[] pluginPaths =
                {
                    @"HelloPlugin\bin\Debug\netcoreapp3.1\HelloPlugin.dll",
                    @"JsonPlugin\bin\Debug\netcoreapp3.1\JsonPlugin.dll"
                };

                foreach (var pluginPath in pluginPaths)
                {
                    ExecuteAndUnload(pluginPath, out var assemblyContextWeakReference);

                    for (int i = 0; assemblyContextWeakReference.IsAlive && (i < 10); i++)
                    {
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                    }
                }

                Console.WriteLine("Press any key!");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        static void ExecuteAndUnload(string assemblyPath, out WeakReference alcWeakRef)
        {
            var pluginPath = GetPluginPath(assemblyPath);
            var alc = new PluginLoadContext(pluginPath);
            Assembly a = alc.LoadFromAssemblyPath(pluginPath);

            alcWeakRef = new WeakReference(alc, trackResurrection: true);

            var types = a.GetTypes();

            var instance = (ICommand) Activator.CreateInstance(a.GetType(types[0].FullName));
            
            var result = instance?.Execute();

            Console.WriteLine($"Result: {result}");

            alc.Unload();
        }

        static string GetPluginPath(string relativePath)
        {
            // Navigate up to the solution root
            string root = Path.GetFullPath(Path.Combine(
                Path.GetDirectoryName(
                    Path.GetDirectoryName(
                        Path.GetDirectoryName(
                            Path.GetDirectoryName(
                                Path.GetDirectoryName(typeof(Program).Assembly.Location)))))));

            return Path.GetFullPath(Path.Combine(root, relativePath.Replace('\\', Path.DirectorySeparatorChar)));
        }

        static IEnumerable<ICommand> CreateCommands(Assembly assembly)
        {
            int count = 0;

            foreach (Type type in assembly.GetTypes())
            {
                if (typeof(ICommand).IsAssignableFrom(type))
                {
                    if (Activator.CreateInstance(type) is ICommand result)
                    {
                        count++;
                        yield return result;
                    }
                }
            }

            if (count == 0)
            {
                string availableTypes = string.Join(",", assembly.GetTypes().Select(t => t.FullName));
                throw new ApplicationException(
                    $"Can't find any type which implements ICommand in {assembly} from {assembly.Location}.\n" +
                    $"Available types: {availableTypes}");
            }
        }
    }
}
