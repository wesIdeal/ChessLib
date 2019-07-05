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

        public EngineProcess(Guid id) : base()
        {
            EngineId = id;
        }
        ~EngineProcess()
        {
            Dispose();
        }

        private readonly StringWriter _stringWriter = new StringWriter();
        private UciState _state = UciState.NeverStarted;
      
        private const char LineEnding = '\n';
        public UciState State => _state;
        public string LastCommandSent { get; set; }
        public new virtual int Id { get => base.Id; }
        
        public event DataReceivedEventHandler ErrorReceived;
        public event EventHandler<EngineCommunicationArgs> EngineCommunicationReceived;
        public event EventHandler EngineExited;
       
        /// <summary>
        /// Fired each time the app communicates with the engine or a response is received.
        /// Contains raw text responses and commands.
        /// </summary>
        public event EventHandler<EngineCommunicationArgs> EngineCommunication;

        public event DataReceivedEventHandler EngineDataReceived;

        public virtual bool Start(string cliString, ProcessStartInfo startInfo)
        {
            StartInfo = startInfo;
            StartInfo.FileName = cliString;
            EnableRaisingEvents = true;
            OutputDataReceived += EngineDataReceived;
            Exited += EngineExited;
            ErrorDataReceived += ErrorReceived;
            ErrorReceived += OnErrorReceived;
            EngineExited += OnEngineExit;
            var result = base.Start();
            _state = UciState.Running;
            return result;
        }

        public virtual new bool Start()
        {
            return base.Start();
        }

        private void OnEngineExit(object sender, EventArgs e)
        {
            _state = UciState.Exited;
        }

        private void OnErrorReceived(object sender, DataReceivedEventArgs e)
        {

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
            _stringWriter?.Dispose();

        }
    }
}
