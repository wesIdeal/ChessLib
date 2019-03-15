using ChessLib.Parse.PGNPieces;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ChessLib.Parse
{
    public class ParsePGN
    {
        const string tagEx = "(\\[\\s*(?<tagName>\\w+)\\s*\"(?<tagValue>[^\"]*)\\\"\\s*\\]\\s*)";
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
                rvTagDictionary.Add(key.Value, value.Value);
            }
            return rvTagDictionary;
        }
    }
}
