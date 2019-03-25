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
using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;
using Antlr4.Runtime;
using Antlr4.Runtime.Atn;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using DFA = Antlr4.Runtime.Dfa.DFA;

[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.7.2")]
[System.CLSCompliant(false)]
public partial class PGNParser : Parser {
	protected static DFA[] decisionToDFA;
	protected static PredictionContextCache sharedContextCache = new PredictionContextCache();
	public const int
		WHITE_WINS=1, BLACK_WINS=2, DRAWN_GAME=3, BOL=4, REST_OF_LINE_COMMENT=5, 
		BRACE_COMMENT=6, ESCAPE=7, NEW_LINE=8, SECTION_SEPARATOR=9, SPACES=10, 
		STRING=11, INTEGER=12, PERIOD=13, ASTERISK=14, LEFT_BRACKET=15, RIGHT_BRACKET=16, 
		LEFT_PARENTHESIS=17, RIGHT_PARENTHESIS=18, LEFT_ANGLE_BRACKET=19, RIGHT_ANGLE_BRACKET=20, 
		NUMERIC_ANNOTATION_GLYPH=21, SYMBOL=22, SUFFIX_ANNOTATION=23, UNEXPECTED_CHAR=24;
	public const int
		RULE_parse = 0, RULE_pgn_database = 1, RULE_pgn_game = 2, RULE_tag_section = 3, 
		RULE_tag_pair = 4, RULE_tag_name = 5, RULE_tag_value = 6, RULE_movetext_section = 7, 
		RULE_element_sequence = 8, RULE_element = 9, RULE_nag_item = 10, RULE_move_number_indication = 11, 
		RULE_san_move = 12, RULE_recursive_variation = 13, RULE_game_termination = 14;
	public static readonly string[] ruleNames = {
		"parse", "pgn_database", "pgn_game", "tag_section", "tag_pair", "tag_name", 
		"tag_value", "movetext_section", "element_sequence", "element", "nag_item", 
		"move_number_indication", "san_move", "recursive_variation", "game_termination"
	};

	private static readonly string[] _LiteralNames = {
		null, "'1-0'", "'0-1'", "'1/2-1/2'", null, null, null, null, null, null, 
		null, null, null, null, "'*'", "'['", "']'", "'('", "')'", "'<'", "'>'"
	};
	private static readonly string[] _SymbolicNames = {
		null, "WHITE_WINS", "BLACK_WINS", "DRAWN_GAME", "BOL", "REST_OF_LINE_COMMENT", 
		"BRACE_COMMENT", "ESCAPE", "NEW_LINE", "SECTION_SEPARATOR", "SPACES", 
		"STRING", "INTEGER", "PERIOD", "ASTERISK", "LEFT_BRACKET", "RIGHT_BRACKET", 
		"LEFT_PARENTHESIS", "RIGHT_PARENTHESIS", "LEFT_ANGLE_BRACKET", "RIGHT_ANGLE_BRACKET", 
		"NUMERIC_ANNOTATION_GLYPH", "SYMBOL", "SUFFIX_ANNOTATION", "UNEXPECTED_CHAR"
	};
	public static readonly IVocabulary DefaultVocabulary = new Vocabulary(_LiteralNames, _SymbolicNames);

	[NotNull]
	public override IVocabulary Vocabulary
	{
		get
		{
			return DefaultVocabulary;
		}
	}

	public override string GrammarFileName { get { return "PGN.g4"; } }

	public override string[] RuleNames { get { return ruleNames; } }

	public override string SerializedAtn { get { return new string(_serializedATN); } }

	static PGNParser() {
		decisionToDFA = new DFA[_ATN.NumberOfDecisions];
		for (int i = 0; i < _ATN.NumberOfDecisions; i++) {
			decisionToDFA[i] = new DFA(_ATN.GetDecisionState(i), i);
		}
	}

		public PGNParser(ITokenStream input) : this(input, Console.Out, Console.Error) { }

		public PGNParser(ITokenStream input, TextWriter output, TextWriter errorOutput)
		: base(input, output, errorOutput)
	{
		Interpreter = new ParserATNSimulator(this, _ATN, decisionToDFA, sharedContextCache);
	}

	public partial class ParseContext : ParserRuleContext {
		public Pgn_databaseContext pgn_database() {
			return GetRuleContext<Pgn_databaseContext>(0);
		}
		public ITerminalNode Eof() { return GetToken(PGNParser.Eof, 0); }
		public ParseContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_parse; } }
		public override void EnterRule(IParseTreeListener listener) {
			IPGNListener typedListener = listener as IPGNListener;
			if (typedListener != null) typedListener.EnterParse(this);
		}
		public override void ExitRule(IParseTreeListener listener) {
			IPGNListener typedListener = listener as IPGNListener;
			if (typedListener != null) typedListener.ExitParse(this);
		}
		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor) {
			IPGNVisitor<TResult> typedVisitor = visitor as IPGNVisitor<TResult>;
			if (typedVisitor != null) return typedVisitor.VisitParse(this);
			else return visitor.VisitChildren(this);
		}
	}

	[RuleVersion(0)]
	public ParseContext parse() {
		ParseContext _localctx = new ParseContext(Context, State);
		EnterRule(_localctx, 0, RULE_parse);
		try {
			EnterOuterAlt(_localctx, 1);
			{
			State = 30; pgn_database();
			State = 31; Match(Eof);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			ErrorHandler.ReportError(this, re);
			ErrorHandler.Recover(this, re);
		}
		finally {
			ExitRule();
		}
		return _localctx;
	}

	public partial class Pgn_databaseContext : ParserRuleContext {
		public Pgn_gameContext[] pgn_game() {
			return GetRuleContexts<Pgn_gameContext>();
		}
		public Pgn_gameContext pgn_game(int i) {
			return GetRuleContext<Pgn_gameContext>(i);
		}
		public Pgn_databaseContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_pgn_database; } }
		public override void EnterRule(IParseTreeListener listener) {
			IPGNListener typedListener = listener as IPGNListener;
			if (typedListener != null) typedListener.EnterPgn_database(this);
		}
		public override void ExitRule(IParseTreeListener listener) {
			IPGNListener typedListener = listener as IPGNListener;
			if (typedListener != null) typedListener.ExitPgn_database(this);
		}
		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor) {
			IPGNVisitor<TResult> typedVisitor = visitor as IPGNVisitor<TResult>;
			if (typedVisitor != null) return typedVisitor.VisitPgn_database(this);
			else return visitor.VisitChildren(this);
		}
	}

	[RuleVersion(0)]
	public Pgn_databaseContext pgn_database() {
		Pgn_databaseContext _localctx = new Pgn_databaseContext(Context, State);
		EnterRule(_localctx, 2, RULE_pgn_database);
		int _la;
		try {
			EnterOuterAlt(_localctx, 1);
			{
			State = 36;
			ErrorHandler.Sync(this);
			_la = TokenStream.LA(1);
			while ((((_la) & ~0x3f) == 0 && ((1L << _la) & ((1L << WHITE_WINS) | (1L << BLACK_WINS) | (1L << DRAWN_GAME) | (1L << INTEGER) | (1L << ASTERISK) | (1L << LEFT_BRACKET) | (1L << LEFT_PARENTHESIS) | (1L << SYMBOL))) != 0)) {
				{
				{
				State = 33; pgn_game();
				}
				}
				State = 38;
				ErrorHandler.Sync(this);
				_la = TokenStream.LA(1);
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			ErrorHandler.ReportError(this, re);
			ErrorHandler.Recover(this, re);
		}
		finally {
			ExitRule();
		}
		return _localctx;
	}

	public partial class Pgn_gameContext : ParserRuleContext {
		public Tag_sectionContext tag_section() {
			return GetRuleContext<Tag_sectionContext>(0);
		}
		public Movetext_sectionContext movetext_section() {
			return GetRuleContext<Movetext_sectionContext>(0);
		}
		public Pgn_gameContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_pgn_game; } }
		public override void EnterRule(IParseTreeListener listener) {
			IPGNListener typedListener = listener as IPGNListener;
			if (typedListener != null) typedListener.EnterPgn_game(this);
		}
		public override void ExitRule(IParseTreeListener listener) {
			IPGNListener typedListener = listener as IPGNListener;
			if (typedListener != null) typedListener.ExitPgn_game(this);
		}
		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor) {
			IPGNVisitor<TResult> typedVisitor = visitor as IPGNVisitor<TResult>;
			if (typedVisitor != null) return typedVisitor.VisitPgn_game(this);
			else return visitor.VisitChildren(this);
		}
	}

	[RuleVersion(0)]
	public Pgn_gameContext pgn_game() {
		Pgn_gameContext _localctx = new Pgn_gameContext(Context, State);
		EnterRule(_localctx, 4, RULE_pgn_game);
		try {
			EnterOuterAlt(_localctx, 1);
			{
			State = 39; tag_section();
			State = 40; movetext_section();
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			ErrorHandler.ReportError(this, re);
			ErrorHandler.Recover(this, re);
		}
		finally {
			ExitRule();
		}
		return _localctx;
	}

	public partial class Tag_sectionContext : ParserRuleContext {
		public Tag_pairContext[] tag_pair() {
			return GetRuleContexts<Tag_pairContext>();
		}
		public Tag_pairContext tag_pair(int i) {
			return GetRuleContext<Tag_pairContext>(i);
		}
		public Tag_sectionContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_tag_section; } }
		public override void EnterRule(IParseTreeListener listener) {
			IPGNListener typedListener = listener as IPGNListener;
			if (typedListener != null) typedListener.EnterTag_section(this);
		}
		public override void ExitRule(IParseTreeListener listener) {
			IPGNListener typedListener = listener as IPGNListener;
			if (typedListener != null) typedListener.ExitTag_section(this);
		}
		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor) {
			IPGNVisitor<TResult> typedVisitor = visitor as IPGNVisitor<TResult>;
			if (typedVisitor != null) return typedVisitor.VisitTag_section(this);
			else return visitor.VisitChildren(this);
		}
	}

	[RuleVersion(0)]
	public Tag_sectionContext tag_section() {
		Tag_sectionContext _localctx = new Tag_sectionContext(Context, State);
		EnterRule(_localctx, 6, RULE_tag_section);
		int _la;
		try {
			EnterOuterAlt(_localctx, 1);
			{
			State = 45;
			ErrorHandler.Sync(this);
			_la = TokenStream.LA(1);
			while (_la==LEFT_BRACKET) {
				{
				{
				State = 42; tag_pair();
				}
				}
				State = 47;
				ErrorHandler.Sync(this);
				_la = TokenStream.LA(1);
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			ErrorHandler.ReportError(this, re);
			ErrorHandler.Recover(this, re);
		}
		finally {
			ExitRule();
		}
		return _localctx;
	}

	public partial class Tag_pairContext : ParserRuleContext {
		public ITerminalNode LEFT_BRACKET() { return GetToken(PGNParser.LEFT_BRACKET, 0); }
		public Tag_nameContext tag_name() {
			return GetRuleContext<Tag_nameContext>(0);
		}
		public Tag_valueContext tag_value() {
			return GetRuleContext<Tag_valueContext>(0);
		}
		public ITerminalNode RIGHT_BRACKET() { return GetToken(PGNParser.RIGHT_BRACKET, 0); }
		public Tag_pairContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_tag_pair; } }
		public override void EnterRule(IParseTreeListener listener) {
			IPGNListener typedListener = listener as IPGNListener;
			if (typedListener != null) typedListener.EnterTag_pair(this);
		}
		public override void ExitRule(IParseTreeListener listener) {
			IPGNListener typedListener = listener as IPGNListener;
			if (typedListener != null) typedListener.ExitTag_pair(this);
		}
		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor) {
			IPGNVisitor<TResult> typedVisitor = visitor as IPGNVisitor<TResult>;
			if (typedVisitor != null) return typedVisitor.VisitTag_pair(this);
			else return visitor.VisitChildren(this);
		}
	}

	[RuleVersion(0)]
	public Tag_pairContext tag_pair() {
		Tag_pairContext _localctx = new Tag_pairContext(Context, State);
		EnterRule(_localctx, 8, RULE_tag_pair);
		try {
			EnterOuterAlt(_localctx, 1);
			{
			State = 48; Match(LEFT_BRACKET);
			State = 49; tag_name();
			State = 50; tag_value();
			State = 51; Match(RIGHT_BRACKET);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			ErrorHandler.ReportError(this, re);
			ErrorHandler.Recover(this, re);
		}
		finally {
			ExitRule();
		}
		return _localctx;
	}

	public partial class Tag_nameContext : ParserRuleContext {
		public ITerminalNode SYMBOL() { return GetToken(PGNParser.SYMBOL, 0); }
		public Tag_nameContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_tag_name; } }
		public override void EnterRule(IParseTreeListener listener) {
			IPGNListener typedListener = listener as IPGNListener;
			if (typedListener != null) typedListener.EnterTag_name(this);
		}
		public override void ExitRule(IParseTreeListener listener) {
			IPGNListener typedListener = listener as IPGNListener;
			if (typedListener != null) typedListener.ExitTag_name(this);
		}
		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor) {
			IPGNVisitor<TResult> typedVisitor = visitor as IPGNVisitor<TResult>;
			if (typedVisitor != null) return typedVisitor.VisitTag_name(this);
			else return visitor.VisitChildren(this);
		}
	}

	[RuleVersion(0)]
	public Tag_nameContext tag_name() {
		Tag_nameContext _localctx = new Tag_nameContext(Context, State);
		EnterRule(_localctx, 10, RULE_tag_name);
		try {
			EnterOuterAlt(_localctx, 1);
			{
			State = 53; Match(SYMBOL);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			ErrorHandler.ReportError(this, re);
			ErrorHandler.Recover(this, re);
		}
		finally {
			ExitRule();
		}
		return _localctx;
	}

	public partial class Tag_valueContext : ParserRuleContext {
		public ITerminalNode STRING() { return GetToken(PGNParser.STRING, 0); }
		public Tag_valueContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_tag_value; } }
		public override void EnterRule(IParseTreeListener listener) {
			IPGNListener typedListener = listener as IPGNListener;
			if (typedListener != null) typedListener.EnterTag_value(this);
		}
		public override void ExitRule(IParseTreeListener listener) {
			IPGNListener typedListener = listener as IPGNListener;
			if (typedListener != null) typedListener.ExitTag_value(this);
		}
		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor) {
			IPGNVisitor<TResult> typedVisitor = visitor as IPGNVisitor<TResult>;
			if (typedVisitor != null) return typedVisitor.VisitTag_value(this);
			else return visitor.VisitChildren(this);
		}
	}

	[RuleVersion(0)]
	public Tag_valueContext tag_value() {
		Tag_valueContext _localctx = new Tag_valueContext(Context, State);
		EnterRule(_localctx, 12, RULE_tag_value);
		try {
			EnterOuterAlt(_localctx, 1);
			{
			State = 55; Match(STRING);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			ErrorHandler.ReportError(this, re);
			ErrorHandler.Recover(this, re);
		}
		finally {
			ExitRule();
		}
		return _localctx;
	}

	public partial class Movetext_sectionContext : ParserRuleContext {
		public Element_sequenceContext element_sequence() {
			return GetRuleContext<Element_sequenceContext>(0);
		}
		public Game_terminationContext game_termination() {
			return GetRuleContext<Game_terminationContext>(0);
		}
		public Movetext_sectionContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_movetext_section; } }
		public override void EnterRule(IParseTreeListener listener) {
			IPGNListener typedListener = listener as IPGNListener;
			if (typedListener != null) typedListener.EnterMovetext_section(this);
		}
		public override void ExitRule(IParseTreeListener listener) {
			IPGNListener typedListener = listener as IPGNListener;
			if (typedListener != null) typedListener.ExitMovetext_section(this);
		}
		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor) {
			IPGNVisitor<TResult> typedVisitor = visitor as IPGNVisitor<TResult>;
			if (typedVisitor != null) return typedVisitor.VisitMovetext_section(this);
			else return visitor.VisitChildren(this);
		}
	}

	[RuleVersion(0)]
	public Movetext_sectionContext movetext_section() {
		Movetext_sectionContext _localctx = new Movetext_sectionContext(Context, State);
		EnterRule(_localctx, 14, RULE_movetext_section);
		try {
			EnterOuterAlt(_localctx, 1);
			{
			State = 57; element_sequence();
			State = 58; game_termination();
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			ErrorHandler.ReportError(this, re);
			ErrorHandler.Recover(this, re);
		}
		finally {
			ExitRule();
		}
		return _localctx;
	}

	public partial class Element_sequenceContext : ParserRuleContext {
		public ElementContext[] element() {
			return GetRuleContexts<ElementContext>();
		}
		public ElementContext element(int i) {
			return GetRuleContext<ElementContext>(i);
		}
		public Recursive_variationContext[] recursive_variation() {
			return GetRuleContexts<Recursive_variationContext>();
		}
		public Recursive_variationContext recursive_variation(int i) {
			return GetRuleContext<Recursive_variationContext>(i);
		}
		public Element_sequenceContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_element_sequence; } }
		public override void EnterRule(IParseTreeListener listener) {
			IPGNListener typedListener = listener as IPGNListener;
			if (typedListener != null) typedListener.EnterElement_sequence(this);
		}
		public override void ExitRule(IParseTreeListener listener) {
			IPGNListener typedListener = listener as IPGNListener;
			if (typedListener != null) typedListener.ExitElement_sequence(this);
		}
		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor) {
			IPGNVisitor<TResult> typedVisitor = visitor as IPGNVisitor<TResult>;
			if (typedVisitor != null) return typedVisitor.VisitElement_sequence(this);
			else return visitor.VisitChildren(this);
		}
	}

	[RuleVersion(0)]
	public Element_sequenceContext element_sequence() {
		Element_sequenceContext _localctx = new Element_sequenceContext(Context, State);
		EnterRule(_localctx, 16, RULE_element_sequence);
		int _la;
		try {
			EnterOuterAlt(_localctx, 1);
			{
			State = 64;
			ErrorHandler.Sync(this);
			_la = TokenStream.LA(1);
			while ((((_la) & ~0x3f) == 0 && ((1L << _la) & ((1L << INTEGER) | (1L << LEFT_PARENTHESIS) | (1L << SYMBOL))) != 0)) {
				{
				State = 62;
				ErrorHandler.Sync(this);
				switch (TokenStream.LA(1)) {
				case INTEGER:
				case SYMBOL:
					{
					State = 60; element();
					}
					break;
				case LEFT_PARENTHESIS:
					{
					State = 61; recursive_variation();
					}
					break;
				default:
					throw new NoViableAltException(this);
				}
				}
				State = 66;
				ErrorHandler.Sync(this);
				_la = TokenStream.LA(1);
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			ErrorHandler.ReportError(this, re);
			ErrorHandler.Recover(this, re);
		}
		finally {
			ExitRule();
		}
		return _localctx;
	}

	public partial class ElementContext : ParserRuleContext {
		public Move_number_indicationContext move_number_indication() {
			return GetRuleContext<Move_number_indicationContext>(0);
		}
		public San_moveContext san_move() {
			return GetRuleContext<San_moveContext>(0);
		}
		public Nag_itemContext nag_item() {
			return GetRuleContext<Nag_itemContext>(0);
		}
		public ElementContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_element; } }
		public override void EnterRule(IParseTreeListener listener) {
			IPGNListener typedListener = listener as IPGNListener;
			if (typedListener != null) typedListener.EnterElement(this);
		}
		public override void ExitRule(IParseTreeListener listener) {
			IPGNListener typedListener = listener as IPGNListener;
			if (typedListener != null) typedListener.ExitElement(this);
		}
		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor) {
			IPGNVisitor<TResult> typedVisitor = visitor as IPGNVisitor<TResult>;
			if (typedVisitor != null) return typedVisitor.VisitElement(this);
			else return visitor.VisitChildren(this);
		}
	}

	[RuleVersion(0)]
	public ElementContext element() {
		ElementContext _localctx = new ElementContext(Context, State);
		EnterRule(_localctx, 18, RULE_element);
		int _la;
		try {
			State = 72;
			ErrorHandler.Sync(this);
			switch (TokenStream.LA(1)) {
			case INTEGER:
				EnterOuterAlt(_localctx, 1);
				{
				State = 67; move_number_indication();
				}
				break;
			case SYMBOL:
				EnterOuterAlt(_localctx, 2);
				{
				State = 68; san_move();
				State = 70;
				ErrorHandler.Sync(this);
				_la = TokenStream.LA(1);
				if (_la==NUMERIC_ANNOTATION_GLYPH) {
					{
					State = 69; nag_item();
					}
				}

				}
				break;
			default:
				throw new NoViableAltException(this);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			ErrorHandler.ReportError(this, re);
			ErrorHandler.Recover(this, re);
		}
		finally {
			ExitRule();
		}
		return _localctx;
	}

	public partial class Nag_itemContext : ParserRuleContext {
		public ITerminalNode NUMERIC_ANNOTATION_GLYPH() { return GetToken(PGNParser.NUMERIC_ANNOTATION_GLYPH, 0); }
		public Nag_itemContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_nag_item; } }
		public override void EnterRule(IParseTreeListener listener) {
			IPGNListener typedListener = listener as IPGNListener;
			if (typedListener != null) typedListener.EnterNag_item(this);
		}
		public override void ExitRule(IParseTreeListener listener) {
			IPGNListener typedListener = listener as IPGNListener;
			if (typedListener != null) typedListener.ExitNag_item(this);
		}
		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor) {
			IPGNVisitor<TResult> typedVisitor = visitor as IPGNVisitor<TResult>;
			if (typedVisitor != null) return typedVisitor.VisitNag_item(this);
			else return visitor.VisitChildren(this);
		}
	}

	[RuleVersion(0)]
	public Nag_itemContext nag_item() {
		Nag_itemContext _localctx = new Nag_itemContext(Context, State);
		EnterRule(_localctx, 20, RULE_nag_item);
		try {
			EnterOuterAlt(_localctx, 1);
			{
			State = 74; Match(NUMERIC_ANNOTATION_GLYPH);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			ErrorHandler.ReportError(this, re);
			ErrorHandler.Recover(this, re);
		}
		finally {
			ExitRule();
		}
		return _localctx;
	}

	public partial class Move_number_indicationContext : ParserRuleContext {
		public ITerminalNode INTEGER() { return GetToken(PGNParser.INTEGER, 0); }
		public ITerminalNode PERIOD() { return GetToken(PGNParser.PERIOD, 0); }
		public Move_number_indicationContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_move_number_indication; } }
		public override void EnterRule(IParseTreeListener listener) {
			IPGNListener typedListener = listener as IPGNListener;
			if (typedListener != null) typedListener.EnterMove_number_indication(this);
		}
		public override void ExitRule(IParseTreeListener listener) {
			IPGNListener typedListener = listener as IPGNListener;
			if (typedListener != null) typedListener.ExitMove_number_indication(this);
		}
		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor) {
			IPGNVisitor<TResult> typedVisitor = visitor as IPGNVisitor<TResult>;
			if (typedVisitor != null) return typedVisitor.VisitMove_number_indication(this);
			else return visitor.VisitChildren(this);
		}
	}

	[RuleVersion(0)]
	public Move_number_indicationContext move_number_indication() {
		Move_number_indicationContext _localctx = new Move_number_indicationContext(Context, State);
		EnterRule(_localctx, 22, RULE_move_number_indication);
		int _la;
		try {
			EnterOuterAlt(_localctx, 1);
			{
			State = 76; Match(INTEGER);
			State = 78;
			ErrorHandler.Sync(this);
			_la = TokenStream.LA(1);
			if (_la==PERIOD) {
				{
				State = 77; Match(PERIOD);
				}
			}

			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			ErrorHandler.ReportError(this, re);
			ErrorHandler.Recover(this, re);
		}
		finally {
			ExitRule();
		}
		return _localctx;
	}

	public partial class San_moveContext : ParserRuleContext {
		public ITerminalNode SYMBOL() { return GetToken(PGNParser.SYMBOL, 0); }
		public San_moveContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_san_move; } }
		public override void EnterRule(IParseTreeListener listener) {
			IPGNListener typedListener = listener as IPGNListener;
			if (typedListener != null) typedListener.EnterSan_move(this);
		}
		public override void ExitRule(IParseTreeListener listener) {
			IPGNListener typedListener = listener as IPGNListener;
			if (typedListener != null) typedListener.ExitSan_move(this);
		}
		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor) {
			IPGNVisitor<TResult> typedVisitor = visitor as IPGNVisitor<TResult>;
			if (typedVisitor != null) return typedVisitor.VisitSan_move(this);
			else return visitor.VisitChildren(this);
		}
	}

	[RuleVersion(0)]
	public San_moveContext san_move() {
		San_moveContext _localctx = new San_moveContext(Context, State);
		EnterRule(_localctx, 24, RULE_san_move);
		try {
			EnterOuterAlt(_localctx, 1);
			{
			State = 80; Match(SYMBOL);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			ErrorHandler.ReportError(this, re);
			ErrorHandler.Recover(this, re);
		}
		finally {
			ExitRule();
		}
		return _localctx;
	}

	public partial class Recursive_variationContext : ParserRuleContext {
		public ITerminalNode LEFT_PARENTHESIS() { return GetToken(PGNParser.LEFT_PARENTHESIS, 0); }
		public Element_sequenceContext element_sequence() {
			return GetRuleContext<Element_sequenceContext>(0);
		}
		public ITerminalNode RIGHT_PARENTHESIS() { return GetToken(PGNParser.RIGHT_PARENTHESIS, 0); }
		public Recursive_variationContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_recursive_variation; } }
		public override void EnterRule(IParseTreeListener listener) {
			IPGNListener typedListener = listener as IPGNListener;
			if (typedListener != null) typedListener.EnterRecursive_variation(this);
		}
		public override void ExitRule(IParseTreeListener listener) {
			IPGNListener typedListener = listener as IPGNListener;
			if (typedListener != null) typedListener.ExitRecursive_variation(this);
		}
		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor) {
			IPGNVisitor<TResult> typedVisitor = visitor as IPGNVisitor<TResult>;
			if (typedVisitor != null) return typedVisitor.VisitRecursive_variation(this);
			else return visitor.VisitChildren(this);
		}
	}

	[RuleVersion(0)]
	public Recursive_variationContext recursive_variation() {
		Recursive_variationContext _localctx = new Recursive_variationContext(Context, State);
		EnterRule(_localctx, 26, RULE_recursive_variation);
		try {
			EnterOuterAlt(_localctx, 1);
			{
			State = 82; Match(LEFT_PARENTHESIS);
			State = 83; element_sequence();
			State = 84; Match(RIGHT_PARENTHESIS);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			ErrorHandler.ReportError(this, re);
			ErrorHandler.Recover(this, re);
		}
		finally {
			ExitRule();
		}
		return _localctx;
	}

	public partial class Game_terminationContext : ParserRuleContext {
		public ITerminalNode WHITE_WINS() { return GetToken(PGNParser.WHITE_WINS, 0); }
		public ITerminalNode BLACK_WINS() { return GetToken(PGNParser.BLACK_WINS, 0); }
		public ITerminalNode DRAWN_GAME() { return GetToken(PGNParser.DRAWN_GAME, 0); }
		public ITerminalNode ASTERISK() { return GetToken(PGNParser.ASTERISK, 0); }
		public Game_terminationContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_game_termination; } }
		public override void EnterRule(IParseTreeListener listener) {
			IPGNListener typedListener = listener as IPGNListener;
			if (typedListener != null) typedListener.EnterGame_termination(this);
		}
		public override void ExitRule(IParseTreeListener listener) {
			IPGNListener typedListener = listener as IPGNListener;
			if (typedListener != null) typedListener.ExitGame_termination(this);
		}
		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor) {
			IPGNVisitor<TResult> typedVisitor = visitor as IPGNVisitor<TResult>;
			if (typedVisitor != null) return typedVisitor.VisitGame_termination(this);
			else return visitor.VisitChildren(this);
		}
	}

	[RuleVersion(0)]
	public Game_terminationContext game_termination() {
		Game_terminationContext _localctx = new Game_terminationContext(Context, State);
		EnterRule(_localctx, 28, RULE_game_termination);
		int _la;
		try {
			EnterOuterAlt(_localctx, 1);
			{
			State = 86;
			_la = TokenStream.LA(1);
			if ( !((((_la) & ~0x3f) == 0 && ((1L << _la) & ((1L << WHITE_WINS) | (1L << BLACK_WINS) | (1L << DRAWN_GAME) | (1L << ASTERISK))) != 0)) ) {
			ErrorHandler.RecoverInline(this);
			}
			else {
				ErrorHandler.ReportMatch(this);
			    Consume();
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			ErrorHandler.ReportError(this, re);
			ErrorHandler.Recover(this, re);
		}
		finally {
			ExitRule();
		}
		return _localctx;
	}

	private static char[] _serializedATN = {
		'\x3', '\x608B', '\xA72A', '\x8133', '\xB9ED', '\x417C', '\x3BE7', '\x7786', 
		'\x5964', '\x3', '\x1A', '[', '\x4', '\x2', '\t', '\x2', '\x4', '\x3', 
		'\t', '\x3', '\x4', '\x4', '\t', '\x4', '\x4', '\x5', '\t', '\x5', '\x4', 
		'\x6', '\t', '\x6', '\x4', '\a', '\t', '\a', '\x4', '\b', '\t', '\b', 
		'\x4', '\t', '\t', '\t', '\x4', '\n', '\t', '\n', '\x4', '\v', '\t', '\v', 
		'\x4', '\f', '\t', '\f', '\x4', '\r', '\t', '\r', '\x4', '\xE', '\t', 
		'\xE', '\x4', '\xF', '\t', '\xF', '\x4', '\x10', '\t', '\x10', '\x3', 
		'\x2', '\x3', '\x2', '\x3', '\x2', '\x3', '\x3', '\a', '\x3', '%', '\n', 
		'\x3', '\f', '\x3', '\xE', '\x3', '(', '\v', '\x3', '\x3', '\x4', '\x3', 
		'\x4', '\x3', '\x4', '\x3', '\x5', '\a', '\x5', '.', '\n', '\x5', '\f', 
		'\x5', '\xE', '\x5', '\x31', '\v', '\x5', '\x3', '\x6', '\x3', '\x6', 
		'\x3', '\x6', '\x3', '\x6', '\x3', '\x6', '\x3', '\a', '\x3', '\a', '\x3', 
		'\b', '\x3', '\b', '\x3', '\t', '\x3', '\t', '\x3', '\t', '\x3', '\n', 
		'\x3', '\n', '\a', '\n', '\x41', '\n', '\n', '\f', '\n', '\xE', '\n', 
		'\x44', '\v', '\n', '\x3', '\v', '\x3', '\v', '\x3', '\v', '\x5', '\v', 
		'I', '\n', '\v', '\x5', '\v', 'K', '\n', '\v', '\x3', '\f', '\x3', '\f', 
		'\x3', '\r', '\x3', '\r', '\x5', '\r', 'Q', '\n', '\r', '\x3', '\xE', 
		'\x3', '\xE', '\x3', '\xF', '\x3', '\xF', '\x3', '\xF', '\x3', '\xF', 
		'\x3', '\x10', '\x3', '\x10', '\x3', '\x10', '\x2', '\x2', '\x11', '\x2', 
		'\x4', '\x6', '\b', '\n', '\f', '\xE', '\x10', '\x12', '\x14', '\x16', 
		'\x18', '\x1A', '\x1C', '\x1E', '\x2', '\x3', '\x4', '\x2', '\x3', '\x5', 
		'\x10', '\x10', '\x2', 'R', '\x2', ' ', '\x3', '\x2', '\x2', '\x2', '\x4', 
		'&', '\x3', '\x2', '\x2', '\x2', '\x6', ')', '\x3', '\x2', '\x2', '\x2', 
		'\b', '/', '\x3', '\x2', '\x2', '\x2', '\n', '\x32', '\x3', '\x2', '\x2', 
		'\x2', '\f', '\x37', '\x3', '\x2', '\x2', '\x2', '\xE', '\x39', '\x3', 
		'\x2', '\x2', '\x2', '\x10', ';', '\x3', '\x2', '\x2', '\x2', '\x12', 
		'\x42', '\x3', '\x2', '\x2', '\x2', '\x14', 'J', '\x3', '\x2', '\x2', 
		'\x2', '\x16', 'L', '\x3', '\x2', '\x2', '\x2', '\x18', 'N', '\x3', '\x2', 
		'\x2', '\x2', '\x1A', 'R', '\x3', '\x2', '\x2', '\x2', '\x1C', 'T', '\x3', 
		'\x2', '\x2', '\x2', '\x1E', 'X', '\x3', '\x2', '\x2', '\x2', ' ', '!', 
		'\x5', '\x4', '\x3', '\x2', '!', '\"', '\a', '\x2', '\x2', '\x3', '\"', 
		'\x3', '\x3', '\x2', '\x2', '\x2', '#', '%', '\x5', '\x6', '\x4', '\x2', 
		'$', '#', '\x3', '\x2', '\x2', '\x2', '%', '(', '\x3', '\x2', '\x2', '\x2', 
		'&', '$', '\x3', '\x2', '\x2', '\x2', '&', '\'', '\x3', '\x2', '\x2', 
		'\x2', '\'', '\x5', '\x3', '\x2', '\x2', '\x2', '(', '&', '\x3', '\x2', 
		'\x2', '\x2', ')', '*', '\x5', '\b', '\x5', '\x2', '*', '+', '\x5', '\x10', 
		'\t', '\x2', '+', '\a', '\x3', '\x2', '\x2', '\x2', ',', '.', '\x5', '\n', 
		'\x6', '\x2', '-', ',', '\x3', '\x2', '\x2', '\x2', '.', '\x31', '\x3', 
		'\x2', '\x2', '\x2', '/', '-', '\x3', '\x2', '\x2', '\x2', '/', '\x30', 
		'\x3', '\x2', '\x2', '\x2', '\x30', '\t', '\x3', '\x2', '\x2', '\x2', 
		'\x31', '/', '\x3', '\x2', '\x2', '\x2', '\x32', '\x33', '\a', '\x11', 
		'\x2', '\x2', '\x33', '\x34', '\x5', '\f', '\a', '\x2', '\x34', '\x35', 
		'\x5', '\xE', '\b', '\x2', '\x35', '\x36', '\a', '\x12', '\x2', '\x2', 
		'\x36', '\v', '\x3', '\x2', '\x2', '\x2', '\x37', '\x38', '\a', '\x18', 
		'\x2', '\x2', '\x38', '\r', '\x3', '\x2', '\x2', '\x2', '\x39', ':', '\a', 
		'\r', '\x2', '\x2', ':', '\xF', '\x3', '\x2', '\x2', '\x2', ';', '<', 
		'\x5', '\x12', '\n', '\x2', '<', '=', '\x5', '\x1E', '\x10', '\x2', '=', 
		'\x11', '\x3', '\x2', '\x2', '\x2', '>', '\x41', '\x5', '\x14', '\v', 
		'\x2', '?', '\x41', '\x5', '\x1C', '\xF', '\x2', '@', '>', '\x3', '\x2', 
		'\x2', '\x2', '@', '?', '\x3', '\x2', '\x2', '\x2', '\x41', '\x44', '\x3', 
		'\x2', '\x2', '\x2', '\x42', '@', '\x3', '\x2', '\x2', '\x2', '\x42', 
		'\x43', '\x3', '\x2', '\x2', '\x2', '\x43', '\x13', '\x3', '\x2', '\x2', 
		'\x2', '\x44', '\x42', '\x3', '\x2', '\x2', '\x2', '\x45', 'K', '\x5', 
		'\x18', '\r', '\x2', '\x46', 'H', '\x5', '\x1A', '\xE', '\x2', 'G', 'I', 
		'\x5', '\x16', '\f', '\x2', 'H', 'G', '\x3', '\x2', '\x2', '\x2', 'H', 
		'I', '\x3', '\x2', '\x2', '\x2', 'I', 'K', '\x3', '\x2', '\x2', '\x2', 
		'J', '\x45', '\x3', '\x2', '\x2', '\x2', 'J', '\x46', '\x3', '\x2', '\x2', 
		'\x2', 'K', '\x15', '\x3', '\x2', '\x2', '\x2', 'L', 'M', '\a', '\x17', 
		'\x2', '\x2', 'M', '\x17', '\x3', '\x2', '\x2', '\x2', 'N', 'P', '\a', 
		'\xE', '\x2', '\x2', 'O', 'Q', '\a', '\xF', '\x2', '\x2', 'P', 'O', '\x3', 
		'\x2', '\x2', '\x2', 'P', 'Q', '\x3', '\x2', '\x2', '\x2', 'Q', '\x19', 
		'\x3', '\x2', '\x2', '\x2', 'R', 'S', '\a', '\x18', '\x2', '\x2', 'S', 
		'\x1B', '\x3', '\x2', '\x2', '\x2', 'T', 'U', '\a', '\x13', '\x2', '\x2', 
		'U', 'V', '\x5', '\x12', '\n', '\x2', 'V', 'W', '\a', '\x14', '\x2', '\x2', 
		'W', '\x1D', '\x3', '\x2', '\x2', '\x2', 'X', 'Y', '\t', '\x2', '\x2', 
		'\x2', 'Y', '\x1F', '\x3', '\x2', '\x2', '\x2', '\t', '&', '/', '@', '\x42', 
		'H', 'J', 'P',
	};

	public static readonly ATN _ATN =
		new ATNDeserializer().Deserialize(_serializedATN);


}
} // namespace ChessLib.Parse.PGN.Parser.BaseClasses