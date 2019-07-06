using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ChessLib.Data;
using ChessLib.Data.MoveRepresentation;
using ChessLib.EngineInterface.Commands;
using ChessLib.EngineInterface.Commands.FromEngine;
using ChessLib.EngineInterface.Commands.ToEngine;

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
    }

    public interface IUCIEngineStartArgs : IEngineStartupArgs
    {
        bool IgnoreMoveCalculationLines { get; set; }
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
        private readonly MoveTranslatorService _moveTranslatorService;
        public new IUCIEngineStartArgs UserSuppliedArgs { get; }

        public UCIEngine(UCIEngineStartupArgs args, UCIEngineMessageSubscriber engineMessageSubscriber = null, EngineProcess process = null)
        : base(args, engineMessageSubscriber)
        {
            UserSuppliedArgs = args;
            _moveTranslatorService = new MoveTranslatorService();
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
                return;
            }
            else if (engineResponse is ReadyOkResponseArgs)
            {
                _readyOkReceived.Set();
            }
            else if (engineResponse.ResponseObject is UCIResponse)
            {
                _uciInfoReceived.Set();
            }
            else if (engineResponse is EngineCalculationResponseArgs calcResponse)
            {
                engineResponse.ResponseObject = FillCalculationResponseWithMoveObjects(calcResponse.ResponseObject);
            }
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

        public void SendNewGame()
        {
            SendPosition();
        }
        public override void SendPosition()
        {
            base.SendPosition();
            _moveTranslatorService.InitializeBoard();
            var commandInfo = new CommandInfo(AppToUCICommand.NewGame);
            QueueCommand(commandInfo);
        }

        public override void SendPosition(string fen)
        {
            SendPosition(fen, new MoveExt[] { });
        }

        public override void SendPosition(MoveExt[] moves)
        {
            SendIsReady();
            base.SendPosition(moves);
            _moveTranslatorService.InitializeBoard(CurrentFEN);
            SendPosition(StartingPositionFEN, moves);
        }

        public override void SendPosition(string fen, MoveExt[] moves)
        {
            SendIsReady();
            base.SendPosition(fen);
            base.SendPosition(moves);
            _moveTranslatorService.InitializeBoard(CurrentFEN);
            var commandInfo = new CommandInfo(AppToUCICommand.Position);
            var positionString = $"fen {StartingPositionFEN}";
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
            SendIsReady();
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

        private IResponseObject FillCalculationResponseWithMoveObjects(in ICalculationInfoResponse calculation)
        {
            if (calculation.ResponseType == CalculationResponseTypes.BestMove)
            {
                var bmResponse = calculation as IBestMoveResponse;
                FillBestMoveResponseWithMoveObjects(ref bmResponse);
                return bmResponse;
            }

            if (calculation.ResponseType == CalculationResponseTypes.CalculationInformation)
            {
                var infoCalcResponse = calculation as IInfoCalculationResponse;
                FillBestMoveResponseWithMoveObjects(ref infoCalcResponse);
                return infoCalcResponse;
            }

            if (calculation.ResponseType == CalculationResponseTypes.PrincipalVariation)
            {
                var pvResponse = calculation as IPrincipalVariationResponse;
                FillBestMoveResponseWithMoveObjects(ref pvResponse);
                return pvResponse;
            }
            return calculation;
        }

        private void FillBestMoveResponseWithMoveObjects(ref IPrincipalVariationResponse principalVariationResponse)
        {
            principalVariationResponse.Variation = _moveTranslatorService.FromLongAlgebraicNotation(principalVariationResponse.VariationLong).ToArray();
        }

        private void FillBestMoveResponseWithMoveObjects(ref IInfoCalculationResponse infoCalcResponse)
        {
            infoCalcResponse.CurrentMove = _moveTranslatorService.FromLongAlgebraicNotation(infoCalcResponse.CurrentMoveLong);
        }

        private void FillBestMoveResponseWithMoveObjects(ref IBestMoveResponse bestMoveResponse)
        {
            bestMoveResponse.BestMove = _moveTranslatorService.FromLongAlgebraicNotation(bestMoveResponse.BestMoveLong);
            if (!string.IsNullOrWhiteSpace(bestMoveResponse.PonderMoveLong))
            {
                bestMoveResponse.PonderMove =
                    _moveTranslatorService.FromLongAlgebraicNotation(bestMoveResponse.PonderMoveLong);
            }
        }

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