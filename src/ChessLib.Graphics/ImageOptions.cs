namespace ChessLib.Graphics
{
    public class ImageOptions
    {
        public ImageOptions()
        {
            this.ShowAttackedSquares = AttackedSquares.None;
        }

        
        public AttackedSquares ShowAttackedSquares { get; set; }
        public AttackedSquaresGradient AttackedSquareGradient { get; set; }
    }
}
