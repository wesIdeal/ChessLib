using ChessLib.Data.PieceMobility;
using ChessLib.Data.Types.Enums;

namespace ChessLib.Data.Magic.Init
{
    internal class RookPatterns : MovePatternStorage
    {
        public RookPatterns()
        {
            Initialize(PieceAttackPatterns.Instance.RookAttackMask, new RookMovesInitializer());
        }
    }

    internal class WhitePawnPatterns : MovePatternStorage
    {
        public WhitePawnPatterns()
        {
            var pawnAttacks = PieceAttackPatterns.Instance.PawnAttackMask[0];
            var pawnMoves = PieceAttackPatterns.Instance.PawnMoveMask[0];
            var mb = new MoveBoard();
            for (var i = 0; i < 64; i++)
            {
                mb[i] = pawnAttacks[i] | pawnMoves[i];
            }
            InitializePawns(mb, new PawnMovesInitializer(Color.White));
        }
    }

    internal class BlackPawnPatterns : MovePatternStorage
    {
        public BlackPawnPatterns()
        {
            var pawnAttacks = PieceAttackPatterns.Instance.PawnAttackMask[1];
            var pawnMoves = PieceAttackPatterns.Instance.PawnMoveMask[1];
            var mb = new MoveBoard();
            for (var i = 0; i < 64; i++)
            {
                mb[i] = pawnAttacks[i] | pawnMoves[i];
            }
            InitializePawns(mb, new PawnMovesInitializer(Color.Black));
        }
    }
}
