using ChessLib.Data.MoveRepresentation;
using EnumsNET;
using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using ChessLib.Data.Helpers;
using System.Collections;
using System.Threading;
using System.Collections.Concurrent;
using System.IO;

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
        private AutoResetEvent UCICommandIssued = new AutoResetEvent(false);
        private AutoResetEvent UCIShutdownIssued = new AutoResetEvent(false);
        private AutoResetEvent UCIResponseRecieved = new AutoResetEvent(false);

        [NonSerialized]
        private ConcurrentQueue<UCICommandInfo> _uciCommandQueue = new ConcurrentQueue<UCICommandInfo>();
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

        public void SendCommand(UCICommandInfo commandInfo, params string[] args)
        {
            if (!IsProcessRunning)
            {
                throw new NullReferenceException("Process must be started before sending command.");
            }
            commandInfo.SetCommandArguments(args);
            _uciCommandQueue.Enqueue(commandInfo);
        }

        public ReceiveOutput _recieveOutput;

        public void Stop()
        {
            if (IsProcessRunning)
            {

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
            _process.Start();
            _process.PriorityClass = _priority;
            _process.BeginErrorReadLine();
            _process.BeginOutputReadLine();
            _process.OutputDataReceived += OnUCIResponseRecieved;
            ThreadPool.QueueUserWorkItem(ExecuteEngine);

        }

        private void OnUCIResponseRecieved(object sender, DataReceivedEventArgs e)
        {
            string engineResponse = e.Data;
            if (string.IsNullOrEmpty(engineResponse))
            {
                return;
            }
            if (_uciCommandQueue.TryPeek(out UCICommandInfo command))
            {
                if (command.AwaitResponse)
                {
                    //TODO StartHere
                }
            }

        }

        private void ExecuteEngine(object state)
        {
            var exit = false;
            var remainingQueueItems = 0;
            int waitResult = WaitHandle.WaitTimeout;
            var commandEvents = new[] { UCICommandIssued, UCIShutdownIssued };
            var finishedEvents = new[] { UCIResponseRecieved, UCIShutdownIssued };
            const int ShutdownHandle = 1;
            StreamWriter engineInputStream = _process.StandardInput;
            this.SendUCI();
            while (!exit)
            {
                remainingQueueItems = _uciCommandQueue.Count();
                if (!_uciCommandQueue.Any())
                {
                    waitResult = WaitHandle.WaitAny(commandEvents);
                }
                if (waitResult == ShutdownHandle)
                {
                    exit = true;
                }
                else
                {
                    if (_uciCommandQueue.TryPeek(out UCICommandInfo commandToIssue))
                    {
                        engineInputStream.WriteLine(commandToIssue.GetFullCommand());
                        if (commandToIssue.AwaitResponse)
                        {
                            this.SendIsReady();
                        }
                    }
                    waitResult = WaitHandle.WaitAny(finishedEvents);
                    if (waitResult == ShutdownHandle)
                    {
                        exit = true;
                    }
                }
            }

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

        public static void SendIsReady(this Engine engine)
        {
            var commandInfo = new UCICommandInfo(UCICommandToEngine.IsReady);
            engine.SendCommand(commandInfo);
        }

        public static void SendUCI(this Engine engine)
        {
            var commandInfo = new UCICommandInfo(UCICommandToEngine.UCI);
            engine.SendCommand(commandInfo);
        }

        private static void SendStop(this Engine engine)
        {
            var commandInfo = new UCICommandInfo(UCICommandToEngine.Stop);
            engine.SendCommand(commandInfo);
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
            //StringBuilder sb = new StringBuilder("go");
            //sb.Append(GetMoves(searchMoves));
            //if (nodesToSearch.HasValue) sb.Append($" nodes {nodesToSearch.Value}");
            //var timeInMsToSearch = searchTime.TotalMilliseconds.ToString();
            //sb.Append($" movetime {timeInMsToSearch}");
            //eng.SendCommand(sb.ToString().Trim());

        }

        ///// <summary>
        ///// Starts a search for infinite amount of time. Must send "stop" command end search.
        ///// </summary>
        ///// <param name="eng"></param>
        ///// <param name="nodesToSearch">search x nodes only</param>
        ///// <param name="searchMoves">estrict search to these moves only</param>
        //public static void SendGo(this Engine eng, int? nodesToSearch, MoveExt[] searchMoves = null)
        //{
        //    StringBuilder sb = new StringBuilder("go");
        //    sb.Append(GetMoves(searchMoves));
        //    if (nodesToSearch.HasValue) sb.Append($" nodes {nodesToSearch.Value}");
        //    eng.SendCommand(sb.ToString().Trim());
        //}
    }
}
