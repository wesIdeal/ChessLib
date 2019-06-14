using System;
using System.Diagnostics;
using System.Linq;
using System.Collections;
using System.Threading;
using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;
using System.Text;
using EnumsNET;
using ChessLib.Data.Helpers;
using ChessLib.Data.MoveRepresentation;
using System.Collections.Generic;
using ChessLib.UCI.Commands;
using ChessLib.UCI.Commands.ToEngine;
using ChessLib.UCI.Commands.FromEngine;

namespace ChessLib.UCI
{


    [Serializable]
    public partial class Engine : IDisposable
    {
        #region Constructors / Descructor
        protected Engine()
        {
            _process = new Process();
            _uciCommandQueue = new CommandQueue();
        }

        public Engine(string description, string command) : this()
        {
            UserEngineDescription = description;
            CommandLineString = command;
            Priority = ProcessPriorityClass.Normal;
        }
        public Engine(string description, string command, bool ignoreMoveCalculationLines)
            : this(description, command)
        {
            IgnoreMoveCalculationLines = ignoreMoveCalculationLines;
        }
        public Engine(string description, string command, bool ignoreMoveCalculationLines, ProcessPriorityClass priorityClass)
            : this(description, command, ignoreMoveCalculationLines)
        {
            Priority = priorityClass;
        }
        public Engine(string description, string command, bool ignoreMoveCalculationLines = true, ProcessPriorityClass priority = ProcessPriorityClass.Normal, Guid? userEngineId = null)
            : this(description, command, ignoreMoveCalculationLines, priority)
        {
            _userAssignedId = userEngineId ?? Guid.NewGuid();
        }
        public Engine(string description, string command, Dictionary<string, string> uciOptions, bool ignoreMoveCalculationLines = true, ProcessPriorityClass priority = ProcessPriorityClass.Normal, Guid? userEngineId = null)
            : this(description, command, ignoreMoveCalculationLines, priority, userEngineId)
        {

            UciOptions = uciOptions;
        }

        private void ProcessUCIOptions()
        {
            if (UciOptions == null) return;
            foreach (var option in UciOptions)
            {
                var message = "";
                if (!EngineInformation.SupportsOption(option.Key))
                {
                    throw new UCICommandException($"Option passed from command line {option.Key} is not valid.");
                }
                if (EngineInformation.IsOptionValid(option.Key, option.Value, out message))
                {
                    throw new UCICommandException(message);
                }
                WriteToEngine(new SetOption(option.Key, option.Value));
            }

        }
        #endregion


        #region Properties
        public Guid UserAssignedId => _userAssignedId;
        protected Guid _userAssignedId;

        /// <summary>
        /// Initial options to set on uci engine
        /// </summary>
        public Dictionary<string, string> UciOptions { get; private set; }

        /// <summary>
        /// Represents whether single move calculation lines should be sent to the client.
        /// </summary>
        public bool IgnoreMoveCalculationLines { get; set; }
        /// <summary>
        /// Command line path + additional arguemtns
        /// </summary>
        public string CommandLineString { get; private set; }

        /// <summary>
        /// Engine description supplied by user
        /// </summary>
        public string UserEngineDescription { get; set; }

        public ProcessPriorityClass Priority
        {
            get => _priority;
            private set => _priority = value;
        }
        #endregion

        private ProcessPriorityClass _priority;

        #region Non-Serialized Fields/Properties
        /// <summary>
        /// Gets the isready value from engine. This should only be false while processing the isready command and waiting for readyok.
        /// </summary>
        public bool IsReady
        {
            get { return _isReady; }
            protected set
            {
                _isReady = value;
                if (value)
                {
                    OnEngineStateChanged(new StateChangeEventArgs(StateChangeField.IsReady, value));
                }
            }
        }

        /// <summary>
        /// Represents whether UCI information was sent back to the engine. 
        /// </summary>
        public bool UCIOk
        {
            get { return _uciOk; }
            protected set
            {
                _uciOk = value;
                if (value)
                {
                    OnEngineStateChanged(new StateChangeEventArgs(StateChangeField.UCIOk, value));
                }
            }
        }

        /// <summary>
        /// Represents whether engine is currently analyzing a position
        /// </summary>
        public bool IsAnalyizing
        {
            get => _isAnalyizing;
            private set
            {
                _isAnalyizing = value;
                OnEngineStateChanged(new StateChangeEventArgs(StateChangeField.IsCalculating, value));
            }
        }

        /// <summary>
        /// Name and Id from uci command
        /// </summary>
        public string UCIName => $"{EngineInformation.Name} {EngineInformation.Id}";
        [NonSerialized] private bool _isDisposed = false;
        [NonSerialized] private StringBuilder _errorBuilder = new StringBuilder();
        [NonSerialized] private Process _process;
        [NonSerialized] private TaskCompletionSource<bool> errorCloseEvent = new TaskCompletionSource<bool>();

        [NonSerialized] private string _fen;
        [NonSerialized] private bool _uciOk = false;
        [NonSerialized] private bool _isReady = false;
        [NonSerialized] private bool _isAnalyizing = true;

        [NonSerialized] private StringBuilder _sbUciResponse = new StringBuilder();
        [NonSerialized] public UCIEngineInformation EngineInformation = new UCIEngineInformation();
        [NonSerialized] private CommandQueue _uciCommandQueue;
        [NonSerialized] private CommandInfo _currentCommand;
        [NonSerialized] private readonly AutoResetEvent ReadyOkReceived = new AutoResetEvent(false);
        [NonSerialized] private readonly AutoResetEvent UCIInfoReceived = new AutoResetEvent(false);

        #endregion

        ~Engine()
        {
            Dispose();
        }

        public void SetPriority(ProcessPriorityClass priority)
        {
            _priority = priority;
            if (_process != null && !_process.HasExited)
            {
                _process.PriorityClass = priority;
            }
        }

        private void QueueCommand(CommandInfo commandInfo, params string[] args)
        {
            OnDebugEventExecuted(new DebugEventArgs($"Adding {commandInfo.CommandText} to queue."));
            commandInfo.SetCommandArguments(args);
            _uciCommandQueue.Enqueue(commandInfo);
        }




        [NonSerialized]
        ProcessStartInfo startInfo = new ProcessStartInfo()
        {
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardError = true,
            RedirectStandardInput = true,
            RedirectStandardOutput = true
        };



        public Task StartAsync()
        {
            EngineInformation = new UCIEngineInformation();
            OnDebugEventExecuted(new DebugEventArgs("Starting engine task - ExecuteEngineAsync()"));
            return Task.Run(() => { ExecuteEngineAsync(); });
        }

        private void ProcessBestMoveResponse(string engineResponse, out IEngineResponse bestMoveResponse)
        {
            var moves = MakeBestMoveArrayFromUCI(engineResponse, out string variationDisplay);
            var bestMove = moves != null && moves.Count() > 0 ? moves[0] : null;
            var ponderMove = moves != null && moves.Count() > 1 ? moves[1] : null;
            IsAnalyizing = false;
            bestMoveResponse = new BestMoveResponse(bestMove, ponderMove);
        }

        private void ProcessInfoResponse(string engineResponse, out IEngineResponse response)
        {
            response = null;
            if (IgnoreMoveCalculationLines && InfoResponse.GetTypeOfInfo(engineResponse) == InfoResponse.InfoTypes.CalculationInfo)
            {
                return;
            }
            response = new InfoResponse(_fen, engineResponse);
        }

        private void ProcessUCIResponse(string engineResponse, EngineToAppCommand commandResponse, out IEngineResponse responseObject)
        {
            _sbUciResponse.AppendLine(engineResponse);
            if (commandResponse == EngineToAppCommand.UCIOk)
            {
                var engineInformation = new UCIEngineInformation(UserAssignedId, _sbUciResponse.ToString());
                _sbUciResponse.Clear();
                responseObject = engineInformation;
                UCIInfoReceived.Set();
            }
            else
            {
                responseObject = null;
            }
        }

        private MoveExt[] MakeBestMoveArrayFromUCI(string engineResponse, out string sanMoveDisplay)
        {
            var strBestMove = engineResponse.GetValueForUCIKeyValuePair(EngineToAppCommand.BestMove.AsString(EnumFormat.Description));
            var strPonder = engineResponse.GetValueForUCIKeyValuePair(EngineToAppCommand.Ponder.AsString(EnumFormat.Description));
            var rv = InfoResponse.FillMoves(_fen, new[] { strBestMove, strPonder }, out List<string> moveDisplay).ToArray();
            sanMoveDisplay = string.Join(" ", moveDisplay);
            return rv;
        }

        private void ExecuteEngineAsync()
        {
            StartEngineProcess();
            SendStartupMessagesToEngine();
            OnDebugEventExecuted(new DebugEventArgs($"Engine Id: {UserAssignedId.ToString()} PID:{_process.Id}\r\nStarted as {EngineInformation.Name} {EngineInformation.Id}"));
            ProcessUCIOptions();
            StartMessageQueue();
        }

        private int StartEngineProcess()
        {
            OnDebugEventExecuted(new DebugEventArgs("executing StartEngineProcess()"));
            _process.StartInfo = startInfo;
            _process.StartInfo.FileName = CommandLineString;
            _process.EnableRaisingEvents = true;
            _process.OutputDataReceived += OnUCIResponseRecieved;
            _process.Exited += OnProcessExited;
            _process.ErrorDataReceived += OnErrorDataReceived;
            _process.Start();
            OnDebugEventExecuted(new DebugEventArgs("Process.Start() called."));
            _process.BeginErrorReadLine();
            _process.BeginOutputReadLine();
            _process.PriorityClass = _priority;
            return _process.Id;
        }

        private void StartMessageQueue()
        {
            var waitResult = WaitHandle.WaitTimeout;
            var exit = false;
            var interruptHandle = 1;
            OnDebugEventExecuted(new DebugEventArgs("Starting message queue - StartMessageQueue()"));
            while (!_process.HasExited && !exit && !_isDisposed)
            {
                if (!_uciCommandQueue.Any())
                {
                    OnDebugEventExecuted(new DebugEventArgs("Nothing in queue. Waiting for command..."));
                    waitResult = WaitHandle.WaitAny(CommandQueue.CommandIssuedEvents);
                }

                if (_uciCommandQueue.TryPeek(out CommandInfo commandToIssue))
                {
                    _uciCommandQueue.TryDequeue(out _);
                    _currentCommand = commandToIssue;
                    if (_currentCommand.CommandSent == AppToUCICommand.IsReady)
                    {
                        IsReady = false;
                    }
                    else if (_currentCommand.CommandSent == AppToUCICommand.UCI)
                    {
                        UCIOk = false;
                    }
                    if (waitResult == interruptHandle || EngineHelpers.IsInterruptCommand(commandToIssue.CommandSent))
                    {
                        OnDebugEventExecuted(new DebugEventArgs($"Received interrupt command in queue - {commandToIssue.CommandText}"));
                        WriteToEngine(commandToIssue);
                        exit = commandToIssue.CommandSent == AppToUCICommand.Quit;

                    }
                    else
                    {
                        OnDebugEventExecuted(new DebugEventArgs($"Received command in queue - {commandToIssue.CommandText}"));
                        WriteToEngine(commandToIssue);
                    }
                }
            }
            BeginExitRoutine();
        }

        private void BeginExitRoutine()
        {
            OnDebugEventExecuted(new DebugEventArgs("Waiting for process to exit."));
            var exitTimeout = (int)TimeSpan.FromSeconds(5).TotalMilliseconds;
            if (!_process.WaitForExit(exitTimeout))
            {
                OnDebugEventExecuted(new DebugEventArgs($"Engine process didn't shutdown in {exitTimeout}ms. Killing process."));
                _process.Kill();
                OnDebugEventExecuted(new DebugEventArgs($"Engine process killed."));
            }
        }

        private void SendStartupMessagesToEngine()
        {
            WriteToEngine(new CommandInfo(AppToUCICommand.UCI));
            WaitHandle.WaitAll(new[] { UCIInfoReceived }, 5 * 1000);
            WriteToEngine(new CommandInfo(AppToUCICommand.IsReady));
            var handle = WaitHandle.WaitAll(new[] { ReadyOkReceived }, (int)10 * 1000);
        }

        private void WriteToEngine(CommandInfo command)
        {
            _process.StandardInput.WriteLine(command.ToString());
            _process.StandardInput.Flush();
            OnEngineCommunication(new EngineCommunicationArgs(EngineCommunicationArgs.TextSource.UI, command.ToString()));
        }

        public void SendIsReady()
        {
            var commandInfo = new CommandInfo(AppToUCICommand.IsReady);
            QueueCommand(commandInfo);
        }

        public void SendUCI()
        {
            var commandInfo = new CommandInfo(AppToUCICommand.UCI);
            QueueCommand(commandInfo);
        }

        public void SendStop()
        {
            var commandInfo = new CommandInfo(AppToUCICommand.Stop);
            QueueCommand(commandInfo);
        }

        public void SendPosition(string fen)
        {
            var commandInfo = new CommandInfo(AppToUCICommand.Position);
            QueueCommand(commandInfo, "fen", fen);
        }

        public void SendQuit()
        {
            var commandInfo = new CommandInfo(AppToUCICommand.Quit);
            QueueCommand(commandInfo);
        }

        /// <summary>
        /// Starts a search for set amount of time
        /// </summary>
        /// <param name="eng"></param>
        /// <param name="searchTime">Time to spend searching</param>
        /// <param name="searchMoves">only consider these moves</param>
        public void SendGo(TimeSpan searchTime, MoveExt[] searchMoves = null)
        {
            QueueCommand(new Go(searchTime, searchMoves));
        }

        public void SetNumberOfLinesToCalculate(double numberOfLines)
        {
            SetOption("MultiPV", numberOfLines.ToString());
        }

        public void SetOption(string optionName, string value)
        {
            var option = new SetOption(optionName, value);
            QueueCommand(option);
        }



        private void OnProcessExited(object sender, EventArgs e)
        {
            OnDebugEventExecuted(new DebugEventArgs($"Received process exited event."));
            CommandQueue.InterruptIssued.Set();
        }

        public void Dispose()
        {
            _process.Exited -= OnProcessExited;
            _process.ErrorDataReceived -= OnErrorDataReceived;
            _process.OutputDataReceived -= OnUCIResponseRecieved;
            _process.Dispose();
            _uciCommandQueue.Dispose();
            ReadyOkReceived.Dispose();
            UCIInfoReceived.Dispose();
            _isDisposed = true;
        }


    }
    public struct EngineProcessResult
    {
        public bool Completed;
        public int? ExitCode;
        public string Output;
    }
}
