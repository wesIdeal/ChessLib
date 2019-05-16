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
using System.Threading.Tasks;

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
        private int _squareSize;
        private int _offset;
        private ImageOptions _imageOptions = new ImageOptions();

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

        public void MakeAnimationFromMoveTree(Stream writeTo, Game<MoveHashStorage> game, double positionDelayInSeconds, ImageOptions imageOpts = null, AnimatedFormat animatedFormat = AnimatedFormat.GIF)
        {
            MagickFormat format = MagickFormat.Gif;
            var initialFen = game.TagSection.ContainsKey("FEN") ? game.TagSection["FEN"] : FENHelpers.FENInitial;
            var moves = game.MoveSection;
            _imageOptions = imageOpts = imageOpts ?? new ImageOptions();
            _squareSize = imageOpts.SquareSize;
            _offset = _squareSize;
            InitPieces(imageOpts.SquareSize);
            var delay = SecondsToHundredths(positionDelayInSeconds);
            using (var boardImage = MakeBoardInstance(imageOpts, game.TagSection))
            {
                var imageList = new MagickImageCollection();
                var firstImage = MakeBoardFromFen(initialFen, boardImage, game.TagSection, null);
                firstImage.AnimationDelay = delay * 2;
                imageList.Add(firstImage);
                foreach (var move in moves)
                {
                    var positionBoard = MakeBoardFromFen(move.Move.FEN, boardImage, game.TagSection, null);
                    positionBoard.AnimationDelay = delay;
                    imageList.Add(positionBoard);
                }
                imageList.OptimizePlus();
                imageList.Write(writeTo, format);
            }
        }
        struct MoveImages
        {
            public int idx { get; set; }
            public IMagickImage image { get; set; }
        }
        public void MakeAnimationFromMoveTreeParallel(Stream writeTo, Game<MoveHashStorage> game, double positionDelayInSeconds, ImageOptions imageOpts = null, AnimatedFormat animatedFormat = AnimatedFormat.GIF)
        {
            MagickFormat format = MagickFormat.Gif;
            var initialFen = game.TagSection.ContainsKey("FEN") ? game.TagSection["FEN"] : FENHelpers.FENInitial;
            var moves = game.MoveSection;
            _imageOptions = imageOpts = imageOpts ?? new ImageOptions();
            _squareSize = imageOpts.SquareSize;
            _offset = _squareSize;
            InitPieces(imageOpts.SquareSize);
            var delay = SecondsToHundredths(positionDelayInSeconds);
            using (var boardImage = MakeBoardInstance(imageOpts, game.TagSection))
            {
                var imageList = new MagickImageCollection();
                var firstImage = MakeBoardFromFen(initialFen, boardImage, game.TagSection, null);
                firstImage.AnimationDelay = delay * 2;
                imageList.Add(firstImage);
                var images = new List<dynamic>();
                var results = Parallel.ForEach(moves.Select((x, i) => new { mv = x, idx = i }), move =>
                   {
                       var positionBoard = MakeBoardFromFen(move.mv.Move.FEN, boardImage, game.TagSection, null);
                       positionBoard.AnimationDelay = delay;
                       images.Add(new { image = positionBoard, idx = move.idx });
                   });
                imageList.AddRange(images.OrderBy(x => x.idx).Select(x => (IMagickImage)x.image));
                imageList.OptimizePlus();
                imageList.Write(writeTo, format);
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

        private IMagickImage MakeBoardInstance(ImageOptions imageOptions, Tags tags)
        {
            var bm = _boardBase.Clone();
            var boardWidth = imageOptions.SquareSize * 10;
            InitPieces(imageOptions.SquareSize);
            bm.Opaque(MagickColor.FromRgb(255, 255, 255), imageOptions.MagickLightSquareColor);
            bm.Opaque(MagickColor.FromRgb(0, 0, 0), imageOptions.MagickDarkSquareColor);
            bm.Resize(imageOptions.SquareSize * 8, imageOptions.SquareSize * 8);
            bm.Extent(boardWidth, boardWidth, Gravity.Center, imageOptions.MagickBackgroundColor);
            WriteRankAndFileLabels(ref bm, imageOptions);
            if (tags != null)
            {
                WriteGameInformation(ref bm, tags);
            }
            return bm;
        }

        public PointD UpperSquareCoordinate(int rank, int file) => UpperSquareCoordinateFromFENRank(Math.Abs(8 - rank), file);

        public PointD UpperSquareCoordinateFromFENRank(int rank, int file) => new PointD((file * _squareSize) + _offset, (rank * _squareSize) + _offset);

        public PointD LowerSquareCoordinate(int rank, int file)
        {
            var upper = UpperSquareCoordinate(rank, file);
            return LowerSquareCoordinate(upper);
        }
        public PointD LowerSquareCoordinate(PointD upperSquareCoordinate)
        {
            return new PointD(upperSquareCoordinate.X + _squareSize, upperSquareCoordinate.Y + _squareSize);
        }

        private void WriteRankAndFileLabels(ref IMagickImage image, ImageOptions imageOptions)
        {
            WriteFileLabels(ref image);
            WriteRankLabels(ref image);
        }

        private void WriteRankLabels(ref IMagickImage image)
        {
            var x = (_squareSize * 0.8);
            var rankAndFileDrawables = new Drawables();
            var fontSizePixels = _squareSize * 0.15;
            for (var rank = 0; rank < 8; rank++)
            {
                var strRank = Math.Abs(rank - 8).ToString();
                var y = (rank * _squareSize) + _offset + (_offset / 2);
                rankAndFileDrawables.FontPointSize(fontSizePixels).StrokeColor(_imageOptions.MagickTextColor).FillColor(_imageOptions.MagickTextColor)
                           .Text(x, y, strRank)
                           .TextAlignment(TextAlignment.Center);
            }
            image.Draw(rankAndFileDrawables);
        }

        private void WriteFileLabels(ref IMagickImage image)
        {
            var y = (_squareSize * 8) + _offset + (_offset * 0.2);
            var rankAndFileDrawables = new Drawables();
            var fontSizePixels = _squareSize * 0.15;
            for (var file = 0; file < 8; file++)
            {
                var strFile = ((char)('A' + file)).ToString();
                var x = (file * _squareSize) + _offset + (_squareSize / 2);
                rankAndFileDrawables.FontPointSize(fontSizePixels).StrokeColor(_imageOptions.MagickTextColor).FillColor(_imageOptions.MagickTextColor)
                           .Text(x, y, strFile)
                           .TextAlignment(TextAlignment.Center);
            }
            image.Draw(rankAndFileDrawables);
        }

        private void WriteGameInformation(ref IMagickImage image, Tags tags)
        {

            var boardWidth = _squareSize * 10;
            var nameFontSize = _squareSize * 0.2;
            if (_offset != 0)
            {
                var drawables = new Drawables();

                drawables
                    .FontPointSize(nameFontSize)
                    .TextAlignment(TextAlignment.Center)
                    .StrokeColor(_imageOptions.MagickTextColor)
                    .FillColor(_imageOptions.MagickTextColor)
                    .Font("Arial", FontStyleType.Normal, FontWeight.Normal, FontStretch.Normal)
                    .Text(boardWidth / 2, (_offset * 0.8), tags.Black)
                    .Text(boardWidth / 2, _squareSize * 10 - (_offset * 0.5), tags.White);
                //var whitePlayerNameDrawable = new DrawableText(_boardWidth / 2, _squareWidth * 10 - (_squareWidth / 2), _whitePlayerName);
                //_boardBase.Draw(blackPlayerNameDrawable, whitePlayerNameDrawable);
                image.Draw(drawables);
            }
        }

        protected IMagickImage MakeBoardFromFen(string fen, IMagickImage image, Tags tags, ushort? emptySquareIndex)
        {
            tags = tags ?? new Tags();
            using (var board = image.Clone())
            {
                var ranks = fen.GetRanksFromFen();
                PointD? emptySquare = null;
                if (emptySquareIndex.HasValue)
                    emptySquare = GetPointFromBoardIndex(emptySquareIndex.Value);

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
                            var center = UpperSquareCoordinateFromFENRank(fenRank, file);
                            if (ShouldDrawPieceInSquare(emptySquare, center))
                            {
                                PieceHelpers.GetPiece(p);
                                var pieceImage = _pieceMap[p];
                                board.Composite(pieceImage, center, CompositeOperator.SrcAtop);
                            }
                        }

                        fileCount++;
                    }
                }
                return board.Clone();
            }
        }

        public void MakeBoardFromFen(Stream writeTo, string fen, ImageOptions imageOptions = null, Tags tags = null, ImageFormat imageFormat = ImageFormat.PNG)
        {
            imageOptions = imageOptions ?? new ImageOptions();
            _imageOptions = imageOptions;
            _squareSize = imageOptions.SquareSize;
            _offset = _squareSize;

            using (var boardImage = MakeBoardInstance(imageOptions, tags))
            {
                using (var board = MakeBoardFromFen(fen, boardImage, tags, null))
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

        public PointD GetPointFromBoardIndex(ushort sq)
        {
            var x = ((sq % 8) * _squareSize) + _squareSize;
            var y = (Math.Abs((sq / 8) - 7)) * _squareSize + _squareSize;
            return new PointD(x, y);
        }

        public RectangleF GetRectFromBoardIndex(ushort squareIndex)
        {
            var p = GetPointFromBoardIndex(squareIndex);
            return new RectangleF((float)p.X, (float)p.Y + _squareSize, _squareSize, _squareSize);
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
