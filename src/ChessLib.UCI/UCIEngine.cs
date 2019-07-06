using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ChessLib.Data;
using ChessLib.Data.Helpers;
using ChessLib.Data.MoveRepresentation;
using ChessLib.UCI.Commands;
using ChessLib.UCI.Commands.FromEngine;
using ChessLib.UCI.Commands.ToEngine;

namespace ChessLib.UCI
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
    }

    public interface IUCIEngineStartArgs : IEngineStartupArgs
    {
        bool IgnoreMoveCalculationLines { get; set; }
    }

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

    public class UCIEngineStartupArgs : EngineStartupArgs, IUCIEngineStartArgs
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
    public sealed class UCIEngine : Engine
    {
        public new IUCIEngineStartArgs UserSuppliedArgs { get; protected set; }

        public UCIEngine(UCIEngineStartupArgs args, UCIEngineMessageSubscriber engineMessageSubscriber = null, EngineProcess process = null)
        : base(args, engineMessageSubscriber)
        {
            UserSuppliedArgs = args;
            MessageSubscriber = engineMessageSubscriber ?? new UCIEngineMessageSubscriber(ResponseReceived);
            if (process != null)
            {
                Process = process;
            }
        }

        public void ResponseReceived(EngineResponseArgs engineResponse)
        {
            if (engineResponse == null)
            {
                OnDebugEventExecuted(new DebugEventArgs("Received null object from engine."));
            }
            else if (engineResponse is ReadyOkResponseArgs)
            {
                _readyOkReceived.Set();
            }
            else if (engineResponse.ResponseObject is UCIResponse)
            {
                _uciInfoReceived.Set();
            }
            engineResponse.Id = UserAssignedId;
            OnEngineObjectReceived(engineResponse);
        }


        [NonSerialized] private bool _isDisposed;
        [NonSerialized] public UCIResponse EngineInformation;
        [NonSerialized] private readonly AutoResetEvent _readyOkReceived = new AutoResetEvent(false);
        [NonSerialized] private readonly AutoResetEvent _uciInfoReceived = new AutoResetEvent(false);
        [NonSerialized] private readonly string[] _uciFlags = { "id", "option" };

        protected override IEngineMessageSubscriber MessageSubscriber
        {
            get => _engineMessageSubscriber ?? (_engineMessageSubscriber = new UCIEngineMessageSubscriber(ResponseReceived));
            set => _engineMessageSubscriber = (UCIEngineMessageSubscriber)value;
        }

        [NonSerialized] private string timeoutFormat = "{0} did not return a result from command '{1}' in the specified timeout period of {2} seconds";

        private readonly TimeSpan _defaultTimeout = TimeSpan.FromSeconds(10);
        private UCIEngineMessageSubscriber _engineMessageSubscriber;

        private string GetTimeoutErrorMessage(string awaitedCommand, TimeSpan timeout) =>
            string.Format(timeoutFormat, ToString(), awaitedCommand, timeout.TotalSeconds);

        public void SendIsReady(TimeSpan? timeout = null)
        {
            timeout = timeout ?? _defaultTimeout;
            if (!SendIsReadyAsync().Wait((int)timeout.Value.TotalMilliseconds))
            {
                throw new Exception(GetTimeoutErrorMessage("isready", timeout.Value));
            }
        }

        private Task SendIsReadyAsync()
        {
            var commandInfo = new CommandInfo(AppToUCICommand.IsReady);
            QueueCommand(commandInfo);
            return _readyOkReceived.AsTask();
        }

        public UCIResponse SendUCI(TimeSpan? timeout = null)
        {
            timeout = timeout ?? _defaultTimeout;
            if (!SendUCIAsync().Wait((int)timeout.Value.TotalMilliseconds))
            {
                throw new Exception(GetTimeoutErrorMessage("uci", timeout.Value));
            }
            return EngineInformation;
        }

        public Task SendUCIAsync()
        {
            var commandInfo = new CommandInfo(AppToUCICommand.UCI);
            QueueCommand(commandInfo);
            return _uciInfoReceived.AsTask();
        }

        public void SendStop()
        {
            var commandInfo = new CommandInfo(AppToUCICommand.Stop);
            QueueCommand(commandInfo);
        }

        #region Send Position Command Methods
        public override void SendPosition()
        {
            var commandInfo = new CommandInfo(AppToUCICommand.NewGame);
            QueueCommand(commandInfo);
        }

        public override void SendPosition(string fen)
        {
            FEN = fen;
            SendPosition(fen, new MoveExt[] { });
        }



        public override void SendPosition(MoveExt[] moves)
        {
            FEN = FENHelpers.FENInitial;
            SendPosition(FEN, moves);
        }

        public override void SendPosition(string fen, MoveExt[] moves)
        {
            FEN = fen;
            var commandInfo = new CommandInfo(AppToUCICommand.Position);
            var positionString = "startpos";
            if (FEN != FENHelpers.FENInitial) positionString = $"fen {FEN}";
            var moveString = "";
            if (moves.Any())
                moveString = " moves " + string.Join(" ", moves.Select(MoveDisplayService.MoveToLan));
            QueueCommand(commandInfo, $"{positionString}{moveString}");
        }

        #endregion

        #region Analysis Command Methods
        /// <summary>
        ///     Starts a search for set amount of time
        /// </summary>
        /// <param name="searchTime">Time to spend searching</param>
        /// <param name="searchMoves">only consider these moves</param>
        public void SendGo(TimeSpan searchTime, MoveExt[] searchMoves = null)
        {
            QueueCommand(new Go(searchTime, searchMoves));
        }

        #endregion

        #region Other Command Methods
        public override void SendQuit()
        {
            var commandInfo = new CommandInfo(AppToUCICommand.Quit);
            QueueCommand(commandInfo);
        }
        public override void SetOption(string optionName, string optionValue)
        {
            var option = new SetOption(optionName, optionValue);
            QueueCommand(option);
        }
        #endregion



        public override string ToString() =>
            EngineInformation != null
                ? EngineInformation.ToString()
                : $"[uninitialized] {UserAssignedId} {UserAssignedId}";

        public override Task StartAsync()
        {
            var task = base.StartAsync();
            SendStartupMessagesToEngine();
            return task;
        }

        protected override void ExecuteEngineAsync()
        {
            OnDebugEventExecuted(new DebugEventArgs(
                $"Engine Id: {UserSuppliedArgs.EngineId.ToString()} PID:{Process.ProcessId}\r\nStarted as {EngineInformation}"));
            base.ExecuteEngineAsync();
        }



        public override void SendStartupMessagesToEngine()
        {
            TimeSpan timeout = TimeSpan.FromSeconds(100);
            SendUCI(timeout);
            SendIsReady(timeout);
        }

        protected override void ProcessEngineStartupOptions()
        {
            if (EngineOptions == null) return;
            foreach (var option in EngineOptions)
            {
                if (!EngineInformation.SupportsOption(option.Key))
                    throw new UCICommandException($"Option passed from command line {option.Key} is not valid.");
                if (EngineInformation.IsOptionValid(option.Key, option.Value, out var message))
                    throw new UCICommandException(message);
                WriteToEngine(new SetOption(option.Key, option.Value));
            }
        }


        private bool IsResponseUCI(string response) => _uciFlags.Any(response.StartsWith);


        protected override void Dispose(bool disposing)
        {
            if (_isDisposed)
            {
                return;
            }

            if (disposing)
            {
                _readyOkReceived.Dispose();
                _uciInfoReceived.Dispose();
            }
            _isDisposed = true;
            base.Dispose(disposing);
        }
    }

    internal static class WaitHandleExtensions
    {
        public static Task AsTask(this WaitHandle handle)
        {
            return AsTask(handle, Timeout.InfiniteTimeSpan);
        }

        public static Task AsTask(this WaitHandle handle, TimeSpan timeout)
        {
            var tcs = new TaskCompletionSource<object>();
            var registration = ThreadPool.RegisterWaitForSingleObject(handle, (state, timedOut) =>
            {
                var localTcs = (TaskCompletionSource<object>)state;
                if (timedOut)
                    localTcs.TrySetCanceled();
                else
                    localTcs.TrySetResult(null);
            }, tcs, timeout, executeOnlyOnce: true);
            tcs.Task.ContinueWith((_, state) => ((RegisteredWaitHandle)state).Unregister(null), registration, TaskScheduler.Default);
            return tcs.Task;
        }
    }
}