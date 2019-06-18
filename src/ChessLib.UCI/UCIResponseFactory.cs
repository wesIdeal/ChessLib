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
    public class UCIResponseFactory
    {
        private readonly InfoResponseFactory _infoFactory;
        public UCIResponseFactory(Guid engineId, bool ignoreSingleMoveCalcLines = true)
        {
            _id = engineId;
            _infoFactory = new InfoResponseFactory();
            UCIInformation = new OptionsResponseArgs(_id);
            IgnoreMoveCalculationLines = ignoreSingleMoveCalcLines;
        }
        OptionsResponseArgs UCIInformation;

        public bool IgnoreMoveCalculationLines { get; }

        private Guid _id;
        private const EngineToAppCommand UCIFlags = EngineToAppCommand.Id | EngineToAppCommand.Option | EngineToAppCommand.UCIOk;

        public EngineResponseArgs MakeResponseArgs(in string fen, in string communication, out string error)
        {
            EngineToAppCommand responseFlag = EngineHelpers.GetResponseType(communication);
            error = "";
            if (UCIFlags.HasFlag(responseFlag))
            {
                UCIInformation.SetInfoFromString(communication);
                if (UCIInformation.UCIOk)
                {
                    return UCIInformation;
                }
                else
                {
                    return null;
                }
            }
            else if (responseFlag == EngineToAppCommand.Ready)
            {
                return new ReadyOkResponseArgs(communication);
            }
            else if (responseFlag == EngineToAppCommand.Info || responseFlag == EngineToAppCommand.BestMove)
            {
                return _infoFactory.GetInfoResponse(fen, communication);
            }

            else if (responseFlag == EngineToAppCommand.UCIOk)
            {
                return new ReadyOkResponseArgs(communication);
            }

            else
            {
                return null;
            }

        }

    }
}
