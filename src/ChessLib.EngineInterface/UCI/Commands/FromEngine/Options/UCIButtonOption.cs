namespace ChessLib.EngineInterface.UCI.Commands.FromEngine.Options
{
    public delegate void OnOptionButtonPressed(string optionName);
    public class UCIButtonOption : UCIOption<OnOptionButtonPressed>
    {
    }
}
