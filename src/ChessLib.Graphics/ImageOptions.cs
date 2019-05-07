using System.Drawing;
using SixLabors.ImageSharp.ColorSpaces;
using SixLabors.ImageSharp.PixelFormats;

namespace ChessLib.Graphics
{
    public class ImageOptions
    {
        public ImageOptions()
        {
            this.ShowAttackedSquares = AttackedSquares.None;
            BackgroundColor = Rgba32.WhiteSmoke;
            LightSquares = Rgba32.LightGray;
            DarkSquares = Rgba32.DarkOliveGreen;
            WhitePieces = Rgba32.White;
            BlackPieces = Rgba32.Black;
            Text = Rgba32.Black;
        }


        public AttackedSquares ShowAttackedSquares { get; set; }
        public AttackedSquaresGradient AttackedSquareGradient { get; set; }

        public Rgba32 BackgroundColor { get; set; }
        public Rgba32 DarkSquares { get; set; }
        public Rgba32 LightSquares { get; set; }
        public Rgba32 WhitePieces { get; set; }
        public Rgba32 BlackPieces { get; set; }
        public Rgba32 Text { get; set; }
        public Rgba32 FromColor(System.Drawing.Color color) => new Rgba32(color.R, color.G, color.B, color.A);

        public Rgba32[] GetPalette() => new[]
            {BackgroundColor, DarkSquares, LightSquares, WhitePieces, BlackPieces, Text};

    }

}
