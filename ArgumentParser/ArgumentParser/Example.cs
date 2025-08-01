namespace ArgumentParser
{
    public class Example
    {
        public void RunExample()
        {   
            using(var parser = new Parser.Parser())
            {
                parser.Builder.AddArgument("FilePath", null, "Path which program operates on");
            }
        }
    }
}