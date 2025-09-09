using ArgumentParser.Enums;
using ArgumentParser.Variable;
using System;
using System.Collections.Generic;
using System.Linq;
using ArgumentParser.HelpGenerator;
using ArgumentParser.Interface;

namespace ArgumentParser.Parser
{
    public class Parser : IDisposable
    {
        public Parser(IHelpMaker HelpGenerator = null)
        {
            if (HelpGenerator == null)
                HelpGenerator = new DefaultHelpMaker();
            this.Help = HelpGenerator;
        }

        public ArgumentBuilder Builder = new ArgumentBuilder();     //Builder is Immediately disposed after parsing
        public IHelpMaker Help;                                      //Help must be disposed manually or with parser itself
        public Dictionary<string, object> ParsedValue = new Dictionary<string, object>();
        public bool IsManualHelp = false;

        //---------------------Arguments---------------------//
        //This is needed due to parsing method
        private readonly List<Argument> Requried = new List<Argument>();
        private readonly List<Argument> Optional = new List<Argument>();
        private readonly List<Argument> _Requried = new List<Argument>();
        private readonly List<Argument> _Optional = new List<Argument>();

        private bool IsPrepared = false;

        //---------------------Interal---------------------//
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
            Help = null;
            Requried.Clear(); //If parsing failed
            Optional.Clear(); //If parsing failed
        }

        //---------------------Public---------------------//
        public bool ParseArguments(string[] args, bool IgnoreUnknown = true)
        {
            ///Parse the arguments
            ///returns true if parsing was successful, false otherwise
            PrepareArguments(); //Seperates Arguments
            MakeDupeArgs(); //We process duplicates of the argument so other function won't be affected
            if (args.Contains("-h") || args.Contains("--help"))
            {
                // Display help information and end process
                Console.WriteLine(Help.GenerateHelp(Requried, Optional, true));
                CleanUp();
                IsManualHelp = true;
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
                        if (!IgnoreUnknown)
                        Console.WriteLine($"Unknown argument: {args[i]} Skipping...");
                }
                ParseRemains(); //Remaining optional arguments uses default values
                if (_Requried.Count > 0) //If there are still required arguments left
                    throw new ArgumentException("Not all requried arguments are provided");
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error parsing arguments {e.Message}");
                Console.WriteLine(Help.GenerateHelp(Requried, Optional, false));
                CleanUp();
                return false;
            }
        }
        public void Dispose()
        {
            ///Dispose of the parser
            Help = null;
            Builder?.Dispose();
            ParsedValue?.Clear();
            Requried.Clear();
            Optional.Clear();
        }
    }
}
