
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using ImageMagick;

namespace ChessLib.Graphics
{
    public class PieceArray : IDisposable
    {
        private int _pieceSizeInPixels = 60;
        private readonly char[] _fenPieces = new char[6] { 'k', 'q', 'r', 'b', 'n', 'p' };
        /// <summary>
        /// Gets or sets the piece position in the image array. 
        /// Images are read from a main image consisting of 2 rows, 6 columns each. 
        /// The Key (of type char) is the FEN-based character representing the piece. 
        /// The Value is the zero-based position of the piece representation in the array, with row 2 being position 6.
        /// </summary>
        public Dictionary<char, int> PiecePosition { get; set; }
        /// <summary>
        /// Piece image representations, with the FEN-based character as the key.
        /// </summary>
        public Dictionary<char, IMagickImage> PieceMap { get; private set; }

        public int PieceSizeInPixels { get => _pieceSizeInPixels; set => _pieceSizeInPixels = value; }
        public PieceArray()
        {
            PiecePosition = new Dictionary<char, int>();
            for (int i = 0; i < _fenPieces.Length; i++)
            {
                var pieceChar = _fenPieces[i];
                PiecePosition.Add(pieceChar, i);
                PiecePosition.Add(char.ToUpper(pieceChar), i + 6);
            }
        }

        public PieceArray(Dictionary<char, int> piecePosition, int pieceSize)
        {
            PiecePosition = piecePosition;
            PieceSizeInPixels = pieceSize;
            ValidatePiecePositionDictionary();
        }

        private void ValidatePiecePositionDictionary()
        {

            if (PiecePosition == null) throw new NullReferenceException("Piece position must be set to load piece array to memory.");
            if (PiecePosition.Count != 12) throw new ArgumentOutOfRangeException("Piece Position Dictionary should contain 12 entries, one for each FEN-based piece.");
            foreach (var piece in _fenPieces)
            {
                if (!PiecePosition.ContainsKey(piece) || !PiecePosition.ContainsKey(char.ToUpper(piece)))
                {
                    throw new ArgumentException("Piece position dictionary must contain all FEN piece representation characters.");
                }
            }
        }

        public Dictionary<char, IMagickImage> Load(IMagickImage image)
        {
            ValidatePiecePositionDictionary();
            var pieces = new List<IMagickImage>();
            PieceMap = new Dictionary<char, IMagickImage>();
            for (var row = 0; row < 2; row++)
            {
                for (var col = 0; col < 6; col++)
                {
                    var x = col * 45;
                    var y = row * 45;
                    var piece = image.Clone(new MagickGeometry(x, y, PieceSizeInPixels, PieceSizeInPixels));
                    piece.Alpha(AlphaOption.Set);
                    pieces.Add(piece);
                }
            }
            foreach (var pieceChar in _fenPieces)
            {
                PieceMap[pieceChar] = pieces[PiecePosition[pieceChar]];
                PieceMap[char.ToUpper(pieceChar)] = pieces[PiecePosition[char.ToUpper(pieceChar)]];
            }
            return PieceMap;
        }

        public PieceArray(FileStream fileStream, Dictionary<char, int> piecePosition, int pieceSize = 60) : this(piecePosition, pieceSize)
        {

        }

        public PieceArray(Assembly assembly, string resourceStream, Dictionary<char, int> piecePosition, int pieceSize = 60) : this(piecePosition, pieceSize)
        {

        }

        public PieceArray(IMagickImage pieceArrayImage, Dictionary<char, int> piecePosition, int pieceSize = 60) : this(piecePosition, pieceSize)
        {

        }

        public void Dispose()
        {
            foreach (var piece in PieceMap)
            {
                piece.Value.Dispose();
            }
        }
    }
}
