using ArgumentParser.Variable;
using System.Collections.Generic;

namespace ArgumentParser.Interface
{
    public interface IHelpMaker
    {
        string GenerateHelp(List<Argument> required, List<Argument> optional, bool FullHelp);
    }
}
