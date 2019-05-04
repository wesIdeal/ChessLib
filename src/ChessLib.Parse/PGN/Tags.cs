using ChessLib.Data.Helpers;
using System.Collections.Generic;

namespace ChessLib.Parse.PGN
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

        public bool HasSetup => this.ContainsKey("SetUp") && this["SetUp"] == "1";
        public string FENStart => HasSetup ? this["FEN"] : FENHelpers.FENInitial;

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
