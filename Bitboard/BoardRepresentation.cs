using MagicBitboard.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MagicBitboard
{
    public class BoardRepresentation
    {
        public ulong[,] PiecePlacement = new ulong[2, 6];

        public BoardRepresentation()
        {
            foreach (var p in (Piece[])Enum.GetValues(typeof(Piece)))
            {
                PiecePlacement[Color.White.ToInt(), p.ToInt()] = 0;
                PiecePlacement[Color.Black.ToInt(), p.ToInt()] = 0;
            }
        }

        public ulong this[Color c, Piece p]
        {
            get { return this.PiecePlacement[c.ToInt(), p.ToInt()]; }
        }

        public ulong GetBlockersOfColor(Color c)
        {
            var board = (ulong)0;
            foreach (var p in (Piece[])Enum.GetValues(typeof(Piece)))
            {
                board |= this[c, p];
            }
            return board;
        }

        public static string BitString(ulong u)
        {
            return Convert.ToString((long)u, 2).PadLeft(64, '0');
        }

        public string BitString(Color c, Piece p)
        {
            var board = this[c, p];
            return BitString(board);
        }



        public BoardRepresentation(string fen)
        {
            var fenPieces = fen.Split(' ');
            var board = fenPieces[0];
            var ranks = board.Split('/');
            var arrayPosition = 63;
            foreach (var rank in ranks)
            {

                for (int idx = 0; idx < rank.Length; idx++)
                {
                    char piece = rank[idx];
                    if (Char.IsDigit(piece))
                    {
                        var skip = Convert.ToInt16(piece);
                        arrayPosition -= skip;
                    }
                    else
                    {
                        var poc = PieceOfColor.GetPieceOfColor(piece);

                        PiecePlacement[poc.Color.ToInt(), poc.Piece.ToInt()] |= ((ulong)1 << arrayPosition);
                        arrayPosition--;
                    }
                }
            }
        }

        public char?[] GetCharRepresentation(ulong board, Piece p, Color c)
        {
            var charRep = PieceOfColor.GetCharRepresentation(p, c);
            var str = BitString(this[c, p]);
            var rv = str.Select(x => x == '0' ? (char?)null : charRep).ToArray();
            return rv;
        }

        public string FenBoard
        {
            get
            {
                var fen = "";
                char?[] fenBeginnings = GetCharacterArrayRepresntation();
                var noPieceCount = 0;
                var fileCount = 0;
                var rankCount = 0;
                char?[] pieces = new char?[8];
                while ((pieces = fenBeginnings.Skip(8 * rankCount).Take(8).ToArray()).Any())
                {
                    foreach (var c in pieces)
                    {
                        if (c == null)
                        {
                            noPieceCount++;
                        }
                        else
                        {
                            if (fileCount == 6)
                                if (noPieceCount > 0)
                                {
                                    fen += noPieceCount.ToString();
                                    noPieceCount = 0;
                                }
                            fen += c;
                        }
                    }
                    if (noPieceCount > 0)
                    {
                        fen += Convert.ToChar(noPieceCount);
                        noPieceCount = 0;
                    }
                    fen += "/";

                }
                return fen;
            }
        }


        public char?[] GetCharacterArrayRepresntation()
        {
            var fenBeginnings = new char?[64];

            foreach (var color in (Color[])Enum.GetValues(typeof(Color)))
            {

                foreach (var piece in (Piece[])Enum.GetValues(typeof(Piece)))
                {

                    var pieceBoard = this[color, piece];
                    var charRep = GetCharRepresentation(pieceBoard, piece, color);

                    for (int i = 0; i < charRep.Count(); i++)
                    {
                        if (charRep[i] != null)
                        {
                            fenBeginnings[i] = charRep[i];
                        }
                    }
                }
            }

            return fenBeginnings;
        }
    }
}