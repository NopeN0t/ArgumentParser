using ArgumentParser.Enums;
using ArgumentParser.Variable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArgumentParser.Parser
{
    public class Parser : IDisposable
    {
        public ArgumentBuilder Builder = new ArgumentBuilder();
        public Dictionary<string, object> ParsedValue = new Dictionary<string, object>();
        
        private readonly List<Argument> Requried = new List<Argument>();
        private readonly List<Argument> Optional = new List<Argument>();

        private string GenerateHelp(List<Argument> required, List<Argument> optional, bool FullHelp = true)
        {
            ///Generate help text for the arguments
            StringBuilder sb = new StringBuilder();
            //Short help
            sb.Append("Available arguments : [-h]");
            foreach (var arg in optional)
                sb.Append($" [{arg.ShortName}]");
            foreach (var arg in required)
                sb.Append($" {arg.Name}");

            //Detailed help
            if (FullHelp)
            {
                sb.AppendLine("\n\nRequired arguments:");
                foreach (var arg in required)
                    sb.AppendLine($"  {arg.Name}\t{arg.Description} [{arg.StoreType}]");
                sb.AppendLine("\nOptional arguments:");
                foreach (var arg in optional)
                    sb.AppendLine($"  {arg.ShortName}, {arg.Name}\t{arg.Description} [{arg.StoreType}]");
                // Add detailed help for each argument
            }
            return sb.ToString();
        }
        private void PrepareArguments()
        {
            foreach (var arg in Builder.args) //Seperate required and optional arguments
                if (arg.IsRequired)
                    Requried.Add(arg);
                else
                    Optional.Add(arg);
        }

        public bool ParseArguments(string[] args)
        {
            PrepareArguments(); 
            if (args.Contains("-h") || args.Contains("--help"))
            {
                // Display help information and end process
                Console.WriteLine(GenerateHelp(Requried, Optional));
                return false;
            }
            for (int i = 0; i < args.Length; i++) //Consume Arguments from left to right
            {
                var argument = Builder.GetArgument(args[i]);
                if (argument != null) //Handles Optional Arguments
                {
                    if (argument.StoreType == StoresType.Boolean) //If it was bool type store true
                    {
                        ParsedValue[argument.Name] = true;
                    }
                    else //Read next argument and store it
                    {
                        ParsedValue[argument.Name] = args[i + 1];
                        i++;
                    }
                }
            }
            return true;
        }
        public void Dispose()
        {
            Builder?.Dispose();
            ParsedValue?.Clear();
            Requried.Clear();
            Optional.Clear();
        }
    }
}
