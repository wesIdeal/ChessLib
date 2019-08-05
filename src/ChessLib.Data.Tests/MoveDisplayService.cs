using ChessLib.Data.Boards;
using ChessLib.Data.MoveRepresentation;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessLib.Data.Tests
{
    [TestFixture]
    public class PGNFormatterTests
    {
        private PGNFormatter<MoveStorage> _moveFormatter = new PGNFormatter<MoveStorage>(new PGNFormatterOptions());
        [TestCase("2bq1rk1/3p1npp/p1p3N1/1rbB1Pp1/1pQ5/P5N1/1PP3PP/R3R2K w - - 0 23", "d5f7", "23. Bxf7+")]
        public void TestCheckDisplay(string fen, string lan, string expected)
        {
            BoardInfo bi = new BoardInfo(fen);
            var _moveTranslator = new MoveTranslatorService(bi);
            bi.ApplyMove(_moveTranslator.FromLongAlgebraicNotation(lan));
           
        }


    }
}
