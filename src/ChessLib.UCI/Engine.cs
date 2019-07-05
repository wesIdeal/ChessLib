using ChessLib.Data;
using ChessLib.Data.Helpers;
using ChessLib.Data.MoveRepresentation;
using ChessLib.UCI.Commands;
using ChessLib.UCI.Commands.ToEngine;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ChessLib.UCI.Commands.FromEngine;

namespace ChessLib.UCI
{
    [Serializable]
    public abstract class Engine : IDisposable
    {
        [NonSerialized]
        private readonly ProcessStartInfo _startInfo = new ProcessStartInfo
        {
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardError = true,
            RedirectStandardInput = true,
            RedirectStandardOutput = true
        };

        private ProcessPriorityClass _priority;

        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_isDisposed)
            {
                return;
            }

            if (disposing)
            {
                Process.Exited -= OnProcessExited;
                Process.ErrorDataReceived -= OnErrorDataReceived;
                Process.Dispose();
                EngineCommandQueue.Dispose();
            }
            _isDisposed = true;
        }

        public void SetPriority(ProcessPriorityClass priority)
        {
            _priority = priority;
            if (Process != null && !Process.HasExited) Process.SetPriority(_priority);
        }

        protected void QueueCommand(CommandInfo commandInfo, params string[] args)
        {
            OnDebugEventExecuted(new DebugEventArgs($"Adding {commandInfo.CommandText} to queue."));
            commandInfo.SetCommandArguments(args);
            EngineCommandQueue.Enqueue(commandInfo);
        }


        public Task StartAsync()
        {
            OnDebugEventExecuted(new DebugEventArgs("Starting engine task - ExecuteEngineAsync()"));
            return Task.Run(ExecuteEngineAsync);
        }



        protected virtual void ExecuteEngineAsync()
        {
            ProcessId = StartEngineProcess();
            SendStartupMessagesToEngine();
            ProcessEngineStartupOptions();
            BeginMessageQueue();
        }

        private int StartEngineProcess()
        {
            OnDebugEventExecuted(new DebugEventArgs("executing StartEngineProcess()"));

            Process.Start(CommandLineString, _startInfo);
            OnDebugEventExecuted(new DebugEventArgs("Process.Start() called."));
            Process.BeginErrorReadLine();
            Process.BeginOutputReadLine();
            Process.SetPriority(_priority);
            return Process.Id;
        }

        private void BeginMessageQueue()
        {
            var waitResult = WaitHandle.WaitTimeout;
            var exit = false;
            var interruptHandle = 1;
            OnDebugEventExecuted(new DebugEventArgs("Starting message queue."));
            while (!exit && !_isDisposed)
            {
                if (!EngineCommandQueue.Any())
                {
                    OnDebugEventExecuted(new DebugEventArgs("Nothing in queue. Waiting for command..."));
                    waitResult = WaitHandle.WaitAny(EngineCommandQueue.CommandIssuedEvents);
                }

                if (EngineCommandQueue.TryPeek(out var commandToIssue))
                {
                    EngineCommandQueue.TryDequeue(out _);
                    if (waitResult == interruptHandle || commandToIssue.CommandSent.IsInterruptCommand())
                    {
                        OnDebugEventExecuted(
                            new DebugEventArgs($"Received interrupt command in queue - {commandToIssue.CommandText}"));
                        WriteToEngine(commandToIssue);
                        exit = commandToIssue.CommandSent == AppToUCICommand.Quit;
                    }
                    else
                    {
                        OnDebugEventExecuted(
                            new DebugEventArgs($"Received command in queue - {commandToIssue.CommandText}"));
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
            if (!Process.WaitForExit(exitTimeout))
            {
                OnDebugEventExecuted(
                    new DebugEventArgs($"Engine process didn't shutdown in {exitTimeout}ms. Killing process."));
                Process.Kill();
                OnDebugEventExecuted(new DebugEventArgs("Engine process killed."));
            }
        }

        public abstract void SendStartupMessagesToEngine();


        public void WriteToEngine(CommandInfo command)
        {
            Process.SendCommandToEngine(command.ToString());
            OnEngineCommunication(
                new EngineCommunicationArgs(EngineCommunicationArgs.TextSource.UI, command.ToString()));
        }





        public void SendNewGame()
        {
            var commandInfo = new CommandInfo(AppToUCICommand.NewGame);
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
            var resultingFen = GetFENResult(fen, moves);

            var positionString = "startpos";
            if (fen != FENHelpers.FENInitial) positionString = $"fen {fen}";
            var moveString = "";
            if (moves.Any())
                moveString = " moves " + string.Join(" ", moves.Select(MoveDisplayService.MoveToLan));
            QueueCommand(commandInfo, $"{positionString}{moveString}");
        }

        private string GetFENResult(string fen, MoveExt[] moves)
        {
            var bi = new BoardInfo(fen);
            foreach (var mv in moves) bi.ApplyMove(mv);
            return bi.ToFEN();
        }

        public void SendQuit()
        {
            var commandInfo = new CommandInfo(AppToUCICommand.Quit);
            QueueCommand(commandInfo);
        }


        private void OnProcessExited(object sender, EventArgs e)
        {
            OnDebugEventExecuted(new DebugEventArgs($"Received process exited event for engine {ToString()}."));
            EngineCommandQueue.InterruptIssued.Set();
        }

        /// <summary>
        ///     Fired each time the app communicates with the engine or a response is received.
        ///     Contains raw text responses and commands.
        /// </summary>
        public event EventHandler<EngineCommunicationArgs> EngineCommunication;



        public event EventHandler<DebugEventArgs> DebugEventExecuted;

        public event EventHandler<StateChangeEventArgs> StateChanged;

        public event DataReceivedEventHandler EngineDataReceived;

        public class StateChangeEventArgs : EventArgs
        {
            public StateChangeEventArgs(StateChangeField stateChangeField, bool value)
            {
                StateChangeField = stateChangeField;
                Value = value;
            }

            public StateChangeField StateChangeField { get; set; }
            public bool Value { get; set; }
        }

        #region Constructors / Descructor

        protected Engine(Guid userEngineId)
        {
            UserAssignedId = userEngineId;
            Process = new EngineProcess(UserAssignedId);
            Process.EngineDataReceived += EngineDataReceived;
            Process.EngineCommunicationReceived += OnEngineCommunicationReceived;
            EngineCommandQueue = new CommandQueue();
        }

        protected Engine(Guid userEngineId, string description, string command) : this(userEngineId)
        {
            UserEngineDescription = description;
            CommandLineString = command;
            Priority = ProcessPriorityClass.Normal;
        }

        protected Engine(Guid userEngineId, string description, string command, EngineProcess process)
            : this(userEngineId, description, command)
        {
            Process = process;
        }

        protected Engine(Guid userEngineId, string description, string command, bool ignoreMoveCalculationLines)
            : this(userEngineId, description, command)
        {
            IgnoreMoveCalculationLines = ignoreMoveCalculationLines;
        }

        protected Engine(Guid userEngineId, string description, string command, bool ignoreMoveCalculationLines,
            ProcessPriorityClass priority)
            : this(userEngineId, description, command, ignoreMoveCalculationLines)
        {
            Priority = priority;
        }

        protected Engine(Guid userEngineId, string description, string command, bool ignoreMoveCalculationLines,
            ProcessPriorityClass priority, Dictionary<string, string> engineOptions)
            : this(userEngineId, description, command, ignoreMoveCalculationLines, priority)
        {
            EngineOptions = engineOptions;
        }

        protected abstract void ProcessEngineStartupOptions();


        #endregion


        #region Properties

        public readonly Guid UserAssignedId;

        /// <summary>
        ///     Initial options to set on uci engine
        /// </summary>
        public Dictionary<string, string> EngineOptions { get; }

        /// <summary>
        ///     Represents whether single move calculation lines should be sent to the client.
        /// </summary>
        public bool IgnoreMoveCalculationLines { get; set; }

        /// <summary>
        ///     Command line path + additional arguments
        /// </summary>
        public string CommandLineString { get; }

        /// <summary>
        ///     Engine description supplied by user
        /// </summary>
        public string UserEngineDescription { get; set; }

        public ProcessPriorityClass Priority
        {
            get => _priority;
            private set => _priority = value;
        }

        #endregion

        #region Non-Serialized Fields/Properties

        [NonSerialized] protected string FEN;
        [NonSerialized] private bool _isDisposed;
        [NonSerialized] public int ProcessId = -1;
        [NonSerialized] private readonly StringBuilder _errorBuilder = new StringBuilder();
        [NonSerialized] protected EngineProcess Process;
        [NonSerialized] private readonly TaskCompletionSource<bool> _errorCloseEvent = new TaskCompletionSource<bool>();
        [NonSerialized] protected CommandQueue EngineCommandQueue;
        public event EventHandler<EngineResponseArgs> EngineResponseObjectReceived;
        public event EventHandler<DebugEventArgs> MessageSentFromQueue;

        #endregion

        #region Event Raisers

        protected abstract void EngineResponseReceived(object sender, DataReceivedEventArgs e);

        protected void OnEngineCommunicationReceived(object sender, EngineCommunicationArgs args)
        {
            OnEngineCommunication(args);
        }

        protected void OnEngineCommunication(EngineCommunicationArgs engineCommunicationArgs)
        {
            Volatile.Read(ref EngineCommunication)?.Invoke(this, engineCommunicationArgs);
        }

        protected void OnDebugEventExecuted(DebugEventArgs args)
        {
            Volatile.Read(ref DebugEventExecuted)?.Invoke(this, args);
        }

        protected void OnEngineStateChanged(StateChangeEventArgs args)
        {
            Volatile.Read(ref StateChanged)?.Invoke(this, args);
        }

        protected void OnEngineObjectReceived(EngineResponseArgs response)
        {
            Volatile.Read(ref EngineResponseObjectReceived)?.Invoke(this, response);
        }

        protected void OnErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            var message = e.Data;
            if (string.IsNullOrEmpty(message))
            {
                message = "No message received.";
                _errorCloseEvent.SetResult(true);
            }
            else
            {
                _errorBuilder.AppendLine(message);
            }

            OnDebugEventExecuted(new DebugEventArgs(message));
            //Volatile.Read(ref ErrorReceivedFromEngineProcess)?.Invoke(sender, e);
        }

        #endregion
    }
}