using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ChessLib.Core;
using ChessLib.Core.Helpers;
using ChessLib.Core.MagicBitboard.Bitwise;
using ChessLib.Core.Types;
using ImageMagick;

namespace ChessLib.Graphics
{
    public enum ImageFormat
    {
        GIF,
        PNG,
        JPG
    }

    public enum AnimatedFormat
    {
        GIF
    }

    public class Imaging : IDisposable
    {
        private MagickImage _boardBase;
        private ImageOptions _imageOptions = new ImageOptions();
        private int _offset;
        private Dictionary<char, IMagickImage> _pieceMap;
        private int _squareSize;

        public Imaging()
        {
            _imageOptions = new ImageOptions();
            var assembly = Assembly.GetExecutingAssembly();
            var names = assembly.GetManifestResourceNames();
            SetBoardBaseImage();
        }

        public Imaging(ImageOptions imageOptions)
        {
            _imageOptions = imageOptions;
            var assembly = Assembly.GetExecutingAssembly();
            var names = assembly.GetManifestResourceNames();
            SetBoardBaseImage();
        }

        public bool IsDisposed { get; private set; }

        public void Dispose()
        {
            if (!_boardBase.IsDisposed)
            {
                _boardBase.Dispose();
            }

            if (_pieceMap != null)
            {
                foreach (var p in _pieceMap)
                {
                    if (!p.Value.IsDisposed)
                    {
                        p.Value.Dispose();
                    }
                }
            }

            IsDisposed = true;
        }

        private void SetBoardBaseImage()
        {
            var assembly = Assembly.GetExecutingAssembly();
            using (var resources = assembly.GetManifestResourceStream("ChessLib.Graphics.Images.Board.svg"))
            {
                var readSettings = new MagickReadSettings {Format = MagickFormat.Svg};
                using (var bm = new MagickImage(resources, readSettings))
                {
                    _boardBase = (MagickImage) bm.Clone();
                }
            }
        }

        ~Imaging()
        {
            Dispose();
        }

        private MagickColor SquareColor(int rank, int file)
        {
            return (rank + file) % 2 == 1 ? _imageOptions.MagickLightSquareColor : _imageOptions.MagickDarkSquareColor;
        }

        public Dictionary<PieceOfColor, IMagickImage> GetPieceTypeDictionaryOfSize(int size)
        {
            var pieces = GetPieceDictionaryOfSize(size);
            var dict = new Dictionary<PieceOfColor, IMagickImage>();
            foreach (var p in pieces)
            {
                dict.Add(PieceHelpers.GetPieceOfColor(p.Key), p.Value.Clone());
                p.Value.Dispose();
            }

            return dict;
        }

        public Dictionary<char, IMagickImage> GetPieceDictionaryOfSize(int size)
        {
            var assembly = Assembly.GetExecutingAssembly();
            using (var resources = assembly.GetManifestResourceStream("ChessLib.Graphics.Images.pieceArray.svg"))
            {
                var readSettings = new MagickReadSettings
                {
                    BackgroundColor = MagickColors.Transparent
                };
                using (var bm = new MagickImage(resources, readSettings))
                {
                    var pieces = new List<IMagickImage>();
                    var colorFrom = new MagickColor(0, 0, 0);
                    var colorTo = _imageOptions.BlackPieceColor;
                    var alterPieceColor = _imageOptions.MagickBlackPieceColor != MagickColor.FromRgb(0, 0, 0) ||
                                          _imageOptions.MagickWhitePieceColor != MagickColor.FromRgb(255, 255, 255);
                    for (var row = 0; row < 2; row++)
                    {
                        for (var col = 0; col < 6; col++)
                        {
                            var x = col * 60;
                            var y = row * 60;
                            var piece = bm.Clone(new MagickGeometry(x, y, 60, 60));
                            piece.Resize(size, size);
                            piece.Alpha(AlphaOption.Set);
                            pieces.Add(piece.Clone());
                            piece.Dispose();
                        }
                    }

                    var pieceMap = new Dictionary<char, IMagickImage>
                    {
                        {'k', pieces[0].Clone()},
                        {'q', pieces[1].Clone()},
                        {'r', pieces[2].Clone()},
                        {'b', pieces[3].Clone()},
                        {'n', pieces[4].Clone()},
                        {'p', pieces[5].Clone()},
                        {'K', pieces[6].Clone()},
                        {'Q', pieces[7].Clone()},
                        {'R', pieces[8].Clone()},
                        {'B', pieces[9].Clone()},
                        {'N', pieces[10].Clone()},
                        {'P', pieces[11].Clone()}
                    };
                    foreach (var piece in pieces)
                    {
                        piece.Dispose();
                    }

                    return pieceMap;
                }
            }
        }

        private void InitPieces(int squareWidth)
        {
            _pieceMap = GetPieceDictionaryOfSize(squareWidth);
        }

        private static IMagickImage ChangeColor(in IMagickImage orig, MagickColor from, MagickColor to)
        {
            orig.ColorFuzz = new Percentage(80);
            orig.Opaque(from, to);
            return orig;
        }

        private static int SecondsToHundredths(double seconds)
        {
            return (int) Math.Round(seconds * 100);
        }

        public void MakeAnimationFromMoveTree(Stream writeTo, Game game, double positionDelayInSeconds,
            ImageOptions imageOpts = null, AnimatedFormat animatedFormat = AnimatedFormat.GIF)
        {
            var format = MagickFormat.Gif;
            var initialFen = game.TagSection.ContainsKey("FEN")
                ? game.TagSection["FEN"]
                : BoardConstants.FenStartingPosition;
            var moves = game.MainMoveTree;
            _imageOptions = imageOpts = imageOpts ?? new ImageOptions();
            _squareSize = imageOpts.SquareSize;
            _offset = _squareSize;
            InitPieces(imageOpts.SquareSize);
            var delay = SecondsToHundredths(positionDelayInSeconds);
            using (var boardImage = MakeBoardInstance(imageOpts, game.TagSection))
            {
                using (var imageList = new MagickImageCollection())
                {
                    using (var firstImage = MakeBoardFromFen(initialFen, boardImage, game.TagSection, null))
                    {
                        firstImage.AnimationDelay = delay * 2;
                        imageList.Add(firstImage);
                        var images = new List<MoveImages>();

                        var results = Parallel.ForEach(moves.Select((x, i) => new {mv = x, idx = i}), move =>
                        {
                            //using (var positionBoard = MakeBoardFromFen(move.mv.PremoveFEN, boardImage, game.TagSection, null))
                            //{
                            //    positionBoard.AnimationDelay = delay;
                            //    images.Add(new MoveImages { Image = positionBoard, Index = move.idx });
                            //}
                        });
                        imageList.AddRange(images.OrderBy(i => i.Index).Select(x => x.Image));
                        imageList.OptimizePlus();
                        imageList.Write(writeTo, format);
                        images.ForEach(i => { i.Image.Dispose(); });
                    }
                }
            }
        }


        protected static MagickFormat GetMagickFormatFromFormat(ImageFormat format)
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

        public PointD UpperSquareCoordinate(int rank, int file)
        {
            return UpperSquareCoordinateFromFENRank(Math.Abs(8 - rank), file);
        }

        public PointD UpperSquareCoordinateFromFENRank(int rank, int file)
        {
            return new PointD(file * _squareSize + _offset, rank * _squareSize + _offset);
        }

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
            var x = _squareSize * 0.8;
            var rankAndFileDrawables = new Drawables();
            var fontSizePixels = _squareSize * 0.15;
            for (var rank = 0; rank < 8; rank++)
            {
                var strRank = Math.Abs(rank - 8).ToString();
                var y = rank * _squareSize + _offset + _offset / 2;
                rankAndFileDrawables.FontPointSize(fontSizePixels).StrokeColor(_imageOptions.MagickTextColor)
                    .FillColor(_imageOptions.MagickTextColor)
                    .Text(x, y, strRank)
                    .TextAlignment(TextAlignment.Center);
            }

            image.Draw(rankAndFileDrawables);
        }

        private void WriteFileLabels(ref IMagickImage image)
        {
            var y = _squareSize * 8 + _offset + _offset * 0.2;
            var rankAndFileDrawables = new Drawables();
            var fontSizePixels = _squareSize * 0.15;
            for (var file = 0; file < 8; file++)
            {
                var strFile = ((char) ('A' + file)).ToString();
                var x = file * _squareSize + _offset + _squareSize / 2;
                rankAndFileDrawables.FontPointSize(fontSizePixels).StrokeColor(_imageOptions.MagickTextColor)
                    .FillColor(_imageOptions.MagickTextColor)
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
                    .Text(boardWidth / 2, _offset * 0.8, tags.Black)
                    .Text(boardWidth / 2, _squareSize * 10 - _offset * 0.5, tags.White);
                //var whitePlayerNameDrawable = new DrawableText(_boardWidth / 2, _squareWidth * 10 - (_squareWidth / 2), _whitePlayerName);
                //_boardBase.Draw(blackPlayerNameDrawable, whitePlayerNameDrawable);
                image.Draw(drawables);
            }
        }

        private IMagickImage MakeBoardFromFen(string fen, IMagickImage image, Tags tags, ushort? emptySquareIndex)
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


        public void MakeBoardFromFen(Stream writeTo, string fen, ImageOptions imageOptions = null, Tags tags = null,
            ImageFormat imageFormat = ImageFormat.PNG)
        {
            imageOptions = imageOptions ?? _imageOptions;
            _imageOptions = imageOptions;
            _squareSize = imageOptions.SquareSize;
            _offset = _squareSize;

            using (var boardImage = MakeBoardInstance(imageOptions ?? _imageOptions, tags))
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


        private static PointD CenterOfRectangle(double upperX, double upperY, double lowerX, double lowerY)
        {
            var centerX = (upperX + lowerX) / 2;
            var centerY = (upperY + lowerY) / 2;
            return new PointD(centerX, centerY);
        }

        public PointD GetPointFromBoardIndex(ushort sq)
        {
            var x = sq % 8 * _squareSize + _squareSize;
            var y = Math.Abs(sq / 8 - 7) * _squareSize + _squareSize;
            return new PointD(x, y);
        }

        public RectangleF GetRectFromBoardIndex(ushort squareIndex)
        {
            var p = GetPointFromBoardIndex(squareIndex);
            return new RectangleF((float) p.X, (float) p.Y + _squareSize, _squareSize, _squareSize);
        }

        private struct MoveImages
        {
            public int Index { get; set; }
            public IMagickImage Image { get; set; }
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