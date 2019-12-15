using ChessLib.Data.Helpers;
using ChessLib.Data.MoveRepresentation;
using ChessLib.Data.Types.Enums;
using ChessLib.Data.Types.Exceptions;
using ChessLib.Graphics;
using ChessLib.Parse.PGN;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// ReSharper disable StringLiteralTypo

namespace ChessLib.Data.Tests
{

    [TestFixture]
    class MoveTraversalServiceTest
    {
        private Game<MoveStorage> _game;
        private readonly Task _imageWait;
        private Imaging _imaging;
        private static readonly object[] UnApplyTestCases = new object[]
           {
                //Ruy
                new object[]
                {
                    FENHelpers.FENInitial,
                    new[] {"e4", "e5", "Nf3", "Nc6", "Bb5", "a6", "Bxc6"},
                    "Ruy Lopez"

                },
                new object[]
                {
                    "rnbqkbnr/1pp1pppp/8/p2pP3/8/8/PPPP1PPP/RNBQKBNR w KQkq d6 0 3",
                    new[]{"exd6"},
                    "En Passant"
                },
                new object[]
                {
                "r3k2r/qpp1pppp/2nP1n2/pb3b2/2BQ4/B1N2N2/PPPP1PPP/R3K2R w KQkq - 0 3",
                new[]{"O-O", "O-O"},
                "Castling Kingside"
                },
                new object[]
                {
                    "r3k2r/qpp1pppp/2nP1n2/pb3b2/2BQ4/B1N2N2/PPPP1PPP/R3K2R w KQkq - 0 3",
                    new[]{"O-O-O", "O-O-O"},
                    "Castling Queenside"
                },
                new object[]
                {
                    "1q6/2P5/8/8/5k1K/8/4p3/3Q4 w - - 0 1",
                    new[]{"c8=N", "e1=N"},
                    "Promotion to Knight"
                },
                new object[]
                {
                    "1q6/2P5/8/8/5k1K/8/4p3/3Q4 w - - 0 1",
                    new[]{"c8=N", "e1=N"},
                    "Promotion to Bishop"
                },
                new object[]
                {
                    "1q6/2P5/8/8/5k1K/8/4p3/3Q4 w - - 0 1",
                    new[]{"c8=R", "e1=R"},
                    "Promotion to Rook"
                },
                new object[]
                {
                    "1q6/2P5/8/8/5k1K/8/4p3/3Q4 w - - 0 1",
                    new[]{"c8=Q", "e1=Q"},
                    "Promotion to Queen"
                },
                new object[]
                {
                "1q6/2P5/6k1/8/7K/8/4p3/3Q4 w - - 0 1",
                new[]{"cxb8=Q", "exd1=Q"},
                "Promotion to Queen after capture"
                }
           };

        public MoveTraversalServiceTest()
        {
            _imageWait = Task.Factory.StartNew(() => _imaging = new Imaging(new ImageOptions()
            {
                SquareSize = 70,
                DarkSquareColor = System.Drawing.Color.LightSteelBlue,
                LightSquareColor = System.Drawing.Color.WhiteSmoke
            }));
        }



        [Test(Description = "1. e4 test")]
        public void ApplyMove_ShouldReflectCorrectBoardStatusAfter_e4()
        {
            _game = new Game<MoveStorage>();
            var move = MoveHelpers.GenerateMove(12, 28);
            var expectedFEN = "rnbqkbnr/pppppppp/8/8/4P3/8/PPPP1PPP/RNBQKBNR b KQkq e3 0 1";
            _game.ApplyMove(move);
            Assert.AreEqual(expectedFEN, _game.CurrentFEN);
        }

        [Test(Description = "Ruy - applying series of moves")]
        public void ApplyMove_ShouldReflectCorrectBoardStatusAfterSeriesOfMoves()
        {
            var expectedFEN = new[]
            {
                    "rnbqkbnr/pppppppp/8/8/4P3/8/PPPP1PPP/RNBQKBNR b KQkq e3 0 1", //1. e4
                    "rnbqkbnr/pppp1ppp/8/4p3/4P3/8/PPPP1PPP/RNBQKBNR w KQkq e6 0 2", //1...e5
                    "rnbqkbnr/pppp1ppp/8/4p3/4P3/5N2/PPPP1PPP/RNBQKB1R b KQkq - 1 2", //2. Nf3
                    "r1bqkbnr/pppp1ppp/2n5/4p3/4P3/5N2/PPPP1PPP/RNBQKB1R w KQkq - 2 3", //2...Nc6
                    "r1bqkbnr/pppp1ppp/2n5/1B2p3/4P3/5N2/PPPP1PPP/RNBQK2R b KQkq - 3 3", //3. Bb5
                    "r1bqkbnr/1ppp1ppp/p1n5/1B2p3/4P3/5N2/PPPP1PPP/RNBQK2R w KQkq - 0 4", //3...a6
                    "r1bqkbnr/1ppp1ppp/p1n5/4p3/B3P3/5N2/PPPP1PPP/RNBQK2R b KQkq - 1 4", //4. Ba4
                    "r1bqkb1r/1ppp1ppp/p1n2n2/4p3/B3P3/5N2/PPPP1PPP/RNBQK2R w KQkq - 2 5", // 4...Nf6
                    "r1bqkb1r/1ppp1ppp/p1n2n2/4p3/B3P3/5N2/PPPP1PPP/RNBQ1RK1 b kq - 3 5" //O-O
                };
            var moves = new[]
            {
                    MoveHelpers.GenerateMove(12, 28),
                    MoveHelpers.GenerateMove(52, 36),
                    MoveHelpers.GenerateMove(6, 21),
                    MoveHelpers.GenerateMove(57, 42),
                    MoveHelpers.GenerateMove(5, 33),
                    MoveHelpers.GenerateMove(48, 40),
                    MoveHelpers.GenerateMove(33, 24),
                    MoveHelpers.GenerateMove(62, 45),
                    MoveHelpers.GenerateMove(4, 6, MoveType.Castle)
                };
            Assert.AreEqual(expectedFEN.Length, moves.Length);
            _game = new Game<MoveStorage>();
            for (var i = 0; i < moves.Length; i++)
            {
                Debug.WriteLine($"Applying move {moves[i]}");
                var expected = expectedFEN[i];
                _game.ApplyMove(moves[i]);
                Assert.AreEqual(expected, _game.CurrentFEN);
            }
        }

        [Test, TestCaseSource(nameof(UnApplyTestCases))]
        public void UnapplyMove(string fenStart, string[] moves, string description)
        {
            var game = new Game<MoveStorage>(fenStart);
            var stateStack = new Stack<string>();
            int index;
            for (index = 0; index < moves.Length; index++)
            {
                var move = moves[index];
                var moveTranslator = new MoveTranslatorService(game.CurrentFEN);
                var moveExt = moveTranslator.GetMoveFromSAN(move);
                stateStack.Push(game.CurrentFEN);
                game.ApplyMove(moveExt);
            }

            for (; index > 0; index--)
            {
                var expectedState = stateStack.Pop();
                Assert.AreNotEqual(game.CurrentFEN, expectedState, $"{description}: expected state should not equal current state.");
                game.TraverseBackward();
                Assert.AreEqual(expectedState, game.CurrentFEN, $"{description}: current state not equal to the expected state after undoing move {index}.");
            }
        }

        [Test]
        public void FindNextMoves_ShouldReturnVariationsInOrder()
        {
            var game = LoadGameByPGN(PGN.WithVariations);
            var move = game.GetNextMoves().FirstOrDefault();
            game.TraverseForward(move);
            var moves = game.GetNextMoves();
            Assert.AreEqual(4, moves.Length);
            Assert.AreEqual(4013, moves[0].Move);
            Assert.AreEqual(3364, moves[1].Move);
            Assert.AreEqual(3372, moves[2].Move);
            Assert.AreEqual(3234, moves[3].Move);
        }

        [Test]
        public void TraversePGN()
        {
            var sb = new StringBuilder();
            var game = LoadGameByPGN(PGN.Fischer01);
            var stateStack = new Stack<(MoveStorage moveApplied, string premoveFEN)>();
            var moves = game.MainMoveTree;
            sb.AppendLine($"****APPLYING {game.MainMoveTree.Count} MOVES****");
            foreach (var move in moves.Skip(1))
            {
                var premoveFEN = game.Board.CurrentFEN;
                stateStack.Push((moveApplied: move, premoveFEN: premoveFEN));
                game.TraverseForward(move);
                var postMoveFEN = game.Board.CurrentFEN;
                sb.AppendLine($"\t{move}\t{premoveFEN}->{postMoveFEN}");
            }

            sb.AppendLine("****UNAPPLYING MOVES****");
            while (stateStack.TryPop(out var expectedState))
            {
                var moveUnapplied = expectedState.moveApplied;
                game.TraverseBackward();
                try
                {
                    MoveDisplayService mts = new MoveDisplayService(expectedState.premoveFEN);
                    Assert.AreEqual(expectedState.premoveFEN, game.CurrentFEN,
                        $"Current state != to expected after undoing {mts.MoveToSAN(moveUnapplied)}.\r\nMove applied to FEN {expectedState.premoveFEN}");


                }
                catch
                {
                    Console.WriteLine(sb.ToString());
                    throw;
                }
                Debug.WriteLine($"Unapplied {moveUnapplied} successfully.");
            }
            Console.WriteLine(sb.ToString());
        }

        [Test]
        public void FindNextMoves_ShouldReturnEmptyCollectionIfTheEndIsReached()
        {
            _game = LoadGameByPGN(PGN.Fischer01);

            _game.GoToLastMove();
            var moves = _game.GetNextMoves();
            Assert.IsEmpty(moves);
        }

        [Test]
        public void TraversingBackwardOnFirstMoveShouldReturnInitialBoard()
        {
            _game = new Game<MoveStorage>();
            _game.TraverseBackward();
            for (int i = 0; i < 25; i++)
            {
                _game.TraverseBackward();
                Assert.AreEqual(FENHelpers.FENInitial, _game.CurrentFEN);
            }
        }

        [Test]
        public void TraversingForwardWithEmptyTreeShouldReturnInitialBoard()
        {
            var game = new Game<MoveStorage>();

            for (int i = 0; i < 25; i++)
            {
                game.TraverseForward();
                Assert.AreEqual(game.InitialFen, game.CurrentFEN);
            }
        }

        [Test]
        public void TraverseBackward_OutOfVariation_ShouldReturnCorrectMove()
        {
            var game = LoadGameByPGN(PGN.ShortVariation);
            MoveStorage[] moves;
            MoveStorage move = null;
            while ((moves = game.GetNextMoves()).Length == 1)
            {
                move = moves[0];
                game.TraverseForward(move);
            }
            game.TraverseForward(moves[1]);
            game.TraverseBackward();
            Assert.AreEqual(move, game.CurrentMoveNode.Value);
        }

        [TestCase(nameof(PGN.Puzzle), "r1b2rk1/pp1p1pp1/1b1p2B1/n1qQ2p1/8/5N2/P3RPPP/4R1K1 w - - 0 1")]
        [TestCase(nameof(PGN.Fischer01), "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1")]
        public void GoToInitialState_ShouldApplyInitialState(string nameOfPgn, string expected)
        {
            try
            {
                _game = LoadGameIntoBoardByName(nameOfPgn);
                _game.GoToLastMove();
                _game.GoToInitialState();
            }
            catch (MoveException moveExc)
            {
                HandleMoveException(moveExc);
                throw;
            }
            Assert.IsTrue(_game.CurrentMoveNode.Value.IsNullMove, "Current move should be null move");
            Assert.IsNull(_game.CurrentTree.VariationParentNode, "Should be on top tree after initial applied.");
            Assert.AreEqual(expected, _game.CurrentFEN);
        }

        private void HandleMoveException(MoveException moveExc)
        {
            if (moveExc.Board == null) return;
            _imageWait.Wait();
            var path = Path.Combine(Path.GetTempPath(), "ChessLib.png");
            using (var st = System.IO.File.Create(path))
            {
                _imaging.MakeBoardFromFen(st, moveExc.Board.ToFEN());
                st.Flush();
            }

            Process viewImage = new Process();
            var winDir = Environment.GetEnvironmentVariable("windir");
            viewImage.StartInfo.FileName = Path.Combine(winDir, "system32", "mspaint.exe");
            viewImage.StartInfo.Arguments = path;
            viewImage.Start();
        }

        [TestCase(nameof(PGN.Puzzle), "r1b1Rk2/pp1p2p1/1b1p2B1/n1q3p1/8/5N2/P4PPP/6K1 b - - 1 4")]
        [TestCase(nameof(PGN.Fischer01), "8/2b5/8/p5R1/P1p1P2p/4kP2/1P4K1/8 b - - 0 52")]
        public void GoToLastMove_ShouldApplyLastMove(string nameOfPgn, string expected)
        {
            var game = LoadGameIntoBoardByName(nameOfPgn);
            game.GoToLastMove();
            Assert.AreEqual(expected, game.CurrentFEN);
        }

        private Game<MoveStorage> LoadGameByPGN(string pgn)
        {
            var parser = new PGNParser();
            var game = parser.GetGamesFromPGNAsync(pgn).Result.First();
            return game;
        }

        private Game<MoveStorage> LoadGameIntoBoardByName(string name)
        {
            var data = PGN.ResourceManager.GetString(name);
            return LoadGameByPGN(data);
        }

        [Test]
        public void GoToLastMoveShouldReturnLastMove()
        {
            var pgn = PGN.WithVariations;
            var parser = new PGNParser();
            var game = parser.GetGamesFromPGNAsync(pgn).Result.First();
            game.GoToLastMove();
            Assert.AreEqual("Nc3", game.CurrentMoveNode.Value.SAN);
        }
        [Test]
        public void GoToInitialStateShouldGoToFirstMoveFromMainLine()
        {
            var pgn = PGN.WithVariations;
            var parser = new PGNParser();
            var game = parser.GetGamesFromPGNAsync(pgn).Result.First();
            game.GoToLastMove();
            game.GoToInitialState();
            Assert.IsTrue(game.CurrentMoveNode.Value.IsNullMove);
        }

        [Test]
        public void ExitVariationShouldGoToMainMove()
        {
            const string variationParentMove = "e5";
            var mSvc = new MoveTraversalService(FENHelpers.FENInitial);
            mSvc.ApplySanMove("c4", MoveApplicationStrategy.ContinueMainLine);
            mSvc.ApplySanMove(variationParentMove, MoveApplicationStrategy.ContinueMainLine);
            mSvc.ApplySanMove("Nf6", MoveApplicationStrategy.Variation);
            mSvc.ExitVariation();
            var currentMoveSan = mSvc.CurrentMoveNode.Value.SAN;
            Assert.AreEqual(variationParentMove, currentMoveSan,
                $"Expected e5, but current node was {currentMoveSan}.");
        }
    }

}
