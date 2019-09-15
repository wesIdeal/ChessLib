using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChessLib.Parse.PGN
{
    public class ParsePolyglot
    {
        public void PolyglotToPgn(in byte[] polyglotContents)
        {
            var tripliets = ReadContents(polyglotContents);

        }

        private PolyglotTriplet[] ReadContents(in byte[] polyglotContents)
        {
            int start = 0;
            PolyglotTriplet triplet;
            var rv = new List<PolyglotTriplet>();
            while ((triplet = ReadTriplet(polyglotContents, start)) != null)
            {
                start++;
                rv.Add(triplet);
            }

            return rv.ToArray();
        }

        const int sizeOfTriplet = 16;

        private PolyglotTriplet ReadTriplet(in byte[] polyglotContents, int start)
        {
            var startByte = start * 16;
            if (startByte >= polyglotContents.Length) return null;
            var readHash = BitConverter.ToUInt64(polyglotContents.Skip(startByte).Take(8).ToArray(), 0);
            var moveUshort = BitConverter.ToUInt16(polyglotContents.Skip(startByte + 8).Take(2).ToArray(), 0);
            var weight = BitConverter.ToUInt16(polyglotContents.Skip(startByte + 10).Take(2).ToArray(), 0);
            var learn = BitConverter.ToUInt16(polyglotContents.Skip(startByte + 12).Take(4).ToArray(), 0);
            return new PolyglotTriplet(readHash, moveUshort, weight, learn);
        }
    }
}
