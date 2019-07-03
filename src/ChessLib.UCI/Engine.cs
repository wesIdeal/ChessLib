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
using ChessLib.Data;

namespace ChessLib.UCI
{

    [Serializable]
    public partial class Engine : IDisposable
    {
        #region Constructors / Descructor

        protected Engine(Guid userEngineId)
        {
            UserAssignedId = userEngineId;
            EngineInformation = new OptionsResponseArgs(UserAssignedId);
            _process = new UCIEngineProc(UserAssignedId);
            _process.EngineCommunicationReceived += OnEngineCommunicationReceived;
            _uciCommandQueue = new CommandQueue();
        }

        public Engine(Guid userEngineId, string description, string command) : this(userEngineId)
        {
            UserEngineDescription = description;
            CommandLineString = command;
            Priority = ProcessPriorityClass.Normal;
        }

        public Engine(Guid userEngineId, string description, string command, UCIEngineProc process)
            : this(userEngineId, description, command)
        {
            _process = process;
        }

        public Engine(Guid userEngineId, string description, string command, bool ignoreMoveCalculationLines)
            : this(userEngineId, description, command)
        {
            IgnoreMoveCalculationLines = ignoreMoveCalculationLines;
        }
        public Engine(Guid userEngineId, string description, string command, bool ignoreMoveCalculationLines, ProcessPriorityClass priority)
            : this(userEngineId, description, command, ignoreMoveCalculationLines)
        {
            Priority = priority;
        }

        public Engine(Guid userEngineId, string description, string command, bool ignoreMoveCalculationLines, ProcessPriorityClass priority, Dictionary<string, string> uciOptions)
            : this(userEngineId, description, command, ignoreMoveCalculationLines, priority)
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
        public readonly Guid UserAssignedId;

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
        [NonSerialized] protected UCIEngineProc _process;
        [NonSerialized] private TaskCompletionSource<bool> errorCloseEvent = new TaskCompletionSource<bool>();

        [NonSerialized] private string _fen;
        [NonSerialized] private bool _uciOk = false;
        [NonSerialized] private bool _isReady = false;
        [NonSerialized] private bool _isAnalyizing = true;

        [NonSerialized] private StringBuilder _sbUciResponse = new StringBuilder();
        [NonSerialized] public OptionsResponseArgs EngineInformation;
        [NonSerialized] protected CommandQueue _uciCommandQueue;
        [NonSerialized] private CommandInfo _currentCommand;
        [NonSerialized] private readonly AutoResetEvent ReadyOkReceived = new AutoResetEvent(false);
        [NonSerialized] private readonly AutoResetEvent UCIInfoReceived = new AutoResetEvent(false);
        public event EventHandler<DebugEventArgs> MessageSentFromQueue;

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
                _process.SetPriority(_priority);
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
            OnDebugEventExecuted(new DebugEventArgs("Starting engine task - ExecuteEngineAsync()"));
            return Task.Run(() => { ExecuteEngineAsync(); });
        }





        private void ProcessUCIResponse(string engineResponse, EngineToAppCommand commandResponse, out IEngineResponse responseObject)
        {
            _sbUciResponse.AppendLine(engineResponse);
            if (commandResponse == EngineToAppCommand.UCIOk)
            {
                var engineInformation = new OptionsResponseArgs(UserAssignedId, _sbUciResponse.ToString());
                _sbUciResponse.Clear();
                responseObject = engineInformation;
                UCIInfoReceived.Set();
            }
            else
            {
                responseObject = null;
            }
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

            _process.Start(CommandLineString, startInfo);
            OnDebugEventExecuted(new DebugEventArgs("Process.Start() called."));
            _process.BeginErrorReadLine();
            _process.BeginOutputReadLine();
            _process.SetPriority(_priority);
            return _process.Id;
        }

        private void StartMessageQueue()
        {
            var waitResult = WaitHandle.WaitTimeout;
            var exit = false;
            var interruptHandle = 1;
            OnDebugEventExecuted(new DebugEventArgs("Starting message queue."));
            while (!exit && !_isDisposed)
            {
                if (!_uciCommandQueue.Any())
                {
                    OnDebugEventExecuted(new DebugEventArgs("Nothing in queue. Waiting for command..."));
                    waitResult = WaitHandle.WaitAny(_uciCommandQueue.CommandIssuedEvents);
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
                    OnQueueMessageSent(commandToIssue);
                }

            }
            BeginExitRoutine();
        }

        private void OnQueueMessageSent(CommandInfo commandToIssue)
        {
            Volatile.Read(ref MessageSentFromQueue)?.Invoke(this, new DebugEventArgs(commandToIssue.CommandText));
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

        public void SendStartupMessagesToEngine()
        {
            WriteToEngine(new CommandInfo(AppToUCICommand.UCI));
            if (UCIInfoReceived.WaitOne(5 * 1000))
            {
                WriteToEngine(new CommandInfo(AppToUCICommand.IsReady));
                ReadyOkReceived.WaitOne(5 * 1000);
            }
        }

        public void WriteToEngine(CommandInfo command)
        {
            _process.SendCommmand(command.ToString());
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

        public void SendNewGame()
        {
            var commandInfo = new CommandInfo(AppToUCICommand.NewGame);
            _process.SetPosition(FENHelpers.FENInitial);
            QueueCommand(commandInfo);
        }

        public void SendPosition(string fen)
        {
            SendPosition(fen, new MoveExt[] { });
        }

        public void SendPosition(MoveExt[] moves)
        {
            SendPosition(FENHelpers.FENInitial, moves);
        }

        public void SendPosition(string fen, MoveExt[] moves)
        {
            var commandInfo = new CommandInfo(AppToUCICommand.Position);
            string resultingFen = GetFENResult(fen, moves);
            
            var positionString = "startpos";
            if (fen != FENHelpers.FENInitial)
            {
                positionString = $"fen {fen}";
            }
            var moveString = "";
            if (moves.Any())
            {
                moveString = " moves " + string.Join(" ", moves.Select(x => MoveDisplayService.MoveToLan(x)));
            }
            QueueCommand(commandInfo, $"{positionString}{moveString}");
            _process.SetPosition(resultingFen);
        }

        private string GetFENResult(string fen, MoveExt[] moves)
        {
            BoardInfo bi = new BoardInfo(fen);
            foreach (var mv in moves)
            {
                bi.ApplyMove(mv);
            }
            return bi.ToFEN();
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
            _uciCommandQueue.InterruptIssued.Set();
        }

        public void Dispose()
        {
            _process.Exited -= OnProcessExited;
            _process.ErrorDataReceived -= OnErrorDataReceived;
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
