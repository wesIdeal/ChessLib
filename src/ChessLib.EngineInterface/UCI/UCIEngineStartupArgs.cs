using System;

namespace ChessLib.EngineInterface.UCI
{
    public class UCIEngineStartupArgs : EngineStartupArgs
    {
        public UCIEngineStartupArgs(Guid engineId, string description, string commandLine, bool ignoreMoveCalculationLines = true)
            : base(engineId, description, commandLine)
        {
            IgnoreMoveCalculationLines = ignoreMoveCalculationLines;
        }
        /// <summary>
        /// Used to ignore single move calculation lines from the engine
        /// </summary>
        public bool IgnoreMoveCalculationLines { get; set; }
    }
}