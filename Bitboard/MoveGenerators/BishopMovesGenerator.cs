namespace MagicBitboard
{
    public class BishopMovesGenerator : MoveInitializer
    {
        public override ulong CalculateMovesFromPosition(int positionIndex, ulong occupancyBoard)
        {
            var rv = (ulong)0;
            var startingValue = (ulong)1 << positionIndex;
            //NE
            var positionalValue = startingValue;
            while ((positionalValue = positionalValue.ShiftNE()) != 0)
            {
                rv |= positionalValue;
                if ((occupancyBoard & positionalValue) == positionalValue) break;
            }
            //NW
            positionalValue = startingValue;
            while ((positionalValue = positionalValue.ShiftNW()) != 0)
            {
                rv |= positionalValue;
                if ((occupancyBoard & positionalValue) == positionalValue) break;
            }
            //SE
            positionalValue = startingValue;
            while ((positionalValue = positionalValue.ShiftSE()) != 0)
            {
                rv |= positionalValue;
                if ((occupancyBoard & positionalValue) == positionalValue) break;
            }
            //SW
            positionalValue = startingValue;
            while ((positionalValue = positionalValue.ShiftSW()) != 0)
            {
                rv |= positionalValue;
                if ((occupancyBoard & positionalValue) == positionalValue) break;
            }
            return rv;
        }
    }
}
