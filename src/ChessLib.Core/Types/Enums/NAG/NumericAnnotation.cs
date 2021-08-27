using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnumsNET;

// ReSharper disable InconsistentNaming

namespace ChessLib.Core.Types.Enums.NAG
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class SymbolAttribute : Attribute
    {
        public string Symbol { get; }

        public SymbolAttribute(string symbol)
        {
            Symbol = symbol;
        }
    }

    public class NumericAnnotation : IEquatable<NumericAnnotation>
    {
        public string AnnotationText { get; set; }

        public MoveNAG MoveNAG { get; set; }
        public List<PositionalNAG> PositionalNAGs { get; set; }
        public List<NonStandardNAG> NonStandardNAGs { get; set; }
        public TimeTroubleNAG TimeTroubleNAG { get; set; }

        public string LastError { get; set; }

        public NumericAnnotation()
        {

            MoveNAG = MoveNAG.Null;
            TimeTroubleNAG = TimeTroubleNAG.Null;
            PositionalNAGs = new List<PositionalNAG>();
            NonStandardNAGs = new List<NonStandardNAG>();
        }

        public NumericAnnotation(string nag) : this()
        {
            ApplyNag(nag);
        }

        public NumericAnnotation(int nag) : this()
        {
            ApplyNag(nag);
        }

        private static readonly EnumFormat _symbolFormat =
            EnumsNET.Enums.RegisterCustomEnumFormat(member => member.Attributes.Get<SymbolAttribute>()?.Symbol);

        public bool Equals(NumericAnnotation other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return AnnotationText == other.AnnotationText && MoveNAG == other.MoveNAG &&
                   PositionalNAGs.SequenceEqual(other.PositionalNAGs) &&
                   NonStandardNAGs.SequenceEqual(other.NonStandardNAGs) &&
                   TimeTroubleNAG == other.TimeTroubleNAG && LastError == other.LastError;
        }

        public bool Any()
        {
            return MoveNAG != MoveNAG.Null ||
                   PositionalNAGs.Any() ||
                   TimeTroubleNAG != TimeTroubleNAG.Null ||
                   NonStandardNAGs.Any();
        }

        public IEnumerable<int> All()
        {
            if (MoveNAG != MoveNAG.Null)
            {
                yield return (int)MoveNAG;
            }

            foreach (var positionalNAG in PositionalNAGs)
            {
                yield return (int)positionalNAG;
            }

            if (TimeTroubleNAG != TimeTroubleNAG.Null)
            {
                yield return (int)TimeTroubleNAG;
            }

            foreach (var nonStandardNAG in NonStandardNAGs)
            {
                yield return (int)nonStandardNAG;
            }
        }

        public string ToString(AnnotationFormat format)
        {
            StringBuilder sb = new StringBuilder();
            Format(sb, MoveNAG, format);
            Format(sb, PositionalNAGs, format);
            Format(sb, TimeTroubleNAG, format);
            Format(sb, NonStandardNAGs, format);
            return sb.ToString().Trim();
        }

        /// <summary>
        /// Builds the Nag string, in a PGN-friendly format
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return ToString(AnnotationFormat.Symbolic);
        }

        /// <summary>
        /// Builds the NAG to a standard string intended for import, formatted in order of annotation Type.
        /// </summary>
        /// <returns></returns>
        public string ToNAGString()
        {
            return ToString(AnnotationFormat.PGNSpec);

        }

        private static void Format<T>(StringBuilder sb, IEnumerable<T> nags, AnnotationFormat annotationFormat)
            where T : struct, Enum
        {
            foreach (var nag in nags)
            {
                Format(sb, nag, annotationFormat).Append(" ");
            }
        }

        private static StringBuilder Format<T>(StringBuilder sb, T nag, AnnotationFormat annotationFormat)
            where T : struct, Enum
        {
            var value = (int)EnumsNET.Enums.GetUnderlyingValue(nag);
            if (value != 0)
            {
                var strNag = annotationFormat == AnnotationFormat.Symbolic
                    ? nag.AsString(_symbolFormat)
                    : "$" + EnumsNET.Enums.GetUnderlyingValue(nag);
                if (!string.IsNullOrWhiteSpace(strNag))
                {
                    sb.Append(strNag).Append(" ");
                }
            }

            return sb;
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

        public void ApplyNag(NumericAnnotation otherNag)
        {
            otherNag.All().ToList().ForEach(ApplyNag);
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