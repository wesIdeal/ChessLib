using System;
using System.Collections.Generic;
using System.Linq;
using ChessLib.Core.MagicBitboard.Bitwise;
using ChessLib.Core.Types.Enums;
using ChessLib.Core.Types.PgnExport;
using EnumsNET;

namespace ChessLib.Core.Types
{
    public delegate void OnFenChangedCallback(string fen);

    public class Tags : Dictionary<string, string>, IEquatable<Tags>
    {
        public string Event
        {
            get => ContainsKey("Event") ? this["Event"] : "?";
            set => Add("Event", value);
        }

        public string Site
        {
            get => ContainsKey("Site") ? this["Site"] : "?";
            set => Add("Site", value);
        }

        public string Date
        {
            get => ContainsKey("Date") ? this["Date"] : "????.??.??";
            set => Add("Date", value);
        }

        public string Round
        {
            get => ContainsKey("Round") ? this["Round"] : "?";
            set => Add("Round", value);
        }

        public string Result => GameResult.AsString(EnumFormat.Description);
        public GameResult GameResult { get; set; } = GameResult.None;


        public IEnumerable<KeyValuePair<string, string>> RequiredTags
        {
            get
            {
                yield return new KeyValuePair<string, string>("Event", Event);
                yield return new KeyValuePair<string, string>("Site", Site);
                yield return new KeyValuePair<string, string>("Date", Date);
                yield return new KeyValuePair<string, string>("Round", Round);
                yield return new KeyValuePair<string, string>("White", White);
                yield return new KeyValuePair<string, string>("Black", Black);
                yield return new KeyValuePair<string, string>("Result", Result);
                if (HasSetup)
                {
                    yield return new KeyValuePair<string, string>("SetUp", "1");
                    yield return new KeyValuePair<string, string>("FEN", FENStart);
                }
            }
        }

        public IEnumerable<KeyValuePair<string, string>> SupplementalTags
        {
            get
            {
                var excludedTags = RequiredTags.Select(x => x.Key);

                var suppTags = Keys.Where(k => !excludedTags.Contains(k)).OrderBy(x => x);
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
            get => ContainsKey("White") ? this["White"] : "?";
            set => Add("White", value);
        }

        public string Black
        {
            get => ContainsKey("Black") ? this["Black"] : "?";
            set => Add("Black", value);
        }


        public Tags(OnFenChangedCallback onFenChanged = null)
        {
            OnFenChanged = onFenChanged;

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

        public OnFenChangedCallback OnFenChanged;

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

        public override string ToString()
        {
            var tagSerializer = new TagSerializer();
            return string.Join(Environment.NewLine, tagSerializer.Convert(this));
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
    }
}