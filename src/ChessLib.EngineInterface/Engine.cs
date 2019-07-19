﻿using ChessLib.Data;
using ChessLib.Data.Helpers;
using ChessLib.Data.MoveRepresentation;
using ChessLib.EngineInterface.UCI.Commands;
using ChessLib.EngineInterface.UCI.Commands.FromEngine;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ChessLib.EngineInterface.UCI;

namespace ChessLib.EngineInterface
{
    [Serializable]
    public abstract class Engine : IDisposable
    {

        public IEngineStartupArgs UserSuppliedArgs { get; protected set; }
        public abstract InterruptCommand QuitCommand { get; }
        public abstract InterruptCommand StopCommand { get; }
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
                Process.EngineProcessExited -= OnProcessExited;
                Process.ProcessErrorDataReceived -= OnErrorDataReceived;
                Process.Dispose();
            }
            _isDisposed = true;
        }

        public void SetPriority(ProcessPriorityClass priority)
        {
            if (Process != null && Process.State == EngineProcess.UciState.Running) Process.SetPriority(priority);
        }

        public void QueueCommand(CommandInfo commandInfo, params string[] args)
        {
            commandInfo.SetCommandArguments(args);
            QueueCommand(commandInfo);
        }

        public void QueueCommand(CommandInfo commandInfo)
        {
            if (!(commandInfo is AwaitableCommandInfo || commandInfo is InterruptCommand))
            {
                Debug.Assert(!(new[] { "isready", "uci", "stop", "quit" }.Contains(commandInfo.CommandText)));
                SendIsReady();
            }
            Process.Send(commandInfo);
        }


        public virtual Task StartAsync()
        {
            if (Process == null)
            {
                throw new ArgumentNullException(nameof(Process), "Process must be initialized before starting engine.");
            }
            OnDebugEventExecuted(new DebugEventArgs("Starting engine task - ExecuteEngineAsync()"));
            return Task.Run(ExecuteEngineAsync);
        }



        protected virtual void ExecuteEngineAsync()
        {
            ProcessId = StartEngineProcess();
            ProcessEngineStartupOptions();
        }

        private int StartEngineProcess()
        {
            Process.Start(UserSuppliedArgs.CommandLine, UserSuppliedArgs.InitialPriority);
            return Process.Id;
        }



        public virtual void SendIsReady()
        {
            using (var resetEvent = SendIsReadyAsync())
            {
                if (!resetEvent.WaitOne())
                {
                    throw new TimeoutException("Timeout waiting for event after sending ready command.");
                }
            }
        }

        public abstract ManualResetEvent SendIsReadyAsync();


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


        /// <summary>
        /// Sends a new game position to engine
        /// </summary>
        public virtual void SendPosition()
        {
            SendPosition(FENHelpers.FENInitial);
        }

        /// <summary>
        /// Sends a fen to the engine
        /// </summary>
        /// <param name="fen"></param>
        public virtual void SendPosition(string fen)
        {
            StartingPositionFEN = CurrentFEN = fen;
        }

        /// <summary>
        /// Sends moves to the engine to be applied from the original StartingPositionFEN sent.
        /// </summary>
        /// <param name="moves"></param>
        public virtual void SendPosition(MoveExt[] moves)
        {
            SetMoves(moves);
        }

        private void SetMoves(IEnumerable<MoveExt> moves)
        {
            Moves.Clear();
            Moves.AddRange(moves);
            var board = CurrentBoard;
            CurrentFEN = board.ToFEN();
        }

        public List<MoveExt> Moves { get; } = new List<MoveExt>();

        /// <summary>
        /// Sends a fen to the board and moves
        /// </summary>
        /// <param name="fen"></param>
        /// <param name="moves"></param>
        public virtual void SendPosition(string fen, MoveExt[] moves)
        {
            StartingPositionFEN = fen;
            SetMoves(moves);
        }

        /// <summary>
        /// Sends a 'stop' command to the engine to halt calculation/other operations
        /// </summary>
        public void SendStop()
        {
            Process.Send(StopCommand);
        }

        /// <summary>
        /// Sends the quit command to the engine
        /// </summary>
        public void SendQuit()
        {
            Process.Send(QuitCommand);
        }


        private void OnProcessExited(object sender, EventArgs e)
        {
            OnDebugEventExecuted(new DebugEventArgs($"Received process exited event for engine {ToString()}."));
        }



        #region Constructors / Descructor

        private Engine()
        {


        }

        protected void InitializeEngineProcess(IEngineMessageSubscriber messageSubscriber)
        {
            Process = new EngineProcess(messageSubscriber);
            InitializeEngineProcess();
        }

        private void InitializeEngineProcess()
        {
            Process.EngineCommunicationReceived += OnCommunicationFromEngine;
            Process.EngineProcessExited += OnProcessExited;
            Process.DebugEventExecuted += OnDebugEventExecuted;
        }

        protected virtual void OnCommunicationFromEngine(object sender, DebugEventArgs e)
        {
            var message = $"[Fr {Process.Id}] {e.DebugText}";
            OnDebugEventExecuted(new DebugEventArgs(message));
        }

        protected Engine(IEngineStartupArgs args)
        {
            UserSuppliedArgs = args;
        }

        protected Engine(IEngineStartupArgs args, EngineProcess process) : this(args)
        {
            Process = process;
            InitializeEngineProcess();
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


        #endregion

        #region Non-Serialized Fields/Properties
        protected BoardInfo StartingBoard => new BoardInfo(StartingPositionFEN);
        protected BoardInfo CurrentBoard
        {
            get
            {
                var board = StartingBoard;
                Moves.ForEach(mv => { board.ApplyMove(mv); });
                return (BoardInfo)board.Clone();
            }
        }

        [NonSerialized] protected string CurrentFEN;
        [NonSerialized] protected string StartingPositionFEN;
        [NonSerialized] private bool _isDisposed;
        [NonSerialized] public int ProcessId = -1;
        protected IEngineProcess Process { get; set; }
       
        /// <summary>
        /// Invoked when a response object was received from the underlying engine process
        /// </summary>
        public event EventHandler<EngineResponseArgs> EngineResponseObjectReceived;

        /// <summary>
        /// Invoked when general debug information is sent from any Engine related process
        /// </summary>
        public event EventHandler<DebugEventArgs> DebugEventExecuted;

        /// <summary>
        /// Only sends the subset of engine objects involving variation calculation / best moves
        /// </summary>
        public event EventHandler<EngineCalculationResponseArgs> EngineCalculationReceived;

        #endregion

        #region Event Raisers
        protected void OnDebugEventExecuted(object sender, DebugEventArgs args)
        {
            Debug.WriteLine(args.DebugText);
            Volatile.Read(ref DebugEventExecuted)?.Invoke(this, args);
        }

        protected void OnDebugEventExecuted(DebugEventArgs args)
        {
            OnDebugEventExecuted(this, args);
        }

        protected void OnEngineObjectReceived(EngineResponseArgs response)
        {
            response.Id = UserAssignedId;
            if (response is EngineCalculationResponseArgs engineCalculationResponseArgs)
            {
                Volatile.Read(ref EngineCalculationReceived)?.Invoke(this, engineCalculationResponseArgs);
            }
            Volatile.Read(ref EngineResponseObjectReceived)?.Invoke(this, response);
        }



        protected void OnErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            var message = e.Data;
            if (string.IsNullOrEmpty(message))
            {
                message = "No message received.";
            }
            OnDebugEventExecuted(new DebugEventArgs(message) { EventLevel = EventLevel.Error });
        }

        #endregion
    }
}