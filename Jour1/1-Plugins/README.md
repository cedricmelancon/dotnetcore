# Application avec Plugin

AssemblyLoadContext est la classe de base à utiliser pour l'utilisation de plugin. Pour pouvoir utiliser le Unload, il faut appeler le constructeur avec un boolean:<br>
```protected AssemblyLoadContext(bool isCollectible)```

Où isCollectible, lorsque mis à `True` permet de unloader l'assembly.

# Création du projet dans Visual Studio

![alt text](createproject.jpg)

# Creation du AssemblyLoadContext
```
using System;
using System.Reflection;
using System.Runtime.Loader;

namespace AppWithPlugin
{
    class PluginLoadContext : AssemblyLoadContext
    {
        private readonly AssemblyDependencyResolver _resolver;

        public PluginLoadContext(string pluginPath)
        {
            _resolver = new AssemblyDependencyResolver(pluginPath);
        }

        protected override Assembly Load(AssemblyName assemblyName)
        {
            string assemblyPath = _resolver.ResolveAssemblyToPath(assemblyName);
            if (assemblyPath != null)
            {
                return LoadFromAssemblyPath(assemblyPath);
            }

            return null;
        }

        protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
        {
            string libraryPath = _resolver.ResolveUnmanagedDllToPath(unmanagedDllName);
            if (libraryPath != null)
            {
                return LoadUnmanagedDllFromPath(libraryPath);
            }

            return IntPtr.Zero;
        }
    }
}
```

# Création de l'interface
Afin de pouvoir identifier un assembly comme étant un 

```
namespace PluginBase
{
    public interface ICommand
    {
        string Name { get; }
        string Description { get; }

        int Execute();
    }
}
```

# Création d'un plugin simple sans dépendances

```
using System;
using PluginBase;

namespace HelloPlugin
{
    public class HelloCommand : ICommand
    {
        public string Name => "hello";
        public string Description => "Hello World!";

        public int Execute()
        {
            Console.WriteLine(Description);
            return 0;
        }
    }
}
```

```
<ItemGroup>
    <ProjectReference Include="..\PluginBase\PluginBase.csproj">
        <Private>false</Private>
        <ExcludeAssets>runtime</ExcludeAssets>
    </ProjectReference>
</ItemGroup>
```

The \<Private>false\</Private> element is important. This tells MSBuild to not copy PluginBase.dll to the output directory for HelloPlugin. If the PluginBase.dll assembly is present in the output directory, PluginLoadContext will find the assembly there and load it when it loads the HelloPlugin.dll assembly.

# Création d'un plugin avec dépendances

```
<PackageReference Include="A.PluginBase" Version="1.0.0">
    <ExcludeAssets>runtime</ExcludeAssets>
</PackageReference>
```
This prevents the A.PluginBase assemblies from being copied to the output directory of your plugin and ensures that your plugin will use A's version of A.PluginBase.

Because plugin dependency loading uses the .deps.json file, there is a gotcha related to the plugin's target framework. Specifically, your plugins should target a runtime, such as .NET Core 3.0, instead of a version of .NET Standard. The .deps.json file is generated based on which framework the project targets, and since many .NET Standard-compatible packages ship reference assemblies for building against .NET Standard and implementation assemblies for specific runtimes, the .deps.json may not correctly see implementation assemblies, or it may grab the .NET Standard version of an assembly instead of the .NET Core version you expect.

# Création de l'application

```
using PluginBase;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

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

                // Load commands from plugins.

                if (args.Length == 0)
                {
                    Console.WriteLine("Commands: ");
                    // Output the loaded commands.
                }
                else
                {
                    foreach (string commandName in args)
                    {
                        Console.WriteLine($"-- {commandName} --");

                        // Execute the command with the name passed as an argument.

                        Console.WriteLine();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
```
