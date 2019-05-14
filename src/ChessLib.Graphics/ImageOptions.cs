using ImageMagick;
using System.Drawing;

namespace ChessLib.Graphics
{
    public class ImageOptions
    {
        public ImageOptions()
        {
            this.ShowAttackedSquares = AttackedSquares.None;
            this.AttackedSquareGradient = AttackedSquaresGradient.Gradient;
            BackgroundColor = Color.WhiteSmoke;
            LightSquareColor = Color.LightGray;
            DarkSquareColor = Color.DarkOliveGreen;
            WhitePieceColor = Color.White;
            BlackPieceColor = Color.Black;
            TextColor = Color.Black;
            BlackAttackedSquareColor = Color.LightGray;
            WhiteAttackedSquareColor = Color.Pink;
            _attackedSquareTransparency = 60;
            SquareSize = 80;
        }


        public AttackedSquares ShowAttackedSquares { get; set; }
        public AttackedSquaresGradient AttackedSquareGradient { get; set; }
        #region Color Preferences
        public Color TextColor { get; set; }
        public Color DarkSquareColor { get; set; }
        public Color LightSquareColor { get; set; }
        public Color BackgroundColor { get; set; }
        public Color BlackPieceColor { get; set; }
        public Color WhitePieceColor { get; set; }
        public Color BlackAttackedSquareColor { get; set; }
        public Color WhiteAttackedSquareColor { get; set; }

        private static int _attackedSquareTransparency;
        #endregion

        public int SquareSize { get; set; }

        #region MagickColorConversions
        public MagickColor MagickTextColor => FromNetColor(TextColor);
        public MagickColor MagickBackgroundColor => FromNetColor(BackgroundColor);
        public MagickColor MagickDarkSquareColor => FromNetColor(DarkSquareColor);
        public MagickColor MagickLightSquareColor => FromNetColor(LightSquareColor);
        public MagickColor MagickWhitePieceColor => FromNetColor(WhitePieceColor);
        public MagickColor MagickBlackPieceColor => FromNetColor(BlackPieceColor);
        #endregion

        public static MagickColor FromNetColor(Color c)
        {
            return MagickColor.FromRgba(c.R, c.G, c.B, c.A);
        }


        public static MagickColor ShadeColor(Color c, int degree)
        {
            var r = c.R * (1 - (degree * 10));
            var g = c.G * (1 - (degree * 10));
            var b = c.B * (1 - (degree * 10));

            return FromNetColor(Color.FromArgb(_attackedSquareTransparency, r, g, b));
        }

    }

}
