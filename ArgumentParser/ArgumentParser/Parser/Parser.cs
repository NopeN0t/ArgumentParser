using System;
using System.Text;

namespace ArgumentParser.Parser
{
    public class Parser : IDisposable
    {
        public ArgumentBuilder Builder = new ArgumentBuilder();
        public void ParseArguments(string[] args)
        {
            foreach (var arg in args)
            {
                if (arg.StartsWith("-") || arg.StartsWith("/"))
                {
                    var argument = Builder.GetArgument(arg);
                    if (argument == null)
                    {
                        throw new ArgumentException($"Argument '{arg}' is not recognized.");
                    }
                    // Here you would handle the argument value based on its type
                    // For simplicity, we are just printing it
                    Console.WriteLine($"Parsed argument: {argument.Name}");
                }
                else
                {
                    throw new ArgumentException($"Invalid argument format: {arg}");
                }
            }
        }
        public string GenerateHelp()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Available arguments : [-h]");
            return sb.ToString();
        }
        public void Dispose()
        {
            Builder.Dispose();
        }
    }
}
