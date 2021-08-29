using System.Collections.Generic;
using ChessLib.Core.Types;

namespace ChessLib.Core.Translate
{
    public class MovesToLanVariation : ChessDto<IEnumerable<Move>, IEnumerable<string>>
    {
        private readonly MoveToLan moveToLan = new MoveToLan();

        public override IEnumerable<string> Translate(IEnumerable<Move> moves)
        {
            foreach (var move in moves)
            {
                yield return moveToLan.Translate(move);
            }
        }
    }
}