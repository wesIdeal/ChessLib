using ChessLib.Data.Helpers;
using System.Collections.Generic;

namespace ChessLib.Data
{
    public class Tags : Dictionary<string, string>
    {
        private readonly string[] _requiredTags = { "Event", "Site", "Date", "Round", "White", "Black", "Result" };
        public Tags()
        {
            foreach (var requiredTag in _requiredTags)
            {
                Add(requiredTag, requiredTag);
            }
        }

        public bool HasSetup => ContainsKey("SetUp") && this["SetUp"] == "1";
        public string FENStart => HasSetup ? this["FEN"] : FENHelpers.FENInitial;

        public string White
        {
            get => ContainsKey("White") ? this["White"] : "";
            set => Add("White", value);
        }

        public string Black
        {
            get => ContainsKey("Black") ? this["Black"] : "";
            set => Add("Black", value);
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
