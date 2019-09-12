using ChessLib.Data.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace ChessLib.Data
{
    public delegate void OnFenChangedCallback(string fen);

    public class Tags : Dictionary<string, string>
    {
        public readonly string[] RequiredTagKeys = { "Event", "Site", "Date", "Round", "White", "Black", "Result" };
        public OnFenChangedCallback OnFenChanged;

        public Tags(OnFenChangedCallback onFenChanged = null)
        {
            OnFenChanged = onFenChanged;
            foreach (var requiredTag in RequiredTagKeys)
            {
                Add(requiredTag, requiredTag);
            }
            OnFENChanged(FENStart);
        }


        public IEnumerable<KeyValuePair<string, string>> RequiredTags
        {
            get
            {
                var requiredTagKeys = this.RequiredTagKeys;
                return requiredTagKeys.Select(t => new KeyValuePair<string, string>(t, Get(t)));
            }
        }


        public IEnumerable<KeyValuePair<string, string>> SupplementalTags
        {
            get
            {
                var suppTags = this.Keys.Where(k => !RequiredTagKeys.Contains(k));
                return suppTags.Select(t => new KeyValuePair<string, string>(t, Get(t)));
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

        public string Get(string key)
        {
            if (this.ContainsKey(key))
            {
                return this[key];
            }

            return "?";
        }

        public new void Add(string key, string value)
        {
            if (ContainsKey(key))
            {
                this[key] = value;
            }
            else { base.Add(key, value); }
            if (key.ToLower() == "fen")
            {
                SetFen(value);
            }
        }


        internal void SetFen(string fen)
        {
            this["PremoveFEN"] = fen;
            OnFENChanged(fen);
        }

        private void OnFENChanged(string fen)
        {
            if (OnFenChanged != null)
            {
                OnFenChanged(fen);
            }
        }
    }
}
