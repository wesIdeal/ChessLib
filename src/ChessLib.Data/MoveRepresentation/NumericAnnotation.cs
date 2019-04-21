using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using EnumsNET;
namespace ChessLib.Data.MoveRepresentation
{
    [AttributeUsage(AttributeTargets.Field)]
    class SymbolAttribute : Attribute
    {
        public string Symbol { get; }

        public SymbolAttribute(string symbol)
        {
            Symbol = symbol;
        }
    }
    public enum MoveAnnotation
    {
        [Description("null annotation")]
        [Symbol("")]
        Null = 0,
        [Description("good move")]
        [Symbol("!")]
        GoodMove = 1,
        [Description("poor move")]
        [Symbol("?")]
        PoorMove = 2,
        [Description("very good move")]
        [Symbol("!!")]
        VeryGoodMove = 3,
        [Description("very poor move")]
        [Symbol("??")]
        VeryPoorMove = 4,
        [Description("speculative move")]
        [Symbol("!?")]
        SpeculativeMove = 5,
        [Description("questionable move")]
        [Symbol("?!")]
        QuestionableMove = 6,
        [Description("forced move (all others lose quickly)")]
        [Symbol("□")]
        ForcedMove = 7,
        [Description("singular move (no reasonable alternatives)")]
        [Symbol("")]
        SingularMove = 8,
        [Description("worst move")]
        [Symbol("")]
        WorstMove = 9
    }
    public class NumericAnnotation
    {
        private readonly EnumFormat _symbolFormat;

        public NumericAnnotation()
        {
            _symbolFormat = Enums.RegisterCustomEnumFormat(member => member.Attributes.Get<SymbolAttribute>()?.Symbol);
            MoveAnnotation = MoveAnnotation.Null;
        }

        public MoveAnnotation MoveAnnotation { get; set; }
    }
}
