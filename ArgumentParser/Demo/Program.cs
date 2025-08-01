using ArgumentParser.Parser;

namespace Demo
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //Example arguments
            args = new string[] { "-h" };

            Parser parser = new Parser();
            parser.Builder.AddArgument("help", "-h", "Display help information");
            parser.ParseArguments(args);
        }
    }
}
