using ChessLib.UCI;
using ChessLib.UCI.Commands;
using ChessLib.UCI.Commands.FromEngine;
using ChessLib.UCI.Commands.ToEngine;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace ChessLib.UCI
{
    public enum DebugInformationLevel
    {

    }
    public partial class Engine
    {



        /// <summary>
        /// Fired each time the app communicates with the engine or a response is received.
        /// Contains raw text responses and commands.
        /// </summary>
        public event EventHandler<EngineCommunicationArgs> EngineCommunication;

        /// <summary>
        /// Receives the important information from the engine with relevant object
        /// </summary>
        public event EventHandler<EngineInfoArgs> ResponseReceived;

        public event EventHandler<DebugEventArgs> DebugEventExecuted;

        public event EventHandler<StateChangeEventArgs> StateChanged;
        #region Event Raisers

        protected void OnEngineCommunication(object sender, DataReceivedEventArgs args)
        {
            OnEngineCommunication(new EngineCommunicationArgs(EngineCommunicationArgs.TextSource.Engine, args.Data));
        }

        protected void OnEngineCommunication(EngineCommunicationArgs args)
        {
            Volatile.Read(ref EngineCommunication)?.Invoke(this, args);
        }

        protected void OnEngineInfoReceived(EngineInfoArgs engineResponseArgs)
        {
            Volatile.Read(ref ResponseReceived)?.Invoke(this, engineResponseArgs);
        }

        protected void OnDebugEventExecuted(DebugEventArgs args)
        {
            Volatile.Read(ref DebugEventExecuted)?.Invoke(this, args);
        }

        protected void OnEngineStateChanged(StateChangeEventArgs args)
        {
            Volatile.Read(ref StateChanged)?.Invoke(this, args);
        }
        #endregion

        private const EngineToAppCommand UCIFlags = EngineToAppCommand.Id | EngineToAppCommand.Option | EngineToAppCommand.UCIOk;
        private void OnUCIResponseRecieved(object sender, DataReceivedEventArgs e)
        {

            string engineResponse = e.Data;
            EngineToAppCommand responseType;
            if (string.IsNullOrEmpty(engineResponse))
            {
                return;
            }
            EngineToAppCommand matchingFlag = EngineHelpers.GetResponseType(engineResponse);
            if (matchingFlag == EngineToAppCommand.None)
            {
                var message = $"**Message with no corresponding command received**\r\n\t{engineResponse}\r\n**End Message**";
                OnDebugEventExecuted(new DebugEventArgs(message));
            }

            IEngineResponse response = null;

            if (UCIFlags.HasFlag(matchingFlag))
            {
                ProcessUCIResponse(engineResponse, matchingFlag, out response);
            }
            else if (matchingFlag == EngineToAppCommand.Ready)
            {
                IsReady = true;
            }
            else if (matchingFlag == EngineToAppCommand.Info)
            {
                ProcessInfoResponse(engineResponse, matchingFlag, out response);
            }
            else if (matchingFlag == EngineToAppCommand.BestMove)
            {
                ProcessBestMoveResponse(engineResponse, out response);
            }
            if (matchingFlag == EngineToAppCommand.UCIOk)
            {
                UCIOk = true;
                OnEngineInfoReceived(new EngineInfoArgs(engineResponse, AppToUCICommand.UCI, matchingFlag, response));
            }
            return;

        }



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
    }
}