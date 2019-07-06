using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ChessLib.EngineInterface
{
    public class EngineStartupArgs : IEngineStartupArgs
    {
        public EngineStartupArgs(Guid engineId, string description, string commandLine,
            ProcessPriorityClass priority = ProcessPriorityClass.Normal, Dictionary<string, string> initialEngineOptions = null)
        {
            EngineId = engineId;
            Description = description;
            CommandLine = commandLine;
            InitialPriority = priority;
            EngineOptions = initialEngineOptions ?? new Dictionary<string, string>();
        }

        /// <summary>
        /// User assigned id for the engine
        /// </summary>
        public Guid EngineId { get; set; }
        /// <summary>
        /// User assigned engine description
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Command to start the engine, along with any commandline options
        /// </summary>
        public string CommandLine { get; set; }

        /// <summary>
        /// Priority to apply upon startup
        /// </summary>
        public ProcessPriorityClass InitialPriority { get; set; }

        /// <summary>
        /// Options to apply upon startup
        /// </summary>
        public Dictionary<string, string> EngineOptions { get; set; }
    }
}