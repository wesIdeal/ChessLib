namespace ChessLib.Core.Translate
{
    public class FenTextToBoard : ChessDto<string, Board>
    {
        private static readonly FenToObject FenReader = new FenToObject();


        internal bool BypassValidation
        {
            get => FenReader.BypassValidation;
            set => FenReader.BypassValidation = value;
        }

        public override Board Translate(string from)
        {
            var fenObj = FenReader.Translate(from);
            return fenObj.AsBoard;
        }
    }
}