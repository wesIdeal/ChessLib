using ChessLib.Data.MoveRepresentation;
using EnumsNET;
using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using ChessLib.Data.Helpers;
using System.Collections;

namespace ChessLib.UCI
{
    [Serializable]
    public class Engine
    {
        public Guid Id => _uid;
        private Guid _uid;
        protected bool IsReady = false;
        public string Description { get; set; }
        public string Command { get; private set; }
        public string[] UciArguments { get; private set; }
        public ProcessPriorityClass Priority
        {
            get => _priority;
            private set => _priority = value;
        }

        public void SetPriority(ProcessPriorityClass priority)
        {
            _priority = priority;
            if (_process != null)
            {
                _process.PriorityClass = priority;
            }
        }
        public ReceiveOutput _recieveOutput;

        public void Stop()
        {
            if (IsProcessRunning)
            {
                this.SendCommand(CommandToUCI.Stop);
            }
        }

        [NonSerialized] public Process _process;
        [NonSerialized]
        ProcessStartInfo startInfo = new ProcessStartInfo()
        {
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardError = true,
            RedirectStandardInput = true,
            RedirectStandardOutput = true
        };
        private ProcessPriorityClass _priority;

        public Engine(string description, string command, string[] uciArguments, ReceiveOutput recieveOutput, Guid? id = null, ProcessPriorityClass priority = ProcessPriorityClass.Normal)
        {
            _uid = id ?? Guid.NewGuid();
            Description = description;
            Command = command;
            UciArguments = uciArguments;
            Priority = priority;
            _recieveOutput = recieveOutput;

        }

        public void Start()
        {
            _process = new Process();
            _process.StartInfo = startInfo;
            _process.StartInfo.FileName = Command;
            _process.OutputDataReceived += OnReceiveOutput;
            _process.Start();
            _process.PriorityClass = _priority;
            _process.BeginErrorReadLine();
            _process.BeginOutputReadLine();
            this.SendCommand(CommandToUCI.IsReady, UCIResponse.Ready);
        }


        public void OnReceiveOutput(object sender, DataReceivedEventArgs e)
        {
            var receivedOutput = e.Data;
            if (receivedOutput == "readyok")
            {
                IsReady = true;
                return;
            }
            _recieveOutput?.Invoke(Id, Description, receivedOutput);
        }

        public bool IsProcessRunning => _process != null && !_process.HasExited;


    }

    public static class EngineHelpers
    {
        private static string GetMoves(MoveExt[] moves)
        {
            if (moves != null && moves.Any())
            {
                StringBuilder sb = new StringBuilder("searchmoves");
                foreach (var move in moves)
                {
                    sb.Append($" {move.SourceIndex.IndexToSquareDisplay()}{move.DestinationIndex.IndexToSquareDisplay()}");
                }

                return sb.ToString().Trim();
            }

            return "";
        }


        public static Engine SendCommand(this Engine eng, CommandToUCI command, UCIResponse response, params string[] args)
        {
            var expectedArgCount = UCICommand.GetExpectedArgCount(command);
            var commandStr = UCICommand.GetCommandString(command);
            commandStr += string.Join(" ", args);
            eng.SendCommand(commandStr);
            return eng;
        }


        public static Engine SendCommand(this Engine eng, CommandToUCI command, params string[] args)
        {
            var expectedArgCount = UCICommand.GetExpectedArgCount(command);
            var commandStr = UCICommand.GetCommandString(command);
            commandStr += string.Join(" ", args);
            eng.SendCommand(commandStr);
            return eng;
        }

        public static Engine SendCommand(this Engine eng, string command)
        {
            if (!eng.IsProcessRunning)
            {
                throw new NullReferenceException("Process must be started before sending command.");
            }
            eng._process.StandardInput.Write(command.Trim('\n', '\r').Append('\n'));
            eng._process.StandardInput.Flush();
            return eng;
        }


        /// <summary>
        /// Starts a search for set depth
        /// </summary>
        /// <param name="eng"></param>
        /// <param name="nodesToSearch">search x nodes only</param>
        ///  /// <param name="depth">search x plies only</param>
        /// <param name="searchMoves">only consider these moves</param>
        public static void SendGo(this Engine eng, int? nodesToSearch, int depth,
            MoveExt[] searchMoves = null)
        {
            StringBuilder sb = new StringBuilder("go");
            sb.Append(GetMoves(searchMoves));
            if (nodesToSearch.HasValue) sb.Append($" nodes {nodesToSearch.Value}");
            sb.Append($" depth {depth}");
        }

        /// <summary>
        /// Starts a search for set amount of time
        /// </summary>
        /// <param name="eng"></param>
        /// <param name="searchDepth">search x plies only</param>
        /// <param name="nodesToSearch">search x nodes only</param>
        /// <param name="searchTime">Time to spend searching</param>
        /// <param name="searchMoves">only consider these moves</param>
        public static void SendGo(this Engine eng, int? nodesToSearch, TimeSpan searchTime,
            MoveExt[] searchMoves = null)
        {
            StringBuilder sb = new StringBuilder("go");
            sb.Append(GetMoves(searchMoves));
            if (nodesToSearch.HasValue) sb.Append($" nodes {nodesToSearch.Value}");
            var timeInMsToSearch = searchTime.TotalMilliseconds.ToString();
            sb.Append($" movetime {timeInMsToSearch}");
            eng.SendCommand(sb.ToString().Trim());

        }

        /// <summary>
        /// Starts a search for infinite amount of time. Must send "stop" command end search.
        /// </summary>
        /// <param name="eng"></param>
        /// <param name="nodesToSearch">search x nodes only</param>
        /// <param name="searchMoves">estrict search to these moves only</param>
        public static void SendGo(this Engine eng, int? nodesToSearch, MoveExt[] searchMoves = null)
        {
            StringBuilder sb = new StringBuilder("go");
            sb.Append(GetMoves(searchMoves));
            if (nodesToSearch.HasValue) sb.Append($" nodes {nodesToSearch.Value}");
            eng.SendCommand(sb.ToString().Trim());
        }
    }
}
