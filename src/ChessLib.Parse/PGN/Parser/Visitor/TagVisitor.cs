using System.Collections.Generic;
using ChessLib.Data;
using ChessLib.Parse.PGN.Parser.BaseClasses;
namespace ChessLib.Parse.PGN.Parser.Visitor
{
    internal class TagVisitor : PGNBaseVisitor<Tags>
    {
        public Tags GetTagSection(Parser.BaseClasses.PGNParser.TagSectionContext ctx)
        {
            var tagSection = new Tags();
            var tagPairContexts = ctx.tagPair();
            foreach (var tagPairContext in tagPairContexts)
            {
                var tagPair = GetTagPair(tagPairContext);
                tagSection.Add(tagPair.Key, tagPair.Value);
            }
            return tagSection;
        }

        private KeyValuePair<string, string> GetTagPair(Parser.BaseClasses.PGNParser.TagPairContext context)
        {
            var tagKey = GetTagKey(context.tagName());
            var tagVal = GetTagValue(context.tagValue());
            return new KeyValuePair<string, string>(tagKey, tagVal);
        }

        private string GetTagKey(Parser.BaseClasses.PGNParser.TagNameContext context)
        {
            return context.GetText().Replace("\"", "");
        }

        private string GetTagValue(Parser.BaseClasses.PGNParser.TagValueContext context)
        {
            return context.GetText().Replace("\"", "");
        }
    }
}