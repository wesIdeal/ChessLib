using System;
using System.Collections.Generic;
using System.Reflection;
using ChessLib.Data.Helpers;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.Primitives;

namespace ChessLib.Graphics
{
    public class FENToImage
    {
        private Image<Rgba32> _boardBase;
        private readonly int _boardWidth, _squareWidth;
        private readonly Rgba32 _darkSquares = Rgba32.DarkOliveGreen;
        private readonly Rgba32 _lightSquares = Rgba32.LightGray;
        private readonly Rgba32 _background = Rgba32.WhiteSmoke;
        private Dictionary<char, Image<Rgba32>> _pieceMap;
        private readonly Font _font;
        public FENToImage(int squareWidth = 80)
        {
            _squareWidth = squareWidth;
            _boardWidth = squareWidth * 11;
            InitPieces();
            _font = SystemFonts.CreateFont("Arial", 22);
            SetBoardBaseImage();
        }

        ~FENToImage()
        {
            _boardBase.Dispose();
            foreach (var p in _pieceMap)
            {
                p.Value.Dispose();
            }
        }
        Rgba32 SquareColor(int rank, int file) => ((rank + file) % 2) == 1 ? _lightSquares : _darkSquares;

        private void InitPieces()
        {
            var assembly = Assembly.GetExecutingAssembly();
            using (var resources = assembly.GetManifestResourceStream("ChessLib.Graphics.Images.ChessPiecesArray.png"))
            {
                var bm = Image.Load(resources);
                var pieces = new List<Image<Rgba32>>();
                for (var row = 0; row < 2; row++)
                {
                    for (var col = 0; col < 6; col++)
                    {
                        var x = col * 60;
                        var y = row * 60;
                        var piece = bm.Clone(i =>
                            i.Crop(new Rectangle(x, y, 60, 60)).Resize(new Size(_squareWidth, _squareWidth)));
                        pieces.Add(piece);

                    }
                }

                _pieceMap = new Dictionary<char, Image<Rgba32>>
                {
                    {'q', pieces[0]},
                    {'k', pieces[1]},
                    {'r', pieces[2]},
                    {'n', pieces[3]},
                    {'b', pieces[4]},
                    {'p', pieces[5]},
                    {'Q', pieces[6]},
                    {'K', pieces[7]},
                    {'R', pieces[8]},
                    {'N', pieces[9]},
                    {'B', pieces[10]},
                    {'P', pieces[11]}
                };
            }
        }

        public void SaveBoardBaseImage(string file)
        {
            _boardBase.Save(file);
        }

        public void SaveBoardFromFen(string fen, string fileName)
        {
            FENHelpers.ValidateFENString(fen);
            using (var board = _boardBase.Clone())
            {
                var ranks = fen.GetRanksFromFen();
                for (var fenRank = 0; fenRank < ranks.Length; fenRank++)
                {
                    var rank = ranks[fenRank];
                    var fileCount = 0;
                    for (var file = 0; file < 8; file++)
                    {
                        var p = rank[fileCount];
                        if (char.IsDigit(p))
                        {
                            var digit = int.Parse(p.ToString());
                            file += digit - 1;
                        }
                        else
                        {
                            PieceHelpers.GetPiece(p);
                            var x = (file * _squareWidth) + _squareWidth;
                            var y = (fenRank * _squareWidth);
                            var center = new Point(x, y);
                            board.Mutate(i => i.DrawImage(_pieceMap[p], center, 1));

                        }

                        fileCount++;
                    }
                }
                board.Save(fileName);
            }
        }

        private void SetBoardBaseImage()
        {
            _boardBase = new Image<Rgba32>(_boardWidth, _boardWidth);

            for (var rank = 8; rank > 0; rank--)
            {
                for (var file = 1; file < 9; file++)
                {
                    var x = file * _squareWidth;
                    var rVal = Math.Abs(rank - 8);
                    var y = rVal * _squareWidth;
                    var rect = new RectangleF(x, y, _squareWidth, _squareWidth);
                    var brush = SquareColor(rank, file);
                    if (file != 0 || rank != 0)
                    {
                        _boardBase.Mutate(i => i.Fill(GraphicsOptions.Default, brush, rect));
                    }
                }
            }
            var textGraphicsOptions = new TextGraphicsOptions(true)
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            for (var rank = 8; rank > 0; rank--)
            {
                var y = Math.Abs(rank - 8) * _squareWidth;
                var x = 0;
                var cRank = rank.ToString();
                var rect = new RectangleF(x, y, _squareWidth, _squareWidth);
                var center = new PointF(x + (_squareWidth / 2), y + (_squareWidth / 2));
                _boardBase.Mutate(i => i.Fill(GraphicsOptions.Default, _background, rect));
                _boardBase.Mutate(i => i.DrawText(textGraphicsOptions, cRank, _font, Rgba32.Black, center));
            }

            for (var file = 0; file < 9; file++)
            {

                var y = 8 * _squareWidth;
                var x = file * _squareWidth;
                var cRank = ((char)((int)'A' + (file - 1))).ToString();

                var rect = new RectangleF(x, y, _squareWidth, _squareWidth);
                var center = new PointF(x + (_squareWidth / 2), y + (_squareWidth / 2));
                _boardBase.Mutate(i => i.Fill(GraphicsOptions.Default, _background, rect));
                if (file > 0)
                {
                    _boardBase.Mutate(i => i.DrawText(textGraphicsOptions, cRank, _font, Rgba32.Black, center));
                }
            }

        }
    }
}
