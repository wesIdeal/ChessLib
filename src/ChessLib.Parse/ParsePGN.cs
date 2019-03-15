using ChessLib.Parse.PGNPieces;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ChessLib.Parse
{
    public class MoveText
    {
        public string Move { get; set; }

    }
    public class ParsePGN
    {
        const string tagEx = "(\\[\\s*(?<tagName>\\w+)\\s*\"(?<tagValue>[^\"]*)\\\"\\s*\\]\\s*)";
        string movesRegex;
            
        const string moveToPlySplitRegex = @"\s+";
        const string variations = @"\(([^)]*)\)";

        public ParsePGN()
        {
            movesRegex = @"\s*(\d{1,3})\.?\s*((?:(?:O-O(?:-O)?)|(?:[KQNBR][1-8a-h]?x?[a-h]x?[1-8])|(?:[a-h]x?[a-h]?[1-8]\=?[QRNB]?))\+?)(?:\s*\d+\.?\d+?m?s)?\.?\s*((?:(?:O-O(?:-O)?)|(?:[KQNBR][1-8a-h]?x?[a-h]x?[1-8])|(?:[a-h]x?[a-h]?[1-8]\=?[QRNB]?))\+?)?(?:\s*\d+\.?\d+?m?s)?";
            var reMoveNumber = @"(\d{1,3})\.?\s*";
            var altmoves = @"(?:(?<mn>\d{1,3}.?)\s+((?<wm>(?:O-O(?:-O)?)|(?:[KQNBR][1-8a-h]?x?[a-h]x?[1-8])|(?:[a-h]x?[a-h]?[1-8]\=?[QRNB]?))\+?)\s?(?<nag>\$\d)\s+((?<bm>(?:O-O(?:-O)?)|(?:[KQNBR][1-8a-h]?x?[a-h]x?[1-8])|(?:[a-h]x?[a-h]?[1-8]\=?[QRNB]?))\+?)?\s?(?<nag>\$\d)?)";
            // It's either Castles or a piece move to a square, or a pice capture on a square or a pawn capgure on a square, or a promotion, possibly a check
            //var reMove =  @"((?:(?:O-O(?:-O)?)|(?:[KQNBR][1-8a-h]?x?[a-h]x?[1-8])|(?:[a-h]x?[a-h]?[1-8]\=?[QRNB]?))\+?)";
            //var nag = 
            // s += "(?:\s*\d+\.?\d+?m?s)?\.?\s*((?:(?:O-O(?:-O)?)|(?:[KQNBR][1-8a-h]?x?[a-h]x?[1-8])|(?:[a-h]x?[a-h]?[1-8]\=?[QRNB]?))\+?)?(?:\s*\d+\.?\d+?m?s)?";
            //movesRegex = $"\\s*{reMoveNumber}\\s*{reMove}"
        }

        protected string RemoveComments(string pgn)
        {
            var rv = Regex.Replace(pgn, @"\{[^}]*\}\s+(\d...\s)?", "");

            return rv;
        }
        protected string RemoveTags(string pgn, out Dictionary<string, string> tags)
        {
            tags = GetTagValues(pgn);

            return Regex.Replace(pgn, "(\\[\\s*(?<tagName>\\w+)(\\s)*\"(?<tagValue>[^\"]*)\\\"\\s*\\]\\s*)+", "");
        }

        protected Tags GetTagValues(string pgn)
        {
            var rvTagDictionary = new Tags();
            MatchCollection tagMatches = Regex.Matches(pgn, tagEx);
            foreach (Match tag in tagMatches)
            {
                //var splitTag = Regex.Split(tag, tagEx);
                var key = tag.Groups[2];
                var value = tag.Groups[3];
                rvTagDictionary.Add(key.Value.Trim(), value.Value.Trim());
            }
            return rvTagDictionary;
        }

        protected string[] GetVariations(string pgn)
        {
            
            var vars = Regex.Matches(pgn, variations);
            return new string[] { };
        }

        public string[] GetMovesFromPGN(string pgn)
        {
            var rv = new List<string>();
            var mainMoves = GetMainMoves(pgn);
            foreach (var mv in mainMoves)
            {
                var splits = Regex.Split(mv, moveToPlySplitRegex);
            }
            return rv.ToArray();
        }

        protected string[] GetMainMoves(string pgn)
        {
            var rv = new List<string>();
            var matches = Regex.Matches(pgn, movesRegex);
            foreach (Match m in matches)
            {
                rv.Add(m.Value.Trim());
            }
            return rv.ToArray();
        }
    }
}
