using ArgumentParser.Interfrace;
using ArgumentParser.Variable;
using System.Collections.Generic;
using System.Text;

namespace ArgumentParser.HelpGenerator
{
    public class DefaultHelpMaker : IHelpMaker
    {
        string IHelpMaker.GenerateHelp(List<Argument> required, List<Argument> optional, bool FullHelp)
        {
            ///Generate help text for the arguments
            StringBuilder sb = new StringBuilder();

            //Short help
            sb.Append("Available arguments : [-h]");
            foreach (var arg in optional)
                sb.Append($" [{arg.ShortName}]");
            foreach (var arg in required)
                sb.Append($" {arg.Name}");

            if (FullHelp)
            {
                //Detailed help
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
    }
}
