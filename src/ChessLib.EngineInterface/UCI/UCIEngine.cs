using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ChessLib.Core;
using ChessLib.Core.IO;
using ChessLib.Core.Parse;
using ChessLib.Data;
using ChessLib.EngineInterface.UCI.Commands;
using ChessLib.EngineInterface.UCI.Commands.FromEngine;
using ChessLib.EngineInterface.UCI.Commands.ToEngine;

namespace ChessLib.EngineInterface.UCI
{
    public sealed class UCIEngine : Engine
    {
        private readonly MoveTranslatorService _moveTranslatorService;
        public new UCIEngineStartupArgs UserSuppliedArgs { get; }
        public override InterruptCommand QuitCommand { get; } = new InterruptCommand(AppToUCICommand.Quit);
        public override InterruptCommand StopCommand { get; } = new InterruptCommand(AppToUCICommand.Stop);

        public UCIEngine(UCIEngineStartupArgs args)
            : base(args)
        {
            UserSuppliedArgs = args;
            _moveTranslatorService = new MoveTranslatorService();
            InitializeEngineProcess(new UCIEngineMessageSubscriber(ResponseReceived));
        }

        public UCIEngine(UCIEngineStartupArgs args, EngineProcess process)
        : base(args, process)
        {
            UserSuppliedArgs = args;
        }

        public void ResponseReceived(EngineResponseArgs engineResponse)
        {
            if (engineResponse == null)
            {
                OnDebugEventExecuted(new DebugEventArgs("Received null object from engine."));
                return;
            }
            else if (engineResponse is EngineCalculationResponseArgs calcResponse && !PauseEngineMessageHandling)
            {
                engineResponse.ResponseObject = FillCalculationResponseWithMoveObjects(calcResponse.ResponseObject);
            }
            OnEngineObjectReceived(engineResponse);
        }


        [NonSerialized] private bool _isDisposed;
        [NonSerialized] public UCIResponse EngineInformation;

        public override ManualResetEvent SendIsReadyAsync()
        {
            PauseEngineMessageHandling = true;
            var commandInfo = new AwaitableCommandInfo(AppToUCICommand.IsReady);
            QueueCommand(commandInfo);
            return commandInfo.ResetEvent;
        }

        public UCIResponse SendUCI(TimeSpan? timeout = null)
        {
            using (var resetEvent = SendUCIAsync())
            {
                if (!resetEvent.WaitOne())
                {
                    throw new TimeoutException("Timeout waiting for event after sending uci command.");
                }
            }

            return EngineInformation;
        }

        public ManualResetEvent SendUCIAsync()
        {
            var commandInfo = new AwaitableCommandInfo(AppToUCICommand.IsReady);
            QueueCommand(commandInfo);
            return commandInfo.ResetEvent;
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
            SendPosition(fen, new Move[] { });
        }

        public override void SendPosition(Move[] moves)
        {
            base.SendPosition(moves);
            _moveTranslatorService.InitializeBoard(CurrentFEN);
            SendPosition(StartingPositionFEN, moves);
        }

        public override void SendPosition(string fen, Move[] moves)
        {
            base.SendPosition(fen, moves);
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
        public void SendGo(TimeSpan searchTime, Move[] searchMoves = null)
        {
            QueueCommand(new Go(searchTime, searchMoves));
        }

        public void SendGoInfinite()
        {
            QueueCommand(new Go());
        }

        #endregion

        #region Other Command Methods
        public void SetOption(string optionName, string optionValue)
        {
            var option = new SetOption(optionName, optionValue);
            QueueCommand(option);
        }
        #endregion



        public override string ToString() =>
            EngineInformation != null
                ? EngineInformation.ToString()
                : $"[uninitialized] {UserAssignedId} {UserAssignedId}";

        public override void SendStop()
        {
            base.SendStop();
            SendIsReadyAsync().WaitOne();
            PauseEngineMessageHandling = false;
        }

        public override Task StartAsync()
        {
            var task = base.StartAsync();
            SendStartupMessagesToEngine();
            return task;
        }

        public void SendStartupMessagesToEngine()
        {
            TimeSpan timeout = TimeSpan.FromSeconds(100);
            SendUCI(timeout);
            SendIsReady();
        }

        protected override void ProcessEngineStartupOptions()
        {
            if (EngineOptions == null) return;
            foreach (var option in EngineOptions)
            {
                if (!EngineInformation.SupportsOption(option.Key))
                {
                    throw new ArgumentException($"Option passed from command line {option.Key} is not valid.");
                }

                if (EngineInformation.IsOptionValid(option.Key, option.Value, out var message))
                {
                    throw new ArgumentException(message);
                }
                QueueCommand(new SetOption(option.Key, option.Value));
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
            }
            _isDisposed = true;
            base.Dispose(disposing);
        }

    }

    //internal static class WaitHandleExtensions
    //{
    //    public static Task AsTask(this WaitHandle handle)
    //    {
    //        return AsTask(handle, Timeout.InfiniteTimeSpan);
    //    }

    //    public static Task AsTask(this WaitHandle handle, TimeSpan timeout)
    //    {
    //        var tcs = new TaskCompletionSource<object>();
    //        var registration = ThreadPool.RegisterWaitForSingleObject(handle, (state, timedOut) =>
    //        {
    //            var localTcs = (TaskCompletionSource<object>)state;
    //            if (timedOut)
    //                localTcs.TrySetCanceled();
    //            else
    //                localTcs.TrySetResult(null);
    //        }, tcs, timeout, executeOnlyOnce: true);
    //        tcs.Task.ContinueWith((_, state) => ((RegisteredWaitHandle)state).Unregister(null), registration, TaskScheduler.Default);
    //        return tcs.Task;
    //    }
    //}
}