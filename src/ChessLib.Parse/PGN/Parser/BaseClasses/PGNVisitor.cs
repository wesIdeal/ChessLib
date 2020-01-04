//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     ANTLR Version: 4.7.2
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from .\PGN.g4 by ANTLR 4.7.2

// Unreachable code detected
#pragma warning disable 0162
// The variable '...' is assigned but its value is never used
#pragma warning disable 0219
// Missing XML comment for publicly visible type or member '...'
#pragma warning disable 1591
// Ambiguous reference in cref attribute
#pragma warning disable 419

namespace ChessLib.Parse.PGN.Parser.BaseClasses {

	#pragma warning disable 3021

using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using IToken = Antlr4.Runtime.IToken;

/// <summary>
/// This interface defines a complete generic visitor for a parse tree produced
/// by <see cref="PGNParser"/>.
/// </summary>
/// <typeparam name="Result">The return type of the visit operation.</typeparam>
[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.7.2")]
[System.CLSCompliant(false)]
internal interface IPGNVisitor<Result> : IParseTreeVisitor<Result> {
	/// <summary>
	/// Visit a parse tree produced by <see cref="PGNParser.parse"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitParse([NotNull] PGNParser.ParseContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="PGNParser.pgnDatabase"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitPgnDatabase([NotNull] PGNParser.PgnDatabaseContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="PGNParser.pgnGame"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitPgnGame([NotNull] PGNParser.PgnGameContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="PGNParser.tagSection"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitTagSection([NotNull] PGNParser.TagSectionContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="PGNParser.tagPair"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitTagPair([NotNull] PGNParser.TagPairContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="PGNParser.tagName"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitTagName([NotNull] PGNParser.TagNameContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="PGNParser.tagValue"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitTagValue([NotNull] PGNParser.TagValueContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="PGNParser.moveSection"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitMoveSection([NotNull] PGNParser.MoveSectionContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="PGNParser.elementSequence"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitElementSequence([NotNull] PGNParser.ElementSequenceContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="PGNParser.element"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitElement([NotNull] PGNParser.ElementContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="PGNParser.moveNumberIndication"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitMoveNumberIndication([NotNull] PGNParser.MoveNumberIndicationContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="PGNParser.nag"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitNag([NotNull] PGNParser.NagContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="PGNParser.comment"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitComment([NotNull] PGNParser.CommentContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="PGNParser.sanMove"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitSanMove([NotNull] PGNParser.SanMoveContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="PGNParser.recursiveVariation"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitRecursiveVariation([NotNull] PGNParser.RecursiveVariationContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="PGNParser.gameTermination"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitGameTermination([NotNull] PGNParser.GameTerminationContext context);
}
} // namespace ChessLib.Parse.PGN.Parser.BaseClasses

