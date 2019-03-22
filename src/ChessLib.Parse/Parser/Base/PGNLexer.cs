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

namespace ChessLib.Parse.Parser.Base {
using System;
using System.IO;
using System.Text;
using Antlr4.Runtime;
using Antlr4.Runtime.Atn;
using Antlr4.Runtime.Misc;
using DFA = Antlr4.Runtime.Dfa.DFA;

[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.7.2")]
[System.CLSCompliant(false)]
public partial class PGNLexer : Lexer {
	protected static DFA[] decisionToDFA;
	protected static PredictionContextCache sharedContextCache = new PredictionContextCache();
	public const int
		WHITE_WINS=1, BLACK_WINS=2, DRAWN_GAME=3, BOL=4, REST_OF_LINE_COMMENT=5, 
		BRACE_COMMENT=6, ESCAPE=7, NEW_LINE=8, SECTION_SEPARATOR=9, SPACES=10, 
		STRING=11, INTEGER=12, PERIOD=13, ASTERISK=14, LEFT_BRACKET=15, RIGHT_BRACKET=16, 
		LEFT_PARENTHESIS=17, RIGHT_PARENTHESIS=18, LEFT_ANGLE_BRACKET=19, RIGHT_ANGLE_BRACKET=20, 
		NUMERIC_ANNOTATION_GLYPH=21, SYMBOL=22, SUFFIX_ANNOTATION=23, UNEXPECTED_CHAR=24;
	public static string[] channelNames = {
		"DEFAULT_TOKEN_CHANNEL", "HIDDEN"
	};

	public static string[] modeNames = {
		"DEFAULT_MODE"
	};

	public static readonly string[] ruleNames = {
		"WHITE_WINS", "BLACK_WINS", "DRAWN_GAME", "BOL", "REST_OF_LINE_COMMENT", 
		"BRACE_COMMENT", "ESCAPE", "NEW_LINE", "SECTION_SEPARATOR", "SPACES", 
		"STRING", "INTEGER", "PERIOD", "ASTERISK", "LEFT_BRACKET", "RIGHT_BRACKET", 
		"LEFT_PARENTHESIS", "RIGHT_PARENTHESIS", "LEFT_ANGLE_BRACKET", "RIGHT_ANGLE_BRACKET", 
		"NUMERIC_ANNOTATION_GLYPH", "SYMBOL", "SUFFIX_ANNOTATION", "UNEXPECTED_CHAR"
	};


	public PGNLexer(ICharStream input)
	: this(input, Console.Out, Console.Error) { }

	public PGNLexer(ICharStream input, TextWriter output, TextWriter errorOutput)
	: base(input, output, errorOutput)
	{
		Interpreter = new LexerATNSimulator(this, _ATN, decisionToDFA, sharedContextCache);
	}

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

	public override string[] ChannelNames { get { return channelNames; } }

	public override string[] ModeNames { get { return modeNames; } }

	public override string SerializedAtn { get { return new string(_serializedATN); } }

	static PGNLexer() {
		decisionToDFA = new DFA[_ATN.NumberOfDecisions];
		for (int i = 0; i < _ATN.NumberOfDecisions; i++) {
			decisionToDFA[i] = new DFA(_ATN.GetDecisionState(i), i);
		}
	}
	private static char[] _serializedATN = {
		'\x3', '\x608B', '\xA72A', '\x8133', '\xB9ED', '\x417C', '\x3BE7', '\x7786', 
		'\x5964', '\x2', '\x1A', '\xB2', '\b', '\x1', '\x4', '\x2', '\t', '\x2', 
		'\x4', '\x3', '\t', '\x3', '\x4', '\x4', '\t', '\x4', '\x4', '\x5', '\t', 
		'\x5', '\x4', '\x6', '\t', '\x6', '\x4', '\a', '\t', '\a', '\x4', '\b', 
		'\t', '\b', '\x4', '\t', '\t', '\t', '\x4', '\n', '\t', '\n', '\x4', '\v', 
		'\t', '\v', '\x4', '\f', '\t', '\f', '\x4', '\r', '\t', '\r', '\x4', '\xE', 
		'\t', '\xE', '\x4', '\xF', '\t', '\xF', '\x4', '\x10', '\t', '\x10', '\x4', 
		'\x11', '\t', '\x11', '\x4', '\x12', '\t', '\x12', '\x4', '\x13', '\t', 
		'\x13', '\x4', '\x14', '\t', '\x14', '\x4', '\x15', '\t', '\x15', '\x4', 
		'\x16', '\t', '\x16', '\x4', '\x17', '\t', '\x17', '\x4', '\x18', '\t', 
		'\x18', '\x4', '\x19', '\t', '\x19', '\x3', '\x2', '\x3', '\x2', '\x3', 
		'\x2', '\x3', '\x2', '\x3', '\x3', '\x3', '\x3', '\x3', '\x3', '\x3', 
		'\x3', '\x3', '\x4', '\x3', '\x4', '\x3', '\x4', '\x3', '\x4', '\x3', 
		'\x4', '\x3', '\x4', '\x3', '\x4', '\x3', '\x4', '\x3', '\x5', '\x6', 
		'\x5', '\x45', '\n', '\x5', '\r', '\x5', '\xE', '\x5', '\x46', '\x3', 
		'\x6', '\x3', '\x6', '\a', '\x6', 'K', '\n', '\x6', '\f', '\x6', '\xE', 
		'\x6', 'N', '\v', '\x6', '\x3', '\x6', '\x3', '\x6', '\x3', '\a', '\x3', 
		'\a', '\a', '\a', 'T', '\n', '\a', '\f', '\a', '\xE', '\a', 'W', '\v', 
		'\a', '\x3', '\a', '\x3', '\a', '\x3', '\a', '\x3', '\a', '\x3', '\b', 
		'\x5', '\b', '^', '\n', '\b', '\x3', '\b', '\x3', '\b', '\a', '\b', '\x62', 
		'\n', '\b', '\f', '\b', '\xE', '\b', '\x65', '\v', '\b', '\x3', '\b', 
		'\x3', '\b', '\x3', '\t', '\x5', '\t', 'j', '\n', '\t', '\x3', '\t', '\x3', 
		'\t', '\x3', '\n', '\x3', '\n', '\x3', '\n', '\x3', '\n', '\x3', '\n', 
		'\x3', '\v', '\x6', '\v', 't', '\n', '\v', '\r', '\v', '\xE', '\v', 'u', 
		'\x3', '\v', '\x3', '\v', '\x3', '\f', '\x3', '\f', '\x3', '\f', '\x3', 
		'\f', '\x3', '\f', '\x3', '\f', '\a', '\f', '\x80', '\n', '\f', '\f', 
		'\f', '\xE', '\f', '\x83', '\v', '\f', '\x3', '\f', '\x3', '\f', '\x3', 
		'\r', '\x6', '\r', '\x88', '\n', '\r', '\r', '\r', '\xE', '\r', '\x89', 
		'\x3', '\xE', '\x3', '\xE', '\x3', '\xE', '\x3', '\xE', '\x5', '\xE', 
		'\x90', '\n', '\xE', '\x3', '\xF', '\x3', '\xF', '\x3', '\x10', '\x3', 
		'\x10', '\x3', '\x11', '\x3', '\x11', '\x3', '\x12', '\x3', '\x12', '\x3', 
		'\x13', '\x3', '\x13', '\x3', '\x14', '\x3', '\x14', '\x3', '\x15', '\x3', 
		'\x15', '\x3', '\x16', '\x3', '\x16', '\x6', '\x16', '\xA2', '\n', '\x16', 
		'\r', '\x16', '\xE', '\x16', '\xA3', '\x3', '\x17', '\x3', '\x17', '\a', 
		'\x17', '\xA8', '\n', '\x17', '\f', '\x17', '\xE', '\x17', '\xAB', '\v', 
		'\x17', '\x3', '\x18', '\x3', '\x18', '\x5', '\x18', '\xAF', '\n', '\x18', 
		'\x3', '\x19', '\x3', '\x19', '\x2', '\x2', '\x1A', '\x3', '\x3', '\x5', 
		'\x4', '\a', '\x5', '\t', '\x6', '\v', '\a', '\r', '\b', '\xF', '\t', 
		'\x11', '\n', '\x13', '\v', '\x15', '\f', '\x17', '\r', '\x19', '\xE', 
		'\x1B', '\xF', '\x1D', '\x10', '\x1F', '\x11', '!', '\x12', '#', '\x13', 
		'%', '\x14', '\'', '\x15', ')', '\x16', '+', '\x17', '-', '\x18', '/', 
		'\x19', '\x31', '\x1A', '\x3', '\x2', '\v', '\x4', '\x2', '\f', '\f', 
		'\xE', '\xF', '\x4', '\x2', '\f', '\f', '\xF', '\xF', '\x3', '\x2', '\x7F', 
		'\x7F', '\x5', '\x2', '\v', '\f', '\xF', '\xF', '\"', '\"', '\x4', '\x2', 
		'$', '$', '^', '^', '\x3', '\x2', '\x32', ';', '\x5', '\x2', '\x32', ';', 
		'\x43', '\\', '\x63', '|', '\n', '\x2', '%', '%', '-', '-', '/', '/', 
		'\x32', '<', '?', '?', '\x43', '\\', '\x61', '\x61', '\x63', '|', '\x4', 
		'\x2', '#', '#', '\x41', '\x41', '\x2', '\xC0', '\x2', '\x3', '\x3', '\x2', 
		'\x2', '\x2', '\x2', '\x5', '\x3', '\x2', '\x2', '\x2', '\x2', '\a', '\x3', 
		'\x2', '\x2', '\x2', '\x2', '\t', '\x3', '\x2', '\x2', '\x2', '\x2', '\v', 
		'\x3', '\x2', '\x2', '\x2', '\x2', '\r', '\x3', '\x2', '\x2', '\x2', '\x2', 
		'\xF', '\x3', '\x2', '\x2', '\x2', '\x2', '\x11', '\x3', '\x2', '\x2', 
		'\x2', '\x2', '\x13', '\x3', '\x2', '\x2', '\x2', '\x2', '\x15', '\x3', 
		'\x2', '\x2', '\x2', '\x2', '\x17', '\x3', '\x2', '\x2', '\x2', '\x2', 
		'\x19', '\x3', '\x2', '\x2', '\x2', '\x2', '\x1B', '\x3', '\x2', '\x2', 
		'\x2', '\x2', '\x1D', '\x3', '\x2', '\x2', '\x2', '\x2', '\x1F', '\x3', 
		'\x2', '\x2', '\x2', '\x2', '!', '\x3', '\x2', '\x2', '\x2', '\x2', '#', 
		'\x3', '\x2', '\x2', '\x2', '\x2', '%', '\x3', '\x2', '\x2', '\x2', '\x2', 
		'\'', '\x3', '\x2', '\x2', '\x2', '\x2', ')', '\x3', '\x2', '\x2', '\x2', 
		'\x2', '+', '\x3', '\x2', '\x2', '\x2', '\x2', '-', '\x3', '\x2', '\x2', 
		'\x2', '\x2', '/', '\x3', '\x2', '\x2', '\x2', '\x2', '\x31', '\x3', '\x2', 
		'\x2', '\x2', '\x3', '\x33', '\x3', '\x2', '\x2', '\x2', '\x5', '\x37', 
		'\x3', '\x2', '\x2', '\x2', '\a', ';', '\x3', '\x2', '\x2', '\x2', '\t', 
		'\x44', '\x3', '\x2', '\x2', '\x2', '\v', 'H', '\x3', '\x2', '\x2', '\x2', 
		'\r', 'Q', '\x3', '\x2', '\x2', '\x2', '\xF', ']', '\x3', '\x2', '\x2', 
		'\x2', '\x11', 'i', '\x3', '\x2', '\x2', '\x2', '\x13', 'm', '\x3', '\x2', 
		'\x2', '\x2', '\x15', 's', '\x3', '\x2', '\x2', '\x2', '\x17', 'y', '\x3', 
		'\x2', '\x2', '\x2', '\x19', '\x87', '\x3', '\x2', '\x2', '\x2', '\x1B', 
		'\x8F', '\x3', '\x2', '\x2', '\x2', '\x1D', '\x91', '\x3', '\x2', '\x2', 
		'\x2', '\x1F', '\x93', '\x3', '\x2', '\x2', '\x2', '!', '\x95', '\x3', 
		'\x2', '\x2', '\x2', '#', '\x97', '\x3', '\x2', '\x2', '\x2', '%', '\x99', 
		'\x3', '\x2', '\x2', '\x2', '\'', '\x9B', '\x3', '\x2', '\x2', '\x2', 
		')', '\x9D', '\x3', '\x2', '\x2', '\x2', '+', '\x9F', '\x3', '\x2', '\x2', 
		'\x2', '-', '\xA5', '\x3', '\x2', '\x2', '\x2', '/', '\xAC', '\x3', '\x2', 
		'\x2', '\x2', '\x31', '\xB0', '\x3', '\x2', '\x2', '\x2', '\x33', '\x34', 
		'\a', '\x33', '\x2', '\x2', '\x34', '\x35', '\a', '/', '\x2', '\x2', '\x35', 
		'\x36', '\a', '\x32', '\x2', '\x2', '\x36', '\x4', '\x3', '\x2', '\x2', 
		'\x2', '\x37', '\x38', '\a', '\x32', '\x2', '\x2', '\x38', '\x39', '\a', 
		'/', '\x2', '\x2', '\x39', ':', '\a', '\x33', '\x2', '\x2', ':', '\x6', 
		'\x3', '\x2', '\x2', '\x2', ';', '<', '\a', '\x33', '\x2', '\x2', '<', 
		'=', '\a', '\x31', '\x2', '\x2', '=', '>', '\a', '\x34', '\x2', '\x2', 
		'>', '?', '\a', '/', '\x2', '\x2', '?', '@', '\a', '\x33', '\x2', '\x2', 
		'@', '\x41', '\a', '\x31', '\x2', '\x2', '\x41', '\x42', '\a', '\x34', 
		'\x2', '\x2', '\x42', '\b', '\x3', '\x2', '\x2', '\x2', '\x43', '\x45', 
		'\t', '\x2', '\x2', '\x2', '\x44', '\x43', '\x3', '\x2', '\x2', '\x2', 
		'\x45', '\x46', '\x3', '\x2', '\x2', '\x2', '\x46', '\x44', '\x3', '\x2', 
		'\x2', '\x2', '\x46', 'G', '\x3', '\x2', '\x2', '\x2', 'G', '\n', '\x3', 
		'\x2', '\x2', '\x2', 'H', 'L', '\a', '=', '\x2', '\x2', 'I', 'K', '\n', 
		'\x3', '\x2', '\x2', 'J', 'I', '\x3', '\x2', '\x2', '\x2', 'K', 'N', '\x3', 
		'\x2', '\x2', '\x2', 'L', 'J', '\x3', '\x2', '\x2', '\x2', 'L', 'M', '\x3', 
		'\x2', '\x2', '\x2', 'M', 'O', '\x3', '\x2', '\x2', '\x2', 'N', 'L', '\x3', 
		'\x2', '\x2', '\x2', 'O', 'P', '\b', '\x6', '\x2', '\x2', 'P', '\f', '\x3', 
		'\x2', '\x2', '\x2', 'Q', 'U', '\a', '}', '\x2', '\x2', 'R', 'T', '\n', 
		'\x4', '\x2', '\x2', 'S', 'R', '\x3', '\x2', '\x2', '\x2', 'T', 'W', '\x3', 
		'\x2', '\x2', '\x2', 'U', 'S', '\x3', '\x2', '\x2', '\x2', 'U', 'V', '\x3', 
		'\x2', '\x2', '\x2', 'V', 'X', '\x3', '\x2', '\x2', '\x2', 'W', 'U', '\x3', 
		'\x2', '\x2', '\x2', 'X', 'Y', '\a', '\x7F', '\x2', '\x2', 'Y', 'Z', '\x3', 
		'\x2', '\x2', '\x2', 'Z', '[', '\b', '\a', '\x2', '\x2', '[', '\xE', '\x3', 
		'\x2', '\x2', '\x2', '\\', '^', '\x5', '\t', '\x5', '\x2', ']', '\\', 
		'\x3', '\x2', '\x2', '\x2', ']', '^', '\x3', '\x2', '\x2', '\x2', '^', 
		'_', '\x3', '\x2', '\x2', '\x2', '_', '\x63', '\a', '\'', '\x2', '\x2', 
		'`', '\x62', '\n', '\x3', '\x2', '\x2', '\x61', '`', '\x3', '\x2', '\x2', 
		'\x2', '\x62', '\x65', '\x3', '\x2', '\x2', '\x2', '\x63', '\x61', '\x3', 
		'\x2', '\x2', '\x2', '\x63', '\x64', '\x3', '\x2', '\x2', '\x2', '\x64', 
		'\x66', '\x3', '\x2', '\x2', '\x2', '\x65', '\x63', '\x3', '\x2', '\x2', 
		'\x2', '\x66', 'g', '\b', '\b', '\x2', '\x2', 'g', '\x10', '\x3', '\x2', 
		'\x2', '\x2', 'h', 'j', '\a', '\xF', '\x2', '\x2', 'i', 'h', '\x3', '\x2', 
		'\x2', '\x2', 'i', 'j', '\x3', '\x2', '\x2', '\x2', 'j', 'k', '\x3', '\x2', 
		'\x2', '\x2', 'k', 'l', '\a', '\f', '\x2', '\x2', 'l', '\x12', '\x3', 
		'\x2', '\x2', '\x2', 'm', 'n', '\x5', '\x11', '\t', '\x2', 'n', 'o', '\x5', 
		'\x11', '\t', '\x2', 'o', 'p', '\x3', '\x2', '\x2', '\x2', 'p', 'q', '\b', 
		'\n', '\x2', '\x2', 'q', '\x14', '\x3', '\x2', '\x2', '\x2', 'r', 't', 
		'\t', '\x5', '\x2', '\x2', 's', 'r', '\x3', '\x2', '\x2', '\x2', 't', 
		'u', '\x3', '\x2', '\x2', '\x2', 'u', 's', '\x3', '\x2', '\x2', '\x2', 
		'u', 'v', '\x3', '\x2', '\x2', '\x2', 'v', 'w', '\x3', '\x2', '\x2', '\x2', 
		'w', 'x', '\b', '\v', '\x2', '\x2', 'x', '\x16', '\x3', '\x2', '\x2', 
		'\x2', 'y', '\x81', '\a', '$', '\x2', '\x2', 'z', '{', '\a', '^', '\x2', 
		'\x2', '{', '\x80', '\a', '^', '\x2', '\x2', '|', '}', '\a', '^', '\x2', 
		'\x2', '}', '\x80', '\a', '$', '\x2', '\x2', '~', '\x80', '\n', '\x6', 
		'\x2', '\x2', '\x7F', 'z', '\x3', '\x2', '\x2', '\x2', '\x7F', '|', '\x3', 
		'\x2', '\x2', '\x2', '\x7F', '~', '\x3', '\x2', '\x2', '\x2', '\x80', 
		'\x83', '\x3', '\x2', '\x2', '\x2', '\x81', '\x7F', '\x3', '\x2', '\x2', 
		'\x2', '\x81', '\x82', '\x3', '\x2', '\x2', '\x2', '\x82', '\x84', '\x3', 
		'\x2', '\x2', '\x2', '\x83', '\x81', '\x3', '\x2', '\x2', '\x2', '\x84', 
		'\x85', '\a', '$', '\x2', '\x2', '\x85', '\x18', '\x3', '\x2', '\x2', 
		'\x2', '\x86', '\x88', '\t', '\a', '\x2', '\x2', '\x87', '\x86', '\x3', 
		'\x2', '\x2', '\x2', '\x88', '\x89', '\x3', '\x2', '\x2', '\x2', '\x89', 
		'\x87', '\x3', '\x2', '\x2', '\x2', '\x89', '\x8A', '\x3', '\x2', '\x2', 
		'\x2', '\x8A', '\x1A', '\x3', '\x2', '\x2', '\x2', '\x8B', '\x90', '\a', 
		'\x30', '\x2', '\x2', '\x8C', '\x8D', '\a', '\x30', '\x2', '\x2', '\x8D', 
		'\x8E', '\a', '\x30', '\x2', '\x2', '\x8E', '\x90', '\a', '\x30', '\x2', 
		'\x2', '\x8F', '\x8B', '\x3', '\x2', '\x2', '\x2', '\x8F', '\x8C', '\x3', 
		'\x2', '\x2', '\x2', '\x90', '\x1C', '\x3', '\x2', '\x2', '\x2', '\x91', 
		'\x92', '\a', ',', '\x2', '\x2', '\x92', '\x1E', '\x3', '\x2', '\x2', 
		'\x2', '\x93', '\x94', '\a', ']', '\x2', '\x2', '\x94', ' ', '\x3', '\x2', 
		'\x2', '\x2', '\x95', '\x96', '\a', '_', '\x2', '\x2', '\x96', '\"', '\x3', 
		'\x2', '\x2', '\x2', '\x97', '\x98', '\a', '*', '\x2', '\x2', '\x98', 
		'$', '\x3', '\x2', '\x2', '\x2', '\x99', '\x9A', '\a', '+', '\x2', '\x2', 
		'\x9A', '&', '\x3', '\x2', '\x2', '\x2', '\x9B', '\x9C', '\a', '>', '\x2', 
		'\x2', '\x9C', '(', '\x3', '\x2', '\x2', '\x2', '\x9D', '\x9E', '\a', 
		'@', '\x2', '\x2', '\x9E', '*', '\x3', '\x2', '\x2', '\x2', '\x9F', '\xA1', 
		'\a', '&', '\x2', '\x2', '\xA0', '\xA2', '\t', '\a', '\x2', '\x2', '\xA1', 
		'\xA0', '\x3', '\x2', '\x2', '\x2', '\xA2', '\xA3', '\x3', '\x2', '\x2', 
		'\x2', '\xA3', '\xA1', '\x3', '\x2', '\x2', '\x2', '\xA3', '\xA4', '\x3', 
		'\x2', '\x2', '\x2', '\xA4', ',', '\x3', '\x2', '\x2', '\x2', '\xA5', 
		'\xA9', '\t', '\b', '\x2', '\x2', '\xA6', '\xA8', '\t', '\t', '\x2', '\x2', 
		'\xA7', '\xA6', '\x3', '\x2', '\x2', '\x2', '\xA8', '\xAB', '\x3', '\x2', 
		'\x2', '\x2', '\xA9', '\xA7', '\x3', '\x2', '\x2', '\x2', '\xA9', '\xAA', 
		'\x3', '\x2', '\x2', '\x2', '\xAA', '.', '\x3', '\x2', '\x2', '\x2', '\xAB', 
		'\xA9', '\x3', '\x2', '\x2', '\x2', '\xAC', '\xAE', '\t', '\n', '\x2', 
		'\x2', '\xAD', '\xAF', '\t', '\n', '\x2', '\x2', '\xAE', '\xAD', '\x3', 
		'\x2', '\x2', '\x2', '\xAE', '\xAF', '\x3', '\x2', '\x2', '\x2', '\xAF', 
		'\x30', '\x3', '\x2', '\x2', '\x2', '\xB0', '\xB1', '\v', '\x2', '\x2', 
		'\x2', '\xB1', '\x32', '\x3', '\x2', '\x2', '\x2', '\x11', '\x2', '\x46', 
		'L', 'U', ']', '\x63', 'i', 'u', '\x7F', '\x81', '\x89', '\x8F', '\xA3', 
		'\xA9', '\xAE', '\x3', '\b', '\x2', '\x2',
	};

	public static readonly ATN _ATN =
		new ATNDeserializer().Deserialize(_serializedATN);


}
} // namespace ChessLib.Parse.Parser.Base
