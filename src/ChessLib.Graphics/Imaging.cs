using ChessLib.Data;
using ChessLib.Data.Helpers;
using ChessLib.Data.MoveRepresentation;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using ChessLib.Data.Types;
using ImageMagick;
using File = System.IO.File;
using System.Drawing;
using System.Net.Mime;
using SixLabors.Fonts;
using System.IO;

namespace ChessLib.Graphics
{
    public enum ImageFormat
    {
        GIF, PNG, JPG
    }
    public enum AnimatedFormat
    {
        GIF
    }
    public class Imaging
    {

        private MagickImage _boardBase;
        private Dictionary<char, IMagickImage> _pieceMap;
        private readonly int _offset;
        private readonly ImageOptions _imageOptions = new ImageOptions();

        public Imaging()
        {

            _imageOptions = new ImageOptions();
            var assembly = Assembly.GetExecutingAssembly();
            var names = assembly.GetManifestResourceNames();
            SetBoardBaseImage();
        }

        private void SetBoardBaseImage()
        {
            var assembly = Assembly.GetExecutingAssembly();
            using (var resources = assembly.GetManifestResourceStream("ChessLib.Graphics.Images.Board.svg"))
            {
                MagickReadSettings readSettings = new MagickReadSettings { Format = MagickFormat.Svg };
                using (var bm = new MagickImage(resources, readSettings))
                {
                    _boardBase = (MagickImage)bm.Clone();
                }
            }
        }

        ~Imaging()
        {
            _boardBase.Dispose();
            foreach (var p in _pieceMap)
            {
                p.Value.Dispose();
            }
        }
        private MagickColor SquareColor(int rank, int file) => ((rank + file) % 2) == 1 ? _imageOptions.MagickLightSquareColor : _imageOptions.MagickDarkSquareColor;

        private void InitPieces(int squareWidth)
        {
            var assembly = Assembly.GetExecutingAssembly();
            using (var resources = assembly.GetManifestResourceStream("ChessLib.Graphics.Images.pieceArray.svg"))
            {
                var readSettings = new MagickReadSettings
                {
                    BackgroundColor = MagickColors.Transparent
                };
                var bm = new MagickImage(resources, readSettings);
                var pieces = new List<IMagickImage>();
                var colorFrom = new MagickColor(0, 0, 0);
                var colorTo = _imageOptions.BlackPieceColor;
                var alterPieceColor = _imageOptions.MagickBlackPieceColor != MagickColor.FromRgb(0, 0, 0) || _imageOptions.MagickWhitePieceColor != MagickColor.FromRgb(255, 255, 255);

                for (var row = 0; row < 2; row++)
                {
                    for (var col = 0; col < 6; col++)
                    {
                        var x = col * 60;
                        var y = row * 60;
                        var piece = bm.Clone(new MagickGeometry(x, y, 60, 60));
                        piece.Resize(squareWidth, squareWidth);
                        piece.Alpha(AlphaOption.Set);
                        pieces.Add(piece);
                    }
                }

                _pieceMap = new Dictionary<char, IMagickImage>
                {
                    {'k', pieces[0]},
                    {'q', pieces[1]},
                    {'r', pieces[2]},
                    {'b', pieces[3]},
                    {'n', pieces[4]},
                    {'p', pieces[5]},
                    {'K', pieces[6]},
                    {'Q', pieces[7]},
                    {'R', pieces[8]},
                    {'B', pieces[9]},
                    {'N', pieces[10]},
                    {'P', pieces[11]}
                };
            }
        }

        private IMagickImage ChangeColor(in IMagickImage orig, MagickColor from, MagickColor to)
        {
            orig.ColorFuzz = new Percentage(80);
            orig.Opaque(from, to);
            return orig;
        }

        private int SecondsToHundredths(double seconds) => (int)Math.Round(seconds * 100);

        public void MakeAnimationFromMoveTree(Stream writeTo, MoveTree<MoveHashStorage> moves, string initialFenPosition, double positionDelayInSeconds, ImageOptions imageOpts = null, AnimatedFormat animatedFormat = AnimatedFormat.GIF)
        {
            MagickFormat format = MagickFormat.Gif;
            imageOpts = imageOpts ?? _imageOptions;
            InitPieces(imageOpts.SquareSize);
            var delay = SecondsToHundredths(positionDelayInSeconds);
            using (var boardImage = MakeBoardInstance(imageOpts))
            {
                var imageList = new List<IMagickImage>
                    {
                        MakeBoardFromFen(initialFenPosition, boardImage, imageOpts)
                    };

                imageList.Last().AnimationDelay = delay;
                foreach (var move in moves)
                {
                    var positionBoard = MakeBoardFromFen(move.Move.FEN, boardImage, imageOpts);
                    positionBoard.AnimationDelay = delay;
                    imageList.Add(positionBoard);
                }
                using (var board = new MagickImageCollection(imageList))
                {
                    board.Write(writeTo, format);
                }
                imageList.ForEach(x => x.Dispose());
            }
        }

        //private void MakeAnimationFromMoveTree(IEnumerable<MoveHashStorage> moves, string initialFenPosition, string fileName, double positionDelayInSeconds, int numberOfMovementFrames = 2, double totalMovementTimeInSeconds = 0.10)
        //{
        //    using (var board = new MagickImageCollection(new[] { MakeBoardFromFen(initialFenPosition) }))
        //    {
        //        board[0].AnimationDelay = SecondsToHundredths(positionDelayInSeconds);

        //        var previousFEN = moves.FENStart;
        //        var movementTimeHudredthsOfSecond = SecondsToHundredths(totalMovementTimeInSeconds);

        //        foreach (var mv in moves)
        //        {
        //            var pieceMoving =
        //                _pieceMap[PieceHelpers.GetCharRepresentation(mv.Move.ColorMoving, mv.Move.PieceMoving)];

        //            if (numberOfMovementFrames > 0)
        //            {
        //                var frameDelay = movementTimeHudredthsOfSecond / numberOfMovementFrames;
        //                var transImages = MakeMovementFrames(previousFEN, (MagickImage)pieceMoving, mv.Move.Move.SourceIndex(), mv.Move.Move.DestinationIndex(), numberOfMovementFrames);
        //                foreach (var f in transImages)
        //                {

        //                    f.AnimationDelay = (int)frameDelay;
        //                    board.Add(f.Clone());
        //                    f.Dispose();

        //                }
        //            }
        //            var finalBoard = MakeBoardFromFen(mv.Move.FEN);
        //            finalBoard.AnimationDelay = SecondsToHundredths(positionDelayInSeconds);

        //            //finalBoard.Quantize(new QuantizeSettings() { Colors = _imageOptions.GetPalette().Length });
        //            board.Add(finalBoard);
        //            previousFEN = mv.Move.FEN;
        //        }

        //        board.OptimizePlus();
        //        board.Write(fileName);
        //    }
        //}

        //private IEnumerable<IMagickImage> MakeMovementFrames(string fen, MagickImage pieceMoving,
        //    ushort sqFrom, ushort sqTo, int frames)
        //{
        //    var points = new PointD[frames];
        //    var rv = new List<IMagickImage>();
        //    var pFrom = GetPointFromBoardIndex(sqFrom);
        //    var pTo = GetPointFromBoardIndex(sqTo);

        //    var rise = pTo.Y - pFrom.Y;
        //    var run = pTo.X - pFrom.X;

        //    var riseSegment = rise / (frames + 2);
        //    var runSegment = run / (frames + 2);

        //    for (int i = 0; i < frames; i++)
        //    {
        //        var riseToY = pFrom.Y + (riseSegment * (i + 1));
        //        var pointInLine = new PointD(pFrom.X + (runSegment * (i + 1)), riseToY);
        //        points[i] = pointInLine;
        //    }

        //    var moveNumber = FENHelpers.GetMoveNumberFromString(fen.GetFENPiece(FENPieces.FullMoveCounter));
        //    var activeSide = FENHelpers.GetActiveColor(fen.GetFENPiece(FENPieces.ActiveColor));


        //    using (var baseImage = MakeBoardFromFen(fen, sqFrom))
        //    {

        //        foreach (var point in points)
        //        {
        //            using (var tImage = baseImage.Clone())
        //            {

        //                var drawPoint = new PointD((int)point.X, (int)point.Y);
        //                var newImage = tImage.Clone();
        //                newImage.Composite(pieceMoving, drawPoint, CompositeOperator.SrcAtop);
        //                rv.Add(newImage);
        //            }
        //        }
        //    }
        //    return rv.ToArray();
        //}





        protected MagickFormat GetMagickFormatFromFormat(ImageFormat format)
        {
            switch (format)
            {
                case ImageFormat.GIF:
                    return MagickFormat.Gif;
                case ImageFormat.JPG:
                    return MagickFormat.Jpg;
                default:
                    return MagickFormat.Png;
            }
        }

        private IMagickImage MakeBoardInstance(ImageOptions imageOptions)
        {
            var bm = _boardBase.Clone();
            var boardWidth = imageOptions.SquareSize * 10;
            InitPieces(imageOptions.SquareSize);
            bm.Opaque(MagickColor.FromRgb(255, 255, 255), imageOptions.MagickLightSquareColor);
            bm.Opaque(MagickColor.FromRgb(0, 0, 0), imageOptions.MagickDarkSquareColor);
            bm.Resize(imageOptions.SquareSize * 8, imageOptions.SquareSize * 8);
            bm.Extent(boardWidth, boardWidth, Gravity.Center, imageOptions.MagickBackgroundColor);
            WriteRankAndFileLabels(ref bm, imageOptions);
            return bm;
        }

        private void WriteRankAndFileLabels(ref IMagickImage image, ImageOptions imageOptions)
        {
            var width = imageOptions.SquareSize;
            var textColor = imageOptions.MagickTextColor;
            var rankAndFileDrawables = new Drawables();
            var fontSizePixels = width * 0.15;
            for (var rank = 9; rank >= 0; rank--)
            {
                for (var file = 0; file <= 8; file++)
                {
                    var strFile = ((char)('A' + (file - 1))).ToString();
                    var strRank = rank.ToString();
                    var upper = imageOptions.UpperSquareCoordinate(rank, file);
                    var lower = imageOptions.LowerSquareCoordinate(upper);
                    var center = CenterOfRectangle(upper.X, upper.Y, lower.X, lower.Y);

                    if (rank == 0 && file > 0)
                    {
                        var topOfSquare = (width * 0.2);

                        rankAndFileDrawables.FontPointSize(fontSizePixels).StrokeColor(textColor).FillColor(textColor)
                            .Text(center.X, upper.Y + topOfSquare, strFile).TextAlignment(TextAlignment.Center);
                    }
                    // write rank
                    else if (file == 0 && rank != 0 && rank != 9)
                    {
                        var rightOfSquare = (width * 0.8);

                        rankAndFileDrawables.FontPointSize(fontSizePixels).StrokeColor(textColor).FillColor(textColor)
                            .Text(upper.X + rightOfSquare, center.Y, strRank)
                            .TextAlignment(TextAlignment.Center);
                    }
                }

            }
            image.Draw(rankAndFileDrawables);
        }

        private void WriteGameInformation(ref IMagickImage image, ImageOptions imageOptions, Tags tags)
        {
            var width = imageOptions.SquareSize;
            var boardWidth = width * 10;
            var nameFontSize = width * 0.2;
            if (_offset != 0)
            {
                var drawables = new Drawables();

                drawables
                    .FontPointSize(nameFontSize)
                    .TextAlignment(TextAlignment.Center)
                    .StrokeColor(_imageOptions.MagickTextColor)
                    .FillColor(_imageOptions.MagickTextColor)
                    .Font("Arial", FontStyleType.Normal, FontWeight.Normal, FontStretch.Normal)
                    .Text(boardWidth / 2, width / 2, tags.White)
                    .Text(boardWidth / 2, width * 10 - (width / 2), tags.Black);
                //var whitePlayerNameDrawable = new DrawableText(_boardWidth / 2, _squareWidth * 10 - (_squareWidth / 2), _whitePlayerName);
                //_boardBase.Draw(blackPlayerNameDrawable, whitePlayerNameDrawable);
                image.Draw(drawables);
            }
        }

        protected IMagickImage MakeBoardFromFen(string fen, IMagickImage image, ImageOptions imageOptions, Tags tags = null, ushort? emptySquareIndex = null)
        {
            tags = tags ?? new Tags();
            using (var board = image.Clone())
            {
                var width = imageOptions.SquareSize;
                var ranks = fen.GetRanksFromFen();
                PointD? emptySquare = null;
                if (emptySquareIndex.HasValue)
                    emptySquare = imageOptions.GetPointFromBoardIndex(emptySquareIndex.Value);

                for (var fenRank = 0; fenRank < ranks.Length; fenRank++)
                {
                    var rank = ranks[fenRank];
                    var fileCount = 0;
                    for (var file = 0; file < 8; file++)
                    {
                        var x = (file * width) + width;
                        var y = (fenRank * width) + _offset;
                        var p = rank[fileCount];
                        if (char.IsDigit(p))
                        {
                            var digit = int.Parse(p.ToString());
                            file += digit - 1;
                        }
                        else
                        {
                            PieceHelpers.GetPiece(p);
                            var center = new PointD(x, y);
                            if (ShouldDrawPieceInSquare(emptySquare, center))
                            {
                                var pieceImage = _pieceMap[p];
                                board.Composite(pieceImage, new PointD(x, y), CompositeOperator.SrcAtop);
                            }
                        }

                        fileCount++;
                    }
                }
                return board.Clone();
            }
        }

        public void MakeBoardFromFen(Stream writeTo, string fen, ImageOptions imageOptions = null, ImageFormat imageFormat = ImageFormat.PNG)
        {
            imageOptions = imageOptions ?? _imageOptions;
            using (var boardImage = MakeBoardInstance(imageOptions))
            {
                using (var board = MakeBoardFromFen(fen, boardImage, imageOptions, null))
                {
                    board.Write(writeTo, GetMagickFormatFromFormat(imageFormat));
                }
            }
        }

        private static bool ShouldDrawPieceInSquare(PointD? emptySquare, PointD currentSquare)
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



        private PointD CenterOfRectangle(double upperX, double upperY, double lowerX, double lowerY)
        {
            var centerX = (upperX + lowerX) / 2;
            var centerY = (upperY + lowerY) / 2;
            return new PointD(centerX, centerY);
        }



        //private void SetBoardBaseImage()
        //{
        //    _boardBase = new MagickImage(_imageOptions.MagickBackgroundColor, _boardWidth, _boardWidth);
        //    var textColor = _imageOptions.MagickTextColor;
        //    var nameFontSize = _squareWidth * 0.25;
        //    if (_offset != 0)
        //    {
        //        var drawables = new Drawables();

        //        drawables
        //            .FontPointSize(nameFontSize)
        //            .TextAlignment(TextAlignment.Center)
        //            .StrokeColor(textColor)
        //            .FillColor(textColor)
        //            .Text(_boardWidth / 2, _squareWidth / 2, _blackPlayerName)
        //            .Text(_boardWidth / 2, _squareWidth * 10 - (_squareWidth / 2), _blackPlayerName);
        //        //var whitePlayerNameDrawable = new DrawableText(_boardWidth / 2, _squareWidth * 10 - (_squareWidth / 2), _whitePlayerName);
        //        //_boardBase.Draw(blackPlayerNameDrawable, whitePlayerNameDrawable);
        //        _boardBase.Draw(drawables);
        //    }
        //    var listOfSquares = new Drawables();
        //    for (var rank = 9; rank >= 0; rank--)
        //    {
        //        for (var file = 0; file <= 8; file++)
        //        {
        //            var strFile = ((char)('A' + (file - 1))).ToString();
        //            var strRank = rank.ToString();
        //            var upper = UpperSquareCoordinate(rank, file);
        //            var lower = LowerSquareCoordinate(upper);
        //            var squareColor = SquareColor(rank, file);
        //            var center = CenterOfRectangle(upper.X, upper.Y, lower.X, lower.Y);
        //            if (file > 0 && rank > 0 && rank < 9)
        //            {
        //                listOfSquares.StrokeColor(squareColor).FillColor(squareColor).Rectangle(upper.X, upper.Y, lower.X, lower.Y);
        //            }
        //            // write file
        //            else if (rank == 0 && file > 0)
        //            {
        //                var topOfSquare = (_squareWidth * 0.2);

        //                listOfSquares.StrokeColor(textColor).FillColor(textColor).Text(center.X, upper.Y + topOfSquare, strFile).TextAlignment(TextAlignment.Center);
        //            }
        //            // write rank
        //            else if (file == 0 && rank != 0 && rank != 9)
        //            {
        //                var rightOfSquare = (_squareWidth * 0.8);

        //                listOfSquares.StrokeColor(textColor).FillColor(textColor).Text(upper.X + rightOfSquare, center.Y, strRank).TextAlignment(TextAlignment.Center);
        //            }
        //        }
        //    }
        //    _boardBase.Draw(listOfSquares);
        //}
    }
}
