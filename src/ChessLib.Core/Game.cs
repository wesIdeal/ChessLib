using System;
using System.Collections.Generic;
using System.Linq;
using ChessLib.Core.MagicBitboard.Bitwise;
using ChessLib.Core.Translate;
using ChessLib.Core.Types;
using ChessLib.Core.Types.Enums;
using ChessLib.Core.Types.Enums.NAG;
using ChessLib.Core.Types.GameTree;

namespace ChessLib.Core
{
    public class Game : GameBuilder
    {
        private static readonly FenTextToBoard fenTextToBoard = new FenTextToBoard();
        
        public BoardNode InitialNode { get; }

        public PostMoveState[] NextMoves => Current?.Continuations.Select(x => x.Value).ToArray();
        public GameResult GameResult { get; set; }
        public string Result { get; set; }
        public int PlyCount => throw new NotImplementedException();

        public Tags Tags { get; }

        public Game() : this(BoardConstants.FenStartingPosition, new Tags())
        {
        }

        public Game(string fen, Tags tags = null)
        {
            var board = fenTextToBoard.Translate(fen);
            InitialNode = new BoardNode(board);
            Current = InitialNode;
            Tags = new Tags(fen, tags);
            ParsingLogs = new List<PgnParsingLog>();
        }


        public override void Reset()
        {
            throw new NotImplementedException();
        }


        public void AddParsingLogItem(ParsingErrorLevel errorLevel, string Message, string inputFromParser = "")
        {
            this.AddParsingLogItem(new PgnParsingLog(errorLevel,Message, inputFromParser));
        }

        public void AddParsingLogItem(PgnParsingLog logEntry) => this.ParsingLogs.Add(logEntry);
        public List<PgnParsingLog> ParsingLogs { get; set; }

        public void AddNag(NumericAnnotation nag)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        ///     Exits the current variation, traversing to it's parent variation. If node is on the main line, the current board is
        ///     set to the initial.
        /// </summary>
        public void ExitVariation()
        {
            var currentBoardStateHash = Current.Value.BoardStateHash;

            while (MovePrevious() &&
                   !Current.Variations.Select(x => x.Value.BoardStateHash).Contains(currentBoardStateHash))
            {
                currentBoardStateHash = Current.Value.BoardStateHash;
            }
        }


        public void SetComment(string comment)
        {
            throw new NotImplementedException();
        }
    }
}