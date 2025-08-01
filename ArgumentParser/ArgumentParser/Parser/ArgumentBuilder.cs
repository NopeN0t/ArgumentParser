using ArgumentParser.Enums;
using ArgumentParser.Variable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace ArgumentParser.Parser
{
    public class ArgumentBuilder : IDisposable
    {
        //No remove method is provided, as the arguments are ment to be static and predefined.
        public List<Argument> args = new List<Argument>();
        public void AddArgument(string Name, StoresType DataType, string ShortName = null, string Description = null)
        {
            ///Add an argument to be parsed
            if (args.Any(a => a.Name == Name && a.ShortName == ShortName && a.StoreType == DataType))
                throw new ArgumentException($"Argument with name {Name} and short name {ShortName} already exists with type {DataType}");
            args.Add(new Argument(Name, DataType, ShortName, Description));
        }
        public void AddArgument(string Name, string ShortName = null, string Description = null)
        {
            ///Add an argument to be parsed with default type String
            AddArgument(Name, StoresType.String, ShortName, Description);
        }
        public void AddArgument(Argument arg)
        {
            ///Add an argument to be parsed
            if (args.Any(a => a.Name == arg.Name && a.ShortName == arg.ShortName && a.StoreType == arg.StoreType))
                throw new ArgumentException($"Argument with name {arg.Name} and short name {arg.ShortName} already exists with type {arg.StoreType}");
            args.Add(arg);
        }
        public Argument GetArgument(string Name)
        {
            return args.FirstOrDefault(a => a.Name.Equals(Name, StringComparison.OrdinalIgnoreCase) ||
                                            (a.ShortName != null &&
                                            a.ShortName.Equals(Name, StringComparison.OrdinalIgnoreCase)));
        }
        public Argument GetArgument(int index)
        {
            if (index < 0 || index >= args.Count)
                throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range");
            return args[index];
        }
        public void Dispose()
        {
            args.Clear();
            args = null;
        }
    }
}
