namespace ArgumentParser.Variable
{
    public class Argument 
    {
        public string Name;
        public string ShortName = null;
        public bool IsRequired = false;
        public string Description;
        public readonly Enums.StoresType StoreType;
        public Argument(string Name, Enums.StoresType DataType, string ShortName = null, string Description = null)
        {
            this.Name = Name;
            this.ShortName = ShortName;
            this.Description = Description;
            this.StoreType = DataType;
            IsRequired = string.IsNullOrWhiteSpace(ShortName);
        }
    }
}
