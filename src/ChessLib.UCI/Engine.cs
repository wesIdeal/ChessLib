using ChessLib.Data.MoveRepresentation;
using ChessLib.UCI.Commands;
using ChessLib.UCI.Commands.FromEngine;
using ChessLib.UCI.Commands.ToEngine;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChessLib.UCI
{
    [Serializable]
    public abstract class Engine : IDisposable
    {


        public IEngineStartupArgs UserSuppliedArgs { get; protected set; }

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
            Priority = priority;
            if (Process != null && !Process.HasExited) Process.SetPriority(Priority);
        }

        protected void QueueCommand(CommandInfo commandInfo, params string[] args)
        {
            commandInfo.SetCommandArguments(args);
            EngineCommandQueue.Enqueue(commandInfo);
        }

        protected void QueueCommand(SetOption commandInfo)
        {
            EngineCommandQueue.Enqueue(commandInfo);
        }


        public virtual Task StartAsync()
        {
            OnDebugEventExecuted(new DebugEventArgs("Starting engine task - ExecuteEngineAsync()"));
            return Task.Run(ExecuteEngineAsync);
        }



        protected virtual void ExecuteEngineAsync()
        {
            ProcessId = StartEngineProcess();
            BeginMessageQueue();

            ProcessEngineStartupOptions();

        }

        private int StartEngineProcess()
        {
            OnDebugEventExecuted(new DebugEventArgs("executing StartEngineProcess()"));

            Process.Start(UserSuppliedArgs.CommandLine, _startInfo);
            OnDebugEventExecuted(new DebugEventArgs("Process.Start() called."));
            Process.BeginErrorReadLine();
            Process.BeginOutputReadLine();
            Process.SetPriority(Priority);
            return Process.ProcessId;
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
                    waitResult = WaitHandle.WaitAny(EngineCommandQueue.CommandIssuedEvents);
                    if (waitResult == WaitHandle.WaitTimeout)
                    {
                        OnDebugEventExecuted(new DebugEventArgs("Nothing in queue after waiting."));
                    }
                }

                if (EngineCommandQueue.TryPeek(out var commandToIssue))
                {
                    EngineCommandQueue.TryDequeue(out _);
                    WriteToEngine(commandToIssue);
                    exit = commandToIssue.CommandSent == AppToUCICommand.Quit;

                }
            }

            BeginExitRoutine();
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
            var message = $"[To {UserAssignedId.ToString().Substring(0, 4)}...] {command}";
            OnDebugEventExecuted(new DebugEventArgs(message));
        }

        public void WriteToEngine(SetOption command)
        {
            Process.SendCommandToEngine(command.ToString());
            var message = $"[To {UserAssignedId.ToString().Substring(0, 4)}...] {command}";
            OnDebugEventExecuted(new DebugEventArgs(message));
        }


        /// <summary>
        /// Sends a new game position to engine
        /// </summary>
        public abstract void SendPosition();

        /// <summary>
        /// Sends a fen to the engine
        /// </summary>
        /// <param name="fen"></param>
        public abstract void SendPosition(string fen);

        /// <summary>
        /// Sends moves to the engine to be applied from the original FEN sent.
        /// </summary>
        /// <param name="moves"></param>
        public abstract void SendPosition(MoveExt[] moves);

        /// <summary>
        /// Sends a fen to the board and moves
        /// </summary>
        /// <param name="fen"></param>
        /// <param name="moves"></param>
        public abstract void SendPosition(string fen, MoveExt[] moves);

        /// <summary>
        /// Sends the quit command to the engine
        /// </summary>
        public abstract void SendQuit();

        /// <summary>
        /// Sets an engine option
        /// </summary>
        public abstract void SetOption(string optionName, string optionValue);

        private void OnProcessExited(object sender, EventArgs e)
        {
            OnDebugEventExecuted(new DebugEventArgs($"Received process exited event for engine {ToString()}."));
            EngineCommandQueue.InterruptIssued.Set();
        }



        #region Constructors / Descructor

        protected Engine()
        {
            Process = new EngineProcess(MessageSubscriber);
            Process.EngineCommunicationReceived += OnCommunicationFromEngine;
            EngineCommandQueue = new CommandQueue();
            _startInfo = new ProcessStartInfo
            {
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true
            };
        }

        private void OnCommunicationFromEngine(object sender, DebugEventArgs e)
        {
            var message = $"[Fr {UserAssignedId.ToString().Substring(0, 4)}...] {e.DebugText}";
            OnDebugEventExecuted(new DebugEventArgs(message));
        }

        protected Engine(EngineStartupArgs args, IEngineMessageSubscriber subscriber) : this()
        {
            MessageSubscriber = subscriber;
            UserSuppliedArgs = args;
            Priority = args.InitialPriority;
            EngineOptions = args.EngineOptions;
        }

        protected abstract void ProcessEngineStartupOptions();

        #endregion


        #region Properties

        public Guid UserAssignedId => UserSuppliedArgs.EngineId;

        /// <summary>
        ///     Initial options to set on uci engine
        /// </summary>
        public Dictionary<string, string> EngineOptions { get; }

        /// <summary>
        ///     Engine description supplied by user
        /// </summary>
        public string UserEngineDescription => UserSuppliedArgs.Description;

        public ProcessPriorityClass Priority { get; private set; }

        #endregion

        #region Non-Serialized Fields/Properties

        protected virtual IEngineMessageSubscriber MessageSubscriber { get; set; }
        [NonSerialized] protected string FEN;
        [NonSerialized] private bool _isDisposed;
        [NonSerialized] public int ProcessId = -1;
        protected EngineProcess Process { get; set; }
        [NonSerialized] private readonly TaskCompletionSource<bool> _errorCloseEvent = new TaskCompletionSource<bool>();
        [NonSerialized] protected CommandQueue EngineCommandQueue;
        [NonSerialized] private readonly ProcessStartInfo _startInfo;

        /// <summary>
        /// Invoked when a response was received from the engine
        /// </summary>
        public event EventHandler<EngineResponseArgs> EngineResponseObjectReceived;

        /// <summary>
        /// Invoked when general debug information is written
        /// </summary>
        public event EventHandler<DebugEventArgs> DebugEventExecuted;

        #endregion

        #region Event Raisers

        protected void OnDebugEventExecuted(DebugEventArgs args)
        {

            Volatile.Read(ref DebugEventExecuted)?.Invoke(this, args);
        }

        protected void OnEngineObjectReceived(EngineResponseArgs response)
        {
            response.Id = UserAssignedId;
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
            OnDebugEventExecuted(new DebugEventArgs(message) { EventLevel = EventLevel.Error });
        }

        #endregion
    }
}