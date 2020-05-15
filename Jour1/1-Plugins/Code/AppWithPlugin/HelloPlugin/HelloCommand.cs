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
