using System;
using ChessLib.UCI.Commands;
using ChessLib.UCI.Commands.FromEngine;

namespace ChessLib.UCI
{
    public class PositionChangedArgs : EventArgs
    {

        /// <summary>
        /// FEN Position - Caller is responsible for passing in valid FEN
        /// </summary>
        public string FEN { get; set; }
    }

    public interface IEngineResponseFactory
    {
        EngineResponseArgs MakeResponseArgs(in string fen, in string communication, out string error);
    }
    public class UCIResponseFactory : IEngineResponseFactory
    {
        private delegate void UCIInformationReceived(UCIResponseArgs uciInformation);
        private readonly InfoResponseFactory _infoFactory;
        public UCIResponseFactory(bool includeMoveCalculationInformation = false)
        {
            _infoFactory = new InfoResponseFactory();
            _uciInformation = new UCIResponseArgs();
            IncludeMoveCalculationInformation = includeMoveCalculationInformation;
        }

        readonly UCIResponseArgs _uciInformation;

        public bool IncludeMoveCalculationInformation { get; }

        private const EngineToAppCommand UCIFlags = EngineToAppCommand.Id | EngineToAppCommand.Option | EngineToAppCommand.UCIOk;

        public EngineResponseArgs MakeResponseArgs(in string fen, in string communication, out string error)
        {
            EngineToAppCommand responseFlag = EngineHelpers.GetResponseType(communication.Split(' ')[0]);
            error = "";
            if (responseFlag == EngineToAppCommand.Info || responseFlag == EngineToAppCommand.BestMove)
            {
                return _infoFactory.GetInfoResponse(fen, communication);
            }
            if (responseFlag == EngineToAppCommand.Ready)
            {
                return new ReadyOkResponseArgs(communication);
            }

            if (UCIFlags.HasFlag(responseFlag))
            {
                _uciInformation.SetInfoFromString(communication);
                if (_uciInformation.UCIOk)
                {
                    return _uciInformation;
                }
                return null;
            }
            

            return null;

        }

    }
}
