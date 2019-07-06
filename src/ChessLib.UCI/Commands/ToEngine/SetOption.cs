namespace ChessLib.UCI.Commands.ToEngine
{
    public class SetOption : CommandInfo
    {
        public readonly string Name, Value;

        public SetOption(string name, string value) : base(AppToUCICommand.SetOption)
        {
            Name = name;
            Value = value;
        }
        public override string ToString()
        {
            return $"setoption name {Name} value {Value}";
        }
    }

}
