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
        private readonly List<Argument> _Requried = new List<Argument>();
        private readonly List<Argument> _Optional = new List<Argument>();

        private bool IsPrepared = false;

        private string GenerateHelp(List<Argument> required, List<Argument> optional, bool FullHelp = true)
        {
            ///Generate help text for the arguments
            ///returns help text as string
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
            if (IsPrepared)
                return;
            foreach (var arg in Builder.args) //Seperate required and optional arguments
                if (arg.IsRequired)
                    Requried.Add(arg);
                else
                    Optional.Add(arg);
            IsPrepared = true;
        }
        private void MakeDupeArgs()
        {
            foreach (var arg in Requried)
                _Requried.Add(arg);
            foreach (var arg in Optional)
                _Optional.Add(arg);
        }
        private object ParseValue(string value, StoresType type)
        {
            switch (type)
            {
                //Boolean is handled separately
                case StoresType.String:
                    return value; //String is already in the correct format
                case StoresType.Int:
                    return int.TryParse(value, out int intValue) ? intValue : throw new ArgumentException("Parsing Failed (int)");
                case StoresType.Long:
                    return long.TryParse(value, out long longValue) ? longValue : throw new ArgumentException("Parsing Failed (long)");
                case StoresType.Double:
                    return double.TryParse(value, out double doubleValue) ? doubleValue : throw new ArgumentException("Parsing Failed (double)");
                case StoresType.Float:
                    return float.TryParse(value, out float floatValue) ? floatValue : throw new ArgumentException("Parsing Failed (float)");
                case StoresType.Short:
                    return short.TryParse(value, out short shortValue) ? shortValue : throw new ArgumentException("Parsing Failed (short)");
                case StoresType.Byte:
                    return byte.TryParse(value, out byte byteValue) ? byteValue : throw new ArgumentException("Parsing Failed (byte)");
                default:
                    throw new ArgumentException($"Unsupported store type: {type}");
            }
        }
        private void ParseRemains()
        {
            if (_Optional.Count > 0)
                foreach (var arg in _Optional)
                {
                    if (arg.StoreType != StoresType.Boolean) //Cursed Parsing method
                        try { ParsedValue[arg.Name] = ParseValue(arg.DefultValue, arg.StoreType); }
                        catch { ParsedValue[arg.Name] = null; }
                    else
                        ParsedValue[arg.Name] = false; //Boolean defaults to false
                }
        }
        private void ParseOptionalArgument(ref Argument argument, ref string[] args, ref int i)
        {
            if (argument.StoreType == StoresType.Boolean) //If it was bool type store true
            {
                ParsedValue[argument.Name] = true;
            }
            else //Read next argument and store it
            {
                ParsedValue[argument.Name] = ParseValue(args[i + 1], argument.StoreType);
                i++;
            }
            _Optional.Remove(argument); //Remove after parsing
        }
        private void ParseRequriedArgument(ref string[] args, ref int i)
        {
            ///Parse a required argument
            //Idea parse value in order of requried arguments
            //What comes first gets parsed first then removed from the list

            Argument argument = _Requried.First(); //Get first required argument
            ParsedValue[argument.Name] = ParseValue(args[i], argument.StoreType); //Parse the value
            _Requried.RemoveAt(0); //Remove the first required argument
            //Queue is not used because of generating help text
        }
        private void CleanUp()
        {
            ///Clean up the parser after parsing
            Builder?.Dispose();
            Builder = null;
            Requried.Clear(); //If parsing failed
            Optional.Clear(); //If parsing failed
        }

        public bool ParseArguments(string[] args)
        {
            ///Parse the arguments
            ///returns true if parsing was successful, false otherwise
            PrepareArguments(); //Seperates Arguments
            MakeDupeArgs(); //We process duplicates of the argument so other function won't be affected
            if (args.Contains("-h") || args.Contains("--help"))
            {
                // Display help information and end process
                Console.WriteLine(GenerateHelp(Requried, Optional));
                CleanUp();
                return false;
            }

            try
            {
                for (int i = 0; i < args.Length; i++) //Consume Arguments from left to right
                {
                    var argument = Builder.GetArgument(args[i]);
                    if (_Optional.Contains(argument)) //Handles Optional Arguments
                        ParseOptionalArgument(ref argument, ref args, ref i); //OOP moment
                    else if (_Requried.Count != 0)
                        ParseRequriedArgument(ref args, ref i); //Handles Requried Arguments
                    else 
                        Console.WriteLine($"Unknown argument: {args[i]} Skipping...");
                }
                ParseRemains(); //Remaining optional arguments uses default values
                if (_Requried.Count > 0) //If there are still required arguments left
                {
                    Console.WriteLine("Not all required arguments were provided.");
                    Console.WriteLine(GenerateHelp(Requried, Optional, false));
                    CleanUp();
                    return false;
                }
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error parsing arguments {e.Message}");
                Console.WriteLine(GenerateHelp(Requried, Optional, false));
                CleanUp();
                return false;
            }
        }
        public void Dispose()
        {
            ///Dispose of the parser
            Builder?.Dispose();
            ParsedValue?.Clear();
            Requried.Clear();
            Optional.Clear();
        }
    }
}
