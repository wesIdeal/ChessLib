using System.Windows.Input;

namespace ChessLib.UCI.Commands.FromEngine.Options
{
    public delegate void OnOptionButtonPressed(string optionName);
    public class UCIButtonOption : UCIOption<OnOptionButtonPressed>
    {
        public UCIButtonOption()
        {
        }
    }
}
