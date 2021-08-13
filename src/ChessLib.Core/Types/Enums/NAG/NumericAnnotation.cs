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
            _symbolFormat =
                EnumsNET.Enums.RegisterCustomEnumFormat(member => member.Attributes.Get<SymbolAttribute>()?.Symbol);
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

        private readonly EnumFormat _symbolFormat;

        public bool Equals(NumericAnnotation other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return AnnotationText == other.AnnotationText && MoveNAG == other.MoveNAG &&
                   PositionalNAGs.SequenceEqual(other.PositionalNAGs) && NonStandardNAGs.SequenceEqual(other.NonStandardNAGs) &&
                   TimeTroubleNAG == other.TimeTroubleNAG && LastError == other.LastError;
        }

        public IEnumerable<int> All()
        {
            if (MoveNAG != MoveNAG.Null)
            {
                yield return (int) MoveNAG;
            }

            foreach (var positionalNAG in PositionalNAGs)
            {
                yield return (int) positionalNAG;
            }

            if (TimeTroubleNAG != TimeTroubleNAG.Null)
            {
                yield return (int) TimeTroubleNAG;
            }

            foreach (var nonStandardNAG in NonStandardNAGs)
            {
                yield return (int) nonStandardNAG;
            }
        }

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
            if (MoveNAG != MoveNAG.Null)
            {
                sb.Append(" $" + (int) MoveNAG);
            }

            foreach (var positionalNAG in PositionalNAGs)
            {
                sb.Append(" $" + (int) positionalNAG);
            }

            if (TimeTroubleNAG != TimeTroubleNAG.Null)
            {
                sb.Append(" $" + (int) TimeTroubleNAG);
            }

            foreach (var nonStandardNAG in NonStandardNAGs)
            {
                sb.Append(" $" + (int) nonStandardNAG);
            }

            return sb.ToString().Trim();
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
                    MoveNAG = (MoveNAG) nag;
                }
                else if (nag >= 10 && nag <= 135)
                {
                    PositionalNAGs.Add((PositionalNAG) nag);
                    PositionalNAGs = PositionalNAGs.Distinct().ToList();
                }
                else if (nag >= 136 && nag <= 139)
                {
                    TimeTroubleNAG = (TimeTroubleNAG) nag;
                }
                else
                {
                    NonStandardNAGs.Add((NonStandardNAG) nag);
                    NonStandardNAGs = NonStandardNAGs.Distinct().ToList();
                }
            }
            catch (Exception)
            {
                LastError = $"Could not decipher NAG given {nag}";
            }
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((NumericAnnotation) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = AnnotationText != null ? AnnotationText.GetHashCode() : 0;
                hashCode = (hashCode * 397) ^ (int) MoveNAG;
                hashCode = (hashCode * 397) ^ (PositionalNAGs != null ? PositionalNAGs.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (NonStandardNAGs != null ? NonStandardNAGs.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (int) TimeTroubleNAG;
                hashCode = (hashCode * 397) ^ (LastError != null ? LastError.GetHashCode() : 0);
                return hashCode;
            }
        }

        public static bool operator ==(NumericAnnotation left, NumericAnnotation right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(NumericAnnotation left, NumericAnnotation right)
        {
            return !Equals(left, right);
        }
    }
}