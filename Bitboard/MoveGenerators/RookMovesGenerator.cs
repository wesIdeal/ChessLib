using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace MagicBitboard
{
    public class RookMovesGenerator : MoveInitializer
    {
        
        public override ulong CalculateMovesFromPosition(int positionIndex, ulong occupancyBoard)
        {
            var rv = (ulong)0;
            var startingValue = (ulong)1 << positionIndex;
            //N
            var positionalValue = startingValue;
            while ((positionalValue = positionalValue.ShiftN()) != 0)
            {

                rv |= positionalValue;
                if ((occupancyBoard & positionalValue) == positionalValue) break;
            }
            //E
            positionalValue = startingValue;

            while ((positionalValue = positionalValue.ShiftE()) != 0)
            {
                rv |= positionalValue;
                if ((occupancyBoard & positionalValue) == positionalValue) break;
            }
            //S
            positionalValue = startingValue;
            while ((positionalValue = positionalValue.ShiftS()) != 0)
            {
                rv |= positionalValue;
                if ((occupancyBoard & positionalValue) == positionalValue) break;
            }
            //W
            positionalValue = startingValue;
            while ((positionalValue = positionalValue.ShiftW()) != 0)
            {
                rv |= positionalValue;
                if ((occupancyBoard & positionalValue) == positionalValue) break;
            }
            return rv;
        }

        
        
    }
}
