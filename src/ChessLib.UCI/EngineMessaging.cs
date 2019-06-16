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

        public event EventHandler<DataReceivedEventArgs> ErrorReceivedFromEngineProcess;
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

        private void OnErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            var message = e.Data;
            if (string.IsNullOrEmpty(message))
            {
                message = "No message received.";
                errorCloseEvent.SetResult(true);
            }
            else
            {
                _errorBuilder.AppendLine(message);
            }
            OnDebugEventExecuted(new DebugEventArgs(message));
            Volatile.Read(ref ErrorReceivedFromEngineProcess)?.Invoke(sender, e);
        }
        #endregion

        private const EngineToAppCommand UCIFlags = EngineToAppCommand.Id | EngineToAppCommand.Option | EngineToAppCommand.UCIOk;
        private void OnUCIResponseRecieved(object sender, DataReceivedEventArgs e)
        {

            string engineResponse = e.Data;
            if (string.IsNullOrEmpty(engineResponse))
            {
                return;
            }
            OnEngineCommunication(new EngineCommunicationArgs(EngineCommunicationArgs.TextSource.Engine, e.Data));
            EngineToAppCommand responseFlag = EngineHelpers.GetResponseType(engineResponse);

            IEngineResponse response = null;

            if (UCIFlags.HasFlag(responseFlag))
            {
                ProcessUCIResponse(engineResponse, responseFlag, out response);
            }
            else if (responseFlag == EngineToAppCommand.Ready)
            {
                IsReady = true;
                ReadyOkReceived.Set();
            }
            else if (responseFlag == EngineToAppCommand.Info)
            {
                ProcessInfoResponse(engineResponse, out response);
            }
            else if (responseFlag == EngineToAppCommand.BestMove)
            {
                ProcessBestMoveResponse(engineResponse, out response);
            }
            else if (responseFlag == EngineToAppCommand.UCIOk)
            {
                UCIOk = true;
                OnEngineInfoReceived(new EngineInfoArgs(engineResponse, AppToUCICommand.UCI, responseFlag, response));
            }
            else
            {
                var message = $"**Message with no corresponding command received**\r\n\t{engineResponse}\r\n**End Message**";
                OnDebugEventExecuted(new DebugEventArgs(message));
            }
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