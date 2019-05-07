using System.Drawing;
using ImageMagick;
using SixLabors.ImageSharp.ColorSpaces;
using SixLabors.ImageSharp.PixelFormats;

namespace ChessLib.Graphics
{
    public class ImageOptions
    {
        public ImageOptions()
        {
            this.ShowAttackedSquares = AttackedSquares.None;
            Background = Rgba32.WhiteSmoke;
            LightSquares = Rgba32.LightGray;
            DarkSquares = Rgba32.DarkOliveGreen;
            WhitePieces = Rgba32.White;
            BlackPieces = Rgba32.Black;
            Text = Rgba32.Black;
        }


        public AttackedSquares ShowAttackedSquares { get; set; }
        public AttackedSquaresGradient AttackedSquareGradient { get; set; }

        public Rgba32 Background { get; set; }
        public Rgba32 DarkSquares { get; set; }
        public Rgba32 LightSquares { get; set; }
        public Rgba32 WhitePieces { get; set; }
        public Rgba32 BlackPieces { get; set; }
        public Rgba32 Text { get; set; }
        public Rgba32 FromColor(System.Drawing.Color color) => new Rgba32(color.R, color.G, color.B, color.A);

        public Rgba32[] GetPalette() => new[]
            {Background, DarkSquares, LightSquares, WhitePieces, BlackPieces, Text};
        private MagickColor FromRgba(Rgba32 color)
        {
            return new MagickColor(color.R, color.G, color.B, color.A);
        }
        public MagickColor TextColor => FromRgba(Text);
        public MagickColor DarkSquareColor => FromRgba(DarkSquares);
        public MagickColor LightSquareColor => FromRgba(LightSquares);
        public MagickColor BackgroundColor => FromRgba(Background);
        public MagickColor BlackPieceColor => FromRgba(BlackPieces);
        public MagickColor WhitePieceColor => FromRgba(WhitePieces);

    }

}
