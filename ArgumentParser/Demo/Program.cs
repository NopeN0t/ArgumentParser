using ArgumentParser.Enums;
using ArgumentParser.Parser;
using System;

namespace Demo
{
    internal class Program
    {
        static void Main()
        {
            //This simulate inputing from terminal
            string[] args = { "-h" };
            
            //Show arguments
            foreach(string arg in args)
                Console.Write(arg + " ");
            Console.Write("\n");

            Parser parser = new Parser(); //Python style arguments (Not all feature included)
            parser.Builder.AddArgument("Req", "This is a required argument");
            parser.Builder.AddArgument("--opt", "-o", "This is an optional argument");
            parser.Builder.AddArgument("TEST2", "This is another required argument");
            parser.Builder.AddArgument("--Num", StoresType.Int, "-n", "This is an int input");
            parser.Builder.AddArgument("--True", StoresType.Boolean, "-t", "This stores a true value false if not included");
            if (!parser.ParseArguments(args))
                return; //If parsing failed terminate program
            //Manual mode can be added as backup if you don't want to just terminate program
            //In this case it is recommended to extract values from parser.ParsedValue to local variables and dispose of parser

            foreach (var x in parser.ParsedValue.Keys)
                Console.WriteLine($"Key : {x} Value : {parser.ParsedValue[x]}");
        }
    }
}
