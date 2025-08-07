using ArgumentParser.Enums;
using ArgumentParser.Parser;
using System;

namespace Demo
{
    internal class Program
    {
        static void Main(string[] args)
        {

            //This simulate inputing from terminal
            if (args.Length == 0)
                args = new string[] { "-o", "Optional", "REQ_1", "-n", "10", "REQ_2" };

            //Show arguments
            foreach (string arg in args)
                Console.Write(arg + " ");
            Console.Write("\n");

            Parser parser = new Parser(); //Python style arguments (Not all feature included)
            //Multi input arguments are not supported
            //Eg. (Req) Path1 Parh2 Path3
            parser.Builder.AddArgument("Req", "This is a required argument");
            parser.Builder.AddArgument("--opt", "-o", "This is an optional argument");
            parser.Builder.AddArgument("TEST2", "This is another required argument");
            parser.Builder.AddArgument("--Num", StoresType.Int, "-n", "This is an int input");
            parser.Builder.AddArgument("--True", StoresType.Boolean, "-t", "This stores a true value false if not included");
            parser.Builder.AddArgument("--Null", StoresType.Int, "-null", "If default value is not provided, it will be null");
            parser.Builder.AddArgument("--Float", StoresType.Float, "-f", "This is a float input with default value", "3.14");
            //The builder is disposed after parsing
            if (!parser.ParseArguments(args))
                return; //If parsing failed terminate program
            //Manual mode can be added as backup if you don't want to just terminate program
            //In this case it is recommended to extract values from parser.ParsedValue to local variables and dispose of parser

            Console.WriteLine();
            foreach (var x in parser.ParsedValue.Keys) //Show all parsed values
                Console.WriteLine($"Key : {x} Value : {parser.ParsedValue[x]}");

            //Accessing parsed values
            string req1 = (string)parser.ParsedValue["Req"];
            string req2 = (string)parser.ParsedValue["TEST2"];
            string opt = (string)parser.ParsedValue["--opt"];
            int? num = (int?)parser.ParsedValue["--Num"];
            int? nullValue = (int?)parser.ParsedValue["--Null"]; //Will be null if not provided
            float? floatValue = (float?)parser.ParsedValue["--Float"]; //Will be 3.14 if not provided
            bool isTrue = (bool)parser.ParsedValue["--True"];
            //Note: Storing in other variables is optional, you can access them directly form parser.ParsedValue
            //But this way you can dispose of parser after doing so
            parser.Dispose();

            Console.WriteLine("\nAccessed Values:");
            Console.WriteLine($"Req1: {req1}");
            Console.WriteLine($"Req2: {req2}");
            Console.WriteLine($"Optional: {opt}");
            Console.WriteLine($"Num: {num}");
            Console.WriteLine($"Null_Value: {(nullValue.HasValue ? nullValue.ToString() : "null")}");
            Console.WriteLine($"Float_Value: {floatValue}");
            Console.WriteLine($"Is_True: {isTrue}");
        }
    }
}
