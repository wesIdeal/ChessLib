﻿using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using ChessLib.UCI;
namespace ChessLib.UCI.Tests
{
    [TestFixture]
    public class UCIOptionTests
    {
        [TestCase("", UCIOptionType.Null)]
        [TestCase(null, UCIOptionType.Null)]
        [TestCase("hi", UCIOptionType.Null)]
        [TestCase("type string", UCIOptionType.String)]
        [TestCase("type spin", UCIOptionType.Spin)]
        [TestCase("type check", UCIOptionType.Check)]
        [TestCase("type combo", UCIOptionType.Combo)]
        [TestCase("type button", UCIOptionType.Button)]
        [TestCase("option name Debug Log File type string default", UCIOptionType.String)]
        public void GetType(string options, UCIOptionType expectedType)
        {
            var actualType = EngineHelpers.GetOptionType(options);
            Assert.AreEqual(expectedType, actualType);
        }

        [TestCase("option name Debug Log File type string default", "Debug Log File")]
        [TestCase("option name Contempt type spin default 24 min -100 max 100", "Contempt")]
        [TestCase("option name Analysis Contempt type combo default Both var Off var White var Black var Both", "Analysis Contempt")]
        [TestCase("option name Threads type spin default 1 min 1 max 512", "Threads")]
        [TestCase("option name Hash type spin default 16 min 1 max 131072", "Hash")]
        [TestCase("option name Clear Hash type button", "Clear Hash")]
        [TestCase("option name Ponder type check default false", "Ponder")]
        [TestCase("option name MultiPV type spin default 1 min 1 max 500", "MultiPV")]
        public void GetOptionName(string option, string expected)
        {
            var actual = option.GetOptionName();
            Assert.AreEqual(expected, actual);
        }

        [TestCase("option name Analysis Contempt type combo default Both var Off var White var Black var Both", new string[] { "Off", "White", "Black", "Both" })]
        [TestCase("option name Analysis Contempt type combo default Both", new string[] { })]
        public void GetComboValues(string option, string[] expected)
        {
            var actual = option.GetComboOptionValues();
            Assert.AreEqual(expected, actual);
        }

        [TestCase("option name Analysis Contempt type combo default Both var Off var White var Black var Both", null)]
        [TestCase("option name Contempt type spin default 24 min -100 max 100", 24)]
        [TestCase("option name Move Overhead type spin default 30 min 0 max 5000", 30)]
        [TestCase("option name Contempt type spin default 24 min -100 max 100", 24)]
        [TestCase("option name Move Overhead type spin default 30 min 0 max 5000", 30)]
        [TestCase("option name Move Overhead type spin min 0 max 5000", null)]
        [TestCase("", null)]
        public void GetNumericDefault(string option, double? expected)
        {
            var actual = EngineHelpers.GetNumericDefault(option);
            Assert.AreEqual(expected, actual);
        }

        [TestCase("option name Analysis Contempt type combo default Both var Off var White var Black var Both", "Both")]
        [TestCase("option name Analysis Contempt type combo var Off var White var Black var Both", "")]
        public void GetStringDefault(string option, string expected)
        {
            var actual = EngineHelpers.GetStringDefault(option);
            Assert.AreEqual(expected, actual);
        }

        [TestCase("option name Ponder type check default false", false)]
        [TestCase("option name this type check default true", true)]
        [TestCase("option name is type check default FALSE", false)]
        [TestCase("option name so type check default TRUE", true)]
        [TestCase("option name the type check default falSe", false)]
        [TestCase("option name tests type check default tRuE", true)]
        [TestCase("option name work type check", false)]
        [TestCase("", false)]
        public void TestCheckboxDefault(string option, bool expected)
        {
            var actual = option.GetDefaultForCheckbox();
            Assert.AreEqual(expected, actual);
        }


        [TestCase("option name Contempt type spin default 24 min -100 max 100", "default", 24)]
        [TestCase("option name Move Overhead type spin default 30 min 0 max 5000", "default", 30)]
        [TestCase("option name Contempt type spin default 24 min -100 max 100", "min", -100)]
        [TestCase("option name Move Overhead type spin default 30 min 0 max 5000", "min", 0)]
        [TestCase("option name Contempt type spin default 24 min -100 max 100", "max", 100)]
        [TestCase("option name Move Overhead type spin default 30 min 0 max 5000", "max", 5000)]
        [TestCase("option name Move Overhead type spin default 30 min 0", "max", null)]
        [TestCase(null, "max", null)]
        public void GetNumericOptionType(string option, string key, double? expected)
        {
            var actualVal = EngineHelpers.GetNumericOptionType(option, key);
            Assert.AreEqual(expected, actualVal);
        }

        [Test]
        public void TestOptionsClass()
        {
            var engineInfo = new UCIEngineInformation(uciResponse);
            Assert.AreEqual("Stockfish 10 64", engineInfo.Name);
            Assert.AreEqual("T. Romstad, M. Costalba, J. Kiiski, G. Linscott", engineInfo.Author);
            Assert.AreEqual(19, engineInfo.Options.Length);
            Assert.AreEqual(true, engineInfo.UCIOk);
        }

        private const string uciResponse =
            @"
id name Stockfish 10 64
id author T. Romstad, M. Costalba, J. Kiiski, G. Linscott

option name Debug Log File type string default
option name Contempt type spin default 24 min -100 max 100
option name Analysis Contempt type combo default Both var Off var White var Black var Both
option name Threads type spin default 1 min 1 max 512
option name Hash type spin default 16 min 1 max 131072
option name Clear Hash type button
option name Ponder type check default false
option name MultiPV type spin default 1 min 1 max 500
option name Skill Level type spin default 20 min 0 max 20
option name Move Overhead type spin default 30 min 0 max 5000
option name Minimum Thinking Time type spin default 20 min 0 max 5000
option name Slow Mover type spin default 84 min 10 max 1000
option name nodestime type spin default 0 min 0 max 10000
option name UCI_Chess960 type check default false
option name UCI_AnalyseMode type check default false
option name SyzygyPath type string default <empty>
option name SyzygyProbeDepth type spin default 1 min 1 max 100
option name Syzygy50MoveRule type check default true
option name SyzygyProbeLimit type spin default 7 min 0 max 7
uciok
";

    }
}
