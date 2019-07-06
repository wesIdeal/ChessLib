using System;
using ChessLib.UCI.Commands;
using ChessLib.UCI.Commands.FromEngine;

namespace ChessLib.UCI
{
    public class PositionChangedArgs : EventArgs
    {

        /// <summary>
        /// StartingPositionFEN Position - Caller is responsible for passing in valid StartingPositionFEN
        /// </summary>
        public string FEN { get; set; }
    }

    public interface IEngineResponseFactory
    {
        EngineResponseArgs MakeResponseArgs(in string fen, in string engineText);
    }
    public class UCIResponseFactory : IEngineResponseFactory
    {
        private delegate void UCIInformationReceived(UCIResponse uciInformation);
        private readonly InfoResponseFactory _infoFactory;
        public UCIResponseFactory(bool includeMoveCalculationInformation = false)
        {
            _infoFactory = new InfoResponseFactory();
            _uciInformation = new UCIResponse();
            IncludeMoveCalculationInformation = includeMoveCalculationInformation;
        }

        readonly UCIResponse _uciInformation;

        public bool IncludeMoveCalculationInformation { get; }

        private const EngineToAppCommand UCIFlags = EngineToAppCommand.Id | EngineToAppCommand.Option | EngineToAppCommand.UCIOk;

        public EngineResponseArgs MakeResponseArgs(in string fen, in string engineText)
        {
            EngineToAppCommand responseFlag = EngineHelpers.GetResponseType(engineText.Split(' ')[0]);

            if (responseFlag == EngineToAppCommand.Info || responseFlag == EngineToAppCommand.BestMove)
            {
                var uciObject = _infoFactory.GetInfoResponse(engineText);
                return new EngineCalculationResponseArgs(uciObject, engineText);
            }
            if (responseFlag == EngineToAppCommand.Ready)
            {
                return new ReadyOkResponseArgs(engineText);
            }

            if (UCIFlags.HasFlag(responseFlag))
            {
                _uciInformation.SetInfoFromString(engineText);
                if (_uciInformation.UCIOk)
                {
                    return new EngineResponseArgs(_uciInformation, engineText);
                }
                return null;
            }

            return new EngineResponseArgs(new ErrorResponse("Response from engine unhandled."), engineText);
        }

    }
}
