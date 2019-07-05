using ChessLib.UCI.Commands.FromEngine;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace ChessLib.UCI
{
    public class EngineProcess : Process
    {
        public enum UciState { NeverStarted, Running, Exited }

        public event DataReceivedEventHandler ProcessLevelErrorReceived;
        public event EventHandler EngineExited;

        public IEngineMessageSubscriber MessageSubscriber;


        public EngineProcess(IEngineMessageSubscriber messageSubscriber) : base()
        {
            MessageSubscriber = messageSubscriber;
        }

        public UciState State { get; private set; } = UciState.NeverStarted;
        public string LastCommandSent { get; set; }
        public new virtual int ProcessId { get => base.Id; }
        private const char LineEnding = '\n';


        /// <summary>
        /// Fired each time the app communicates with the engine or a response is received.
        /// Contains raw text responses and commands.
        /// </summary>
        public event EventHandler<EngineCommunicationArgs> EngineCommunication;

        public virtual bool Start(string cliString, ProcessStartInfo startInfo)
        {
            StartInfo = startInfo;
            StartInfo.FileName = cliString;
            EnableRaisingEvents = true;
            OutputDataReceived += EngineDataReceived;
            Exited += EngineExited;
            ErrorDataReceived += ProcessLevelErrorReceived;
            EngineExited += OnEngineExit;
            var result = base.Start();
            State = UciState.Running;
            return result;
        }

        private void EngineDataReceived(object sender, DataReceivedEventArgs e)
        {
            MessageSubscriber.ProcessEngineResponse(e.Data);
        }

        public virtual new bool Start()
        {
            return base.Start();
        }

        private void OnEngineExit(object sender, EventArgs e)
        {
            State = UciState.Exited;
        }


        protected virtual new TextWriter StandardInput => base.StandardInput;


        private void OnEngineCommunication(EngineCommunicationArgs engineCommunicationArgs)
        {
            Volatile.Read(ref EngineCommunication)?.Invoke(this, engineCommunicationArgs);
        }

        private new ProcessPriorityClass PriorityClass { get => base.PriorityClass; set => base.PriorityClass = value; }
        public Guid EngineId { get; private set; }

        public virtual void SetPriority(ProcessPriorityClass priorityClass)
        {
            PriorityClass = priorityClass;
        }

        public new virtual bool WaitForExit(int waitTime)
        {
            if (State == UciState.Running) { return base.WaitForExit(waitTime); }
            else return true;
        }

        public new virtual void BeginErrorReadLine()
        {
            base.BeginErrorReadLine();
        }

        public new virtual void BeginOutputReadLine()
        {
            base.BeginErrorReadLine();
        }

        public virtual void SendCommandToEngine(string command)
        {
            LastCommandSent = command;
            StandardInput.Write(command + LineEnding);
            StandardInput.Flush();
        }

        public new void Dispose()
        {
            base.Dispose();
        }
    }
}
