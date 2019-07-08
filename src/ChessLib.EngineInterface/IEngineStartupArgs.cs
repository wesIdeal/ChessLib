using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ChessLib.EngineInterface
{
    public interface IEngineStartupArgs
    {
        /// <summary>
        /// User assigned id for the engine
        /// </summary>
        Guid EngineId { get; set; }

        /// <summary>
        /// User assigned engine description
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// Command to start the engine, along with any commandline options
        /// </summary>
        string CommandLine { get; set; }

        /// <summary>
        /// Priority in which the process starts
        /// </summary>
        ProcessPriorityClass InitialPriority { get; set; }

        /// <summary>
        /// A dictionary of name/values of options to set on startup
        /// </summary>
        Dictionary<string, string> EngineOptions { get; set; }
    }
}