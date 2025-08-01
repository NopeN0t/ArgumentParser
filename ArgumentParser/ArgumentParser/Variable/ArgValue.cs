namespace ArgumentParser.Variable
{
    public class ArgValue <T>
    {
        public readonly string Name;
        public readonly T Value;
        public ArgValue(string name, T value)
        {
            this.Name = name;
            this.Value = value;
        }
    }
}