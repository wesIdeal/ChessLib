using ChessLib.Core;
using ChessLib.Core.Helpers;
using ChessLib.Core.Translate;
using ChessLib.Core.Types;
using ChessLib.Core.Types.Exceptions;
using EnumsNET;

namespace ChessLib.Data.Tests.Validators.MoveValidation.CastlingRules
{
    internal class CastlingTestCase
    {
        private static readonly FenTextToBoard FenReader = new FenTextToBoard();

        public CastlingTestCase(string fen, MoveError error, Move castlingMove)
        {
            Fen = fen;
            ExpectedError = error;
            CastlingMove = castlingMove;
            Board = FenReader.Translate(fen);
        }

        public string Fen { get; set; }
        public MoveError ExpectedError { get; set; }
        public Move CastlingMove { get; set; }
        public Board Board { get; }

        public override string ToString()
        {
            string move;
            if (CastlingMove.Equals(MoveHelpers.WhiteCastleKingSide) ||
                CastlingMove.Equals(MoveHelpers.BlackCastleKingSide))
            {
                move = "O-O";
            }
            else if (CastlingMove.Equals(MoveHelpers.WhiteCastleQueenSide) ||
                     CastlingMove.Equals(MoveHelpers.BlackCastleQueenSide))
            {
                move = "0-0-0";
            }
            else
            {
                move = "Non-Castling move.";
            }

            return
                $"{Board.ActivePlayer.AsString()}'s move is {move}, Expecting {ExpectedError.AsString()}\r\n\tFEN: {Fen}";
        }
    }
}