using ChessLib.Data;
using ChessLib.Data.Helpers;
using ChessLib.Data.MoveRepresentation;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.Primitives;

using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.MetaData.Profiles.Exif;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ChessLib.Data.Types;
using SixLabors.ImageSharp.Processing.Processors.Quantization;
using SixLabors.Shapes;

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
        private readonly int _offset;
        private readonly string _blackPlayerName;
        private readonly string _whitePlayerName;

        public FENToImage(int squareWidth = 80, string black = "", string white = "")
        {

            _squareWidth = squareWidth;
            _boardWidth = squareWidth * 9;

            if (!string.IsNullOrWhiteSpace(black))
            {
                _offset += _squareWidth;
                _blackPlayerName = black;
                _whitePlayerName = white;
            }
            else _offset = 0;
            InitPieces();
            _font = SystemFonts.CreateFont("Arial", 16);
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

        public void MakeGifFromMoveTree(MoveTree<MoveHashStorage> moves, string fileName, int positionDelay, int numberOfMovementFrames = 10, int totalMovementTime = 50)
        {
            using (var board = MakeBoardFromFen(moves.FENStart))
            {
                board.Save("INIT.png");
                var previousFEN = moves.FENStart;
                foreach (var mv in moves)
                {

                    var from = mv.Move.Move.SourceIndex();
                    var to = mv.Move.Move.DestinationIndex();
                    var pieceMoving =
                        _pieceMap[PieceHelpers.GetCharRepresentation(mv.Move.ColorMoving, mv.Move.PieceMoving)];
                    var transImages = MakeMovementFrames(previousFEN, pieceMoving, mv.Move.Move.SourceIndex(), mv.Move.Move.DestinationIndex(), numberOfMovementFrames, totalMovementTime);
                    foreach (var f in transImages)
                    {
                        var frameDelay = (int)totalMovementTime / numberOfMovementFrames;
                        board.Frames.AddFrame(f.Frames[0]);
                        var gifMetaData = board.Frames.Last().MetaData.GetFormatMetaData(GifFormat.Instance);
                        gifMetaData.FrameDelay = frameDelay;
                        gifMetaData.ColorTableLength = 5;
                        f.Dispose();

                    }
                    var finalBoard = MakeBoardFromFen(mv.Move.FEN);


                    var finalPosition = finalBoard.Frames.RootFrame;

                    finalPosition.MetaData.GetFormatMetaData(GifFormat.Instance).FrameDelay = positionDelay;
                    finalPosition.MetaData.GetFormatMetaData(GifFormat.Instance).ColorTableLength = 4;

                    board.Frames.AddFrame(finalPosition);
                    previousFEN = mv.Move.FEN;

                }

                var firstFrame = board.Frames.First().MetaData.GetFormatMetaData(GifFormat.Instance);
                firstFrame.FrameDelay = positionDelay * 2;
                var finalFrame = board.Frames.Last().MetaData.GetFormatMetaData(GifFormat.Instance);
                finalFrame.FrameDelay = (int)Math.Round(positionDelay * 2.5);
                board.MetaData.GetFormatMetaData(GifFormat.Instance).ColorTableMode = GifColorTableMode.Global;

                board.Save(fileName);


                //for (int i = 0; i < board.Frames.Count; i++)
                //{
                //    board.Frames.CloneFrame(i).Save(System.IO.Path.Combine(".\\Game1\\", $"frame.{i}.png"));
                //}
            }
        }

        private IEnumerable<Image<Rgba32>> MakeMovementFrames(string fen, Image<Rgba32> pieceMoving,
            ushort sqFrom, ushort sqTo, int frames, int delay)
        {

            var rv = new List<Image<Rgba32>>();
            var pFrom = GetPointFromBoardIndex(sqFrom);
            var pTo = GetPointFromBoardIndex(sqTo);
            Path p = new Path(new LinearLineSegment(pFrom, pTo));
            var sizeOfTransitions = p.Length / (frames + 1);
            var points = new PointF[frames];
            int i = 0;
            for (i = 0; i < frames; i++)
            {
                points[i] = p.PointAlongPath(sizeOfTransitions * i).Point;
            }

            i = 0;
            var moveNumber = FENHelpers.GetMoveNumberFromString(fen.GetFENPiece(FENPieces.FullMoveCounter));
            var activeSide = FENHelpers.GetActiveColor(fen.GetFENPiece(FENPieces.ActiveColor));


            using (var baseImage = MakeBoardFromFen(fen, sqFrom))
            {

                foreach (var point in points)
                {
                    using (var tImage = baseImage.Clone())
                    {

                        var drawPoint = new Point((int)point.X, (int)point.Y);
                        tImage.Mutate(m => m.DrawImage(pieceMoving, drawPoint, 1));
                        rv.Add(tImage.Clone());
                    }
                }
            }
            return rv.ToArray();
        }

        private Point GetPointFromBoardIndex(ushort sq)
        {
            var x = ((sq % 8) * _squareWidth) + _squareWidth;
            var y = (Math.Abs((sq / 8) - 7)) * _squareWidth + _offset;
            return new Point(x, y);

        }
        private Rectangle GetRectFromBoardIndex(ushort square)
        {
            var p = GetPointFromBoardIndex(square);
            return new Rectangle(p.X, p.Y + _offset, _squareWidth, _squareWidth);
        }

        private Image<Rgba32> MakeBoardFromFen(string fen, ushort? leaveEmptyBoardIndex = null)
        {
            var board = _boardBase.Clone();
            var ranks = fen.GetRanksFromFen();
            Point? emptySquare = null;
            if (leaveEmptyBoardIndex.HasValue)
                emptySquare = GetPointFromBoardIndex(leaveEmptyBoardIndex.Value);
            for (var fenRank = 0; fenRank < ranks.Length; fenRank++)
            {
                var rank = ranks[fenRank];
                var fileCount = 0;
                for (var file = 0; file < 8; file++)
                {
                    var x = (file * _squareWidth) + _squareWidth;
                    var y = (fenRank * _squareWidth) + _offset;
                    var p = rank[fileCount];
                    if (char.IsDigit(p))
                    {
                        var digit = int.Parse(p.ToString());
                        file += digit - 1;
                    }
                    else
                    {
                        PieceHelpers.GetPiece(p);
                        var center = new Point(x, y);
                        if (ShouldDrawPieceInSquare(emptySquare, center))
                        {
                            board.Mutate(i => i.DrawImage(_pieceMap[p], center, 1));
                        }
                    }

                    fileCount++;
                }
            }

            return board;
        }

        private static bool ShouldDrawPieceInSquare(Point? emptySquare, Point currentSquare)
        {
            if (emptySquare.HasValue)
            {
                if (emptySquare.Value == currentSquare)
                {
                    return false;
                }
            }
            return true;
        }

        public void SaveBoardFromFen(string fen, string fileName)
        {
            FENHelpers.ValidateFENString(fen);
            using (var board = MakeBoardFromFen(fen))
            {

                board.Save(fileName);
            }
        }

        private void SetBoardBaseImage()
        {

            _boardBase = new Image<Rgba32>(_boardWidth, _boardWidth + (_offset * 2));
            var textGraphicsOptions = new TextGraphicsOptions(true)
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            if (_offset != 0)
            {
                var centerOfRank = new PointF(_boardWidth / 2, _squareWidth / 2);
                _boardBase.Mutate(x => x.DrawText(textGraphicsOptions, _blackPlayerName, _font, Rgba32.Black, centerOfRank));
                centerOfRank.Y = _squareWidth * 10 + (_offset / 2);
                _boardBase.Mutate(x => x.DrawText(textGraphicsOptions, _whitePlayerName, _font, Rgba32.Black, centerOfRank));
            }
            for (var rank = 8; rank > 0; rank--)
            {
                for (var file = 1; file < 9; file++)
                {
                    var x = file * _squareWidth;
                    var rVal = Math.Abs(rank - 8);
                    var y = (rVal * _squareWidth) + _offset;
                    var rect = new RectangleF(x, y, _squareWidth, _squareWidth);
                    var brush = SquareColor(rank, file);
                    if (file != 0 || rank != 0)
                    {
                        _boardBase.Mutate(i => i.Fill(GraphicsOptions.Default, brush, rect));
                    }
                }
            }

            for (var rank = 8; rank > 0; rank--)
            {
                var y = (Math.Abs(rank - 8)) * _squareWidth + _offset;
                var x = 0;
                var cRank = rank.ToString();
                var rect = new RectangleF(x, y, _squareWidth, _squareWidth);
                var center = new PointF(x + (_squareWidth / 2), y + (_squareWidth / 2));
                _boardBase.Mutate(i => i.Fill(GraphicsOptions.Default, _background, rect));
                _boardBase.Mutate(i => i.DrawText(textGraphicsOptions, center.Y.ToString(), _font, Rgba32.Black, center));
            }

            for (var file = 0; file < 9; file++)
            {

                var y = (8 * _squareWidth) + _offset;
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
