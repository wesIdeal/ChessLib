using System;
using System.Collections.Generic;
using System.Text;

namespace ChessLib.Parse.PGNPieces
{
    public class Tags : Dictionary<string, string>
    {
        private readonly string[] requiredTags = new string[] { "Event", "Site", "Date", "Round", "White", "Black", "Result" };
        public Tags()
        {
            foreach (var requiredTag in requiredTags)
            {
                Add(requiredTag, requiredTag);
            }
        }

        public new void Add(string key, string value)
        {
            if (ContainsKey(key))
            {
                this[key] = value;
            }
            else { base.Add(key, value); }
        }
    }
}
