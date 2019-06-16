namespace ChessLib.UCI.Commands.ToEngine
{
    public class SetOption : CommandInfo
    {
        public readonly string Name, Value;
        public static readonly string NameKey = CommandAttribute.GetCommandString(AppToUCICommand.Option_Name);
        public SetOption(string name, string value) : base(AppToUCICommand.SetOption)
        {
            Name = name;
            Value = value;
        }
        public new string ToString()
        {
            return $"setoption name {Name} value {Value}";
        }
    }

}
