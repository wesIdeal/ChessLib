using System;
using System.Collections.Generic;
using System.Linq;
using ChessLib.Core.MagicBitboard.Bitwise;

namespace ChessLib.Core.Types
{
    public delegate void OnFenChangedCallback(string fen);

    public class Tags : Dictionary<string, string>, IEquatable<Tags>
    {
        public readonly string[] RequiredTagKeys = { "Event", "Site", "Date", "Round", "White", "Black", "Result" };
        public OnFenChangedCallback OnFenChanged;


        public Tags(OnFenChangedCallback onFenChanged = null)
        {
            OnFenChanged = onFenChanged;
            foreach (var requiredTag in RequiredTagKeys)
            {
                Add(requiredTag, "?");
            }
            OnFENChanged(FENStart);
        }

        /// <summary>
        ///     Copies the elements from <paramref name="tags" /> into a new object.
        /// </summary>
        /// <param name="tags"></param>
        public Tags(Tags tags) : base(tags)
        {
        }

        public Tags(string fen) : this(fen, null)
        {

        }
        public Tags(string fen, Tags tags) : this(tags ?? new Tags())
        {
            if (fen != BoardConstants.FenStartingPosition)
            {
                SetFen(fen);
            }
        }

        public string Event
        {
            get => ContainsKey("Event") ? this["Event"] : "";
            set => Add("Event", value);
        }

        public string Site
        {
            get => ContainsKey("Site") ? this["Site"] : "";
            set => Add("Site", value);
        }

        public string Date
        {
            get => ContainsKey("Date") ? this["Date"] : "";
            set => Add("Date", value);
        }

        public string Round
        {
            get => ContainsKey("Round") ? this["Round"] : "";
            set => Add("Round", value);
        }

        public string Result
        {
            get => ContainsKey("Result") ? this["Result"] : "";
            set => Add("Result", value);
        }


        public IEnumerable<KeyValuePair<string, string>> RequiredTags
        {
            get
            {
                var requiredTagKeys = RequiredTagKeys;
                return requiredTagKeys.Select(t => new KeyValuePair<string, string>(t, Get(t)));
            }
        }


        public IEnumerable<KeyValuePair<string, string>> SupplementalTags
        {
            get
            {
                var suppTags = Keys.Where(k => !RequiredTagKeys.Contains(k));
                return suppTags.Select(t => new KeyValuePair<string, string>(t, Get(t)));
            }
        }

        public bool HasSetup => ContainsKey("FEN") || ContainsKey("SetUp") && this["SetUp"] == "1";
        public string FENStart
        {
            get
            {
                var fenReturnVal = BoardConstants.FenStartingPosition;
                if (HasSetup)
                {
                    var fen = this["FEN"];
                    if (!string.IsNullOrWhiteSpace(fen) && fen != "?")
                    {
                        fenReturnVal = fen;
                    }
                }

                return fenReturnVal;
            }
        }

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
            if (ContainsKey(key))
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
            else
            {
                base.Add(key, value);
            }

            if (key.ToLower() == "fen")
            {
                SetFen(value);
            }
        }


        internal void SetFen(string fen)
        {
            this["FEN"] = fen;
            OnFENChanged(fen);
        }

        private void OnFENChanged(string fen)
        {
            OnFenChanged?.Invoke(fen);
        }

        public bool Equals(Tags other)
        {
            if (other == null || Keys.Count != other.Keys.Count)
            {
                return false;
            }
            var theseKeys = Keys.OrderBy(x => x);

            foreach (var key in theseKeys)
            {
                if (!other.ContainsKey(key))
                {
                    return false;
                }

                if (this[key] != other[key])
                {
                    return false;
                }

            }

            return true;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Tags)obj);
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }

        public static bool operator ==(Tags left, Tags right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Tags left, Tags right)
        {
            return !Equals(left, right);
        }
    }
}