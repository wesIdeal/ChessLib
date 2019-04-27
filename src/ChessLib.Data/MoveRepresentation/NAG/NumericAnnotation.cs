using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnumsNET;

namespace ChessLib.Data.MoveRepresentation.NAG
{
    [AttributeUsage(AttributeTargets.Field)]
    internal sealed class SymbolAttribute : Attribute
    {
        public SymbolAttribute(string symbol)
        {
            Symbol = symbol;
        }

        public string Symbol { get; }
    }

    public class NumericAnnotation
    {
        private readonly EnumFormat _symbolFormat;

        public NumericAnnotation()
        {
            _symbolFormat = Enums.RegisterCustomEnumFormat(member => member.Attributes.Get<SymbolAttribute>()?.Symbol);
            MoveNAG = MoveNAG.Null;
            TimeTroubleNAG = TimeTroubleNAG.Null;
            PositionalNAGs = new List<PositionalNAG>();
            NonStandardNAGs = new List<NonStandardNAG>();
        }

        public MoveNAG MoveNAG { get; set; }
        public List<PositionalNAG> PositionalNAGs { get; set; }
        public List<NonStandardNAG> NonStandardNAGs { get; set; }
        public TimeTroubleNAG TimeTroubleNAG { get; set; }

        public string LastError { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(MoveNAG.AsString(_symbolFormat) + " ");
            foreach (var positionalNAG in PositionalNAGs) sb.Append(positionalNAG.AsString(_symbolFormat) + " ");

            sb.Append(TimeTroubleNAG.AsString(_symbolFormat) + " ");
            foreach (var nonStandardNAG in NonStandardNAGs) sb.Append(nonStandardNAG.AsString(_symbolFormat) + " ");

            return sb.ToString().Replace("  ", " ").Trim();
        }

        public string ToNAGString()
        {
            var sb = new StringBuilder();
            sb.Append("$" + MoveNAG + " ");
            foreach (var positionalNAG in PositionalNAGs) sb.Append("$" + positionalNAG + " ");

            sb.Append("$" + TimeTroubleNAG + " ");
            foreach (var nonStandardNAG in NonStandardNAGs) sb.Append("$" + nonStandardNAG + " ");

            return sb.ToString().Replace("  ", " ").Trim();
        }

        /// <summary>
        ///     Apply a Numeric Annotation Glyph (NAG) from a string representation, either format works- ${nagInt} or {nagInt}
        /// </summary>
        /// <param name="nagText"></param>
        public void ApplyNag(string nagText)
        {
            nagText = nagText.Replace("$", "");
            if (!int.TryParse(nagText, out var nag))
            {
                LastError = $"Could not parse NAG {nagText}";
                return;
            }

            LastError = "";
            ApplyNag(nag);
        }

        /// <summary>
        ///     Apply a Numeric Annotation Glyph (NAG) from an integer
        /// </summary>
        public void ApplyNag(int nag)
        {
            try
            {
                if (nag >= 1 && nag <= 9)
                {
                    MoveNAG = (MoveNAG)nag;
                }
                else if (nag >= 10 && nag <= 135)
                {
                    PositionalNAGs.Add((PositionalNAG)nag);
                    PositionalNAGs = PositionalNAGs.Distinct().ToList();
                }
                else if (nag >= 136 && nag <= 139)
                {
                    TimeTroubleNAG = (TimeTroubleNAG)nag;
                }
                else
                {
                    NonStandardNAGs.Add((NonStandardNAG)nag);
                    NonStandardNAGs = NonStandardNAGs.Distinct().ToList();
                }
            }
            catch (Exception)
            {
                LastError = $"Could not decipher NAG given {nag}";
            }
        }
    }
}