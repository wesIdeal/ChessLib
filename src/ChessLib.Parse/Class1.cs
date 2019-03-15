using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ChessLib.Parse
{
    public class ParsePGN
    {
        protected string RemoveComments(string pgn)
        {
            var rv = Regex.Replace(pgn, @"\{[^}]*\}\s+(\d...\s)?", "");

            return rv;
        }
        protected string RemoveTags(string pgn, out Dictionary<string, string> tags)
        {
            const string tagEx = "(\\[\\s*(?<tagName>\\w+)\\s*\"(?<tagValue>[^\"]*)\\\"\\s*\\]\\s*)+";

            tags = new Dictionary<string, string>();
            var tagMatches = Regex.Matches(pgn, tagEx);
            var rv = Regex.Split(pgn, tagEx);

            return Regex.Replace(pgn, "(\\[\\s*(?<tagName>\\w+)\\s*\"(?<tagValue>[^\"]*)\\\"\\s*\\]\\s*)+", "");
        }
    }
}
