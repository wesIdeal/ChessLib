using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ChessLib.Data.MoveRepresentation;
using ChessLib.UCI.Commands;
using ChessLib.UCI.Commands.FromEngine;
using ChessLib.UCI.Commands.ToEngine;

namespace ChessLib.UCI
{
    public class UCIEngine : Engine
    {
        public UCIEngine(Guid userEngineId) : base(userEngineId)
        {
            EngineInformation = new UCIResponseArgs(UserAssignedId);
            _responseFactory = new UCIResponseFactory(UserAssignedId);
        }

        public UCIEngine(Guid userEngineId, string description, string command) : base(userEngineId, description,
            command)
        {
        }

        public UCIEngine(Guid userEngineId, string description, string command, EngineProcess process) : base(userEngineId, description, command, process)
        {
        }

        public UCIEngine(Guid userEngineId, string description, string command, bool ignoreMoveCalculationLines) : base(userEngineId, description, command, ignoreMoveCalculationLines)
        {
        }

        public UCIEngine(Guid userEngineId, string description, string command, bool ignoreMoveCalculationLines,
            ProcessPriorityClass priority) : base(userEngineId, description, command, ignoreMoveCalculationLines,
            priority)
        {
        }

        public UCIEngine(Guid userEngineId, string description, string command, bool ignoreMoveCalculationLines,
            ProcessPriorityClass priority, Dictionary<string, string> engineOptions) : base(userEngineId, description,
            command, ignoreMoveCalculationLines, priority, engineOptions)
        {
        }
        [NonSerialized] private bool _isDisposed;
        [NonSerialized] private readonly UCIResponseFactory _responseFactory;
        [NonSerialized] public UCIResponseArgs EngineInformation;
        [NonSerialized] private readonly ManualResetEventSlim _readyOkReceived = new ManualResetEventSlim(false);
        [NonSerialized] private readonly ManualResetEventSlim _uciInfoReceived = new ManualResetEventSlim(false);
        [NonSerialized] private readonly string[] _uciFlags = { "id", "option" };

        [NonSerialized]
        private string timeoutFormat =
            "{0} did not return a result from command '{1}' in the specified timeout period of {2} seconds";

        private readonly TimeSpan _defaultTimeout = TimeSpan.FromSeconds(19);
        protected string GetTimeoutErrorMessage(string awaitedCommand, TimeSpan timeout) =>
            string.Format(timeoutFormat, ToString(), awaitedCommand, timeout.TotalSeconds);

        public void SendIsReady(TimeSpan? timeout = null)
        {
            timeout = timeout ?? _defaultTimeout;
            if (!SendIsReadyAsync().Wait((int)timeout.Value.TotalMilliseconds))
            {
                throw new Exception(GetTimeoutErrorMessage("isready", timeout.Value));
            }
        }
        protected Task SendIsReadyAsync()
        {
            var commandInfo = new CommandInfo(AppToUCICommand.IsReady);
            QueueCommand(commandInfo);
            return _readyOkReceived.WaitHandle.AsTask();
        }

        public UCIResponseArgs SendUCI(TimeSpan? timeout = null)
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
            return _uciInfoReceived.WaitHandle.AsTask();
        }

        public void SendStop()
        {
            var commandInfo = new CommandInfo(AppToUCICommand.Stop);
            QueueCommand(commandInfo);
        }

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

        public void SetOption(string optionName, string value)
        {
            var option = new SetOption(optionName, value);
            QueueCommand(option);
        }

        public override string ToString() =>
            EngineInformation != null
                ? EngineInformation.ToString()
                : $"[uninitialized] {UserAssignedId} {UserAssignedId}";

        protected override void ExecuteEngineAsync()
        {
            OnDebugEventExecuted(new DebugEventArgs(
                $"Engine Id: {UserAssignedId.ToString()} PID:{Process.Id}\r\nStarted as {EngineInformation}"));
            base.ExecuteEngineAsync();
        }

        public override void SendStartupMessagesToEngine()
        {
            TimeSpan timeout = TimeSpan.FromSeconds(10);
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

        protected override void EngineResponseReceived(object sender, DataReceivedEventArgs e)
        {

            string engineResponseText = e.Data;
            if (string.IsNullOrEmpty(engineResponseText)) { return; }

            OnEngineCommunication(new EngineCommunicationArgs(EngineCommunicationArgs.TextSource.Engine, e.Data));

            var response = _responseFactory.MakeResponseArgs(FEN, engineResponseText, out string error);

            if (response != null)
            {
                if (response is ReadyOkResponseArgs)
                {
                    _readyOkReceived.Set();
                }
                else if (response is UCIResponseArgs)
                {
                    _uciInfoReceived.Set();
                }
                OnEngineObjectReceived(response);
            }
            else if (!IsResponseUCI(engineResponseText))
            {
                var message = $"**Message with no corresponding command received**\r\n\t{engineResponseText}\r\n**End Message**";
                OnDebugEventExecuted(new DebugEventArgs(message));
            }
            if (!string.IsNullOrWhiteSpace(error))
            {
                OnDebugEventExecuted(new DebugEventArgs(error));
            }
        }

        protected bool IsResponseUCI(string response) => _uciFlags.Any(response.StartsWith);


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