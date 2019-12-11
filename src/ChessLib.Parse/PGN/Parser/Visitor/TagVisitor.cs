using System.Collections.Generic;
using ChessLib.Data;
using ChessLib.Parse.PGN.Parser.BaseClasses;
namespace ChessLib.Parse.PGN.Parser.Visitor
{
    internal class TagVisitor : PGNBaseVisitor<Tags>
    {
        public Tags VisitTagSection(Parser.BaseClasses.PGNParser.Tag_sectionContext ctx)
        {
            var tagSection = new Tags();
            var tagPairCtxs = ctx.tag_pair();
            foreach (var tagPairContext in tagPairCtxs)
            {
                var tagPair = VisitTagPair(tagPairContext);
                tagSection.Add(tagPair.Key, tagPair.Value);
            }
            return tagSection;
        }

        private KeyValuePair<string, string> VisitTagPair(Parser.BaseClasses.PGNParser.Tag_pairContext context)
        {
            var tagKey = VisitTagKey(context.tag_name());
            var tagVal = VisitTagValue(context.tag_value());
            return new KeyValuePair<string, string>(tagKey, tagVal);
        }

        private string VisitTagKey(Parser.BaseClasses.PGNParser.Tag_nameContext context)
        {
            return context.GetText().Replace("\"", "");
        }

        private string VisitTagValue(Parser.BaseClasses.PGNParser.Tag_valueContext context)
        {
            return context.GetText().Replace("\"", "");
        }
    }
}