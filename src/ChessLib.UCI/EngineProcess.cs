using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using ChessLib.EngineInterface.UCI.Commands;
using ChessLib.EngineInterface.UCI.Commands.ToEngine;

namespace ChessLib.EngineInterface
{
    public interface IEngineProcess : IDisposable
    {

        /// <summary>
        /// Fired each time the app communicates with the engine or a response is received.
        /// Contains raw text responses and commands.
        /// </summary>
        event EventHandler<DebugEventArgs> EngineCommunicationReceived;
        /// <summary>
        /// Sent when process exits
        /// </summary>
        event EventHandler EngineProcessExited;
        /// <summary>
        /// Notifies on error information from the process
        /// </summary>
        event DataReceivedEventHandler ProcessErrorDataReceived;
        /// <summary>
        /// Sends basic debug information to subscribers. Verbose and not recommended for production
        /// </summary>
        event EventHandler<DebugEventArgs> DebugEventExecuted;

        EngineProcess.UciState State { get; }
        string LastCommandSent { get; }
        void Send(CommandInfo command);
        /// <summary>
        /// Sets the process' priority
        /// </summary>
        /// <param name="priorityClass"></param>
        void SetPriority(ProcessPriorityClass priorityClass);



        void Start(string cliString, ProcessPriorityClass initialPriority = ProcessPriorityClass.Normal);

        #region System.Diagnostics.Process members
        int Id { get; }
        void WaitForExit();
        bool WaitForExit(int milliseconds);
        void Kill();
        #endregion
    }

    public class EngineProcess : Process, IEngineProcess
    {
        CommandInfo _currentCommand;
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                EngineCommandQueue?.Dispose();
            }

            base.Dispose(disposing);
        }

        public enum UciState { NeverStarted, Running, Exited }
        public event EventHandler<DebugEventArgs> DebugEventExecuted;
        public event DataReceivedEventHandler ProcessErrorDataReceived;
        public event EventHandler EngineProcessExited;
        public IEngineMessageSubscriber MessageSubscriber { get; set; }
        protected CommandQueue EngineCommandQueue;

        public EngineProcess(IEngineMessageSubscriber messageSubscriber)
        {
            EngineCommandQueue = new CommandQueue();
            MessageSubscriber = messageSubscriber;
        }


        public UciState State { get; private set; } = UciState.NeverStarted;
        public string LastCommandSent { get; set; }
        public virtual int ProcessId { get; protected set; }
        private const char LineEnding = '\n';


        /// <summary>
        /// Fired each time the app communicates with the engine or a response is received.
        /// Contains raw text responses and commands.
        /// </summary>
        public event EventHandler<DebugEventArgs> EngineCommunicationReceived;

        public virtual void Start(string cliString, ProcessPriorityClass initialPriority = ProcessPriorityClass.Normal)
        {
            StartInfo = new ProcessStartInfo
            {
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true
            }; ;
            StartInfo.FileName = cliString;
            EnableRaisingEvents = true;
            OutputDataReceived += EngineDataReceived;
            Exited += OnProcessExited;
            ErrorDataReceived += ProcessErrorDataReceived;
            WriteDebugInformation($"Starting Engine Process\r\n\t{cliString}");
            base.Start();
            BeginErrorReadLine();
            BeginOutputReadLine();
            base.PriorityClass = initialPriority;
            ProcessId = Id;
            State = UciState.Running;
            WriteDebugInformation($"Engine started with priority of {initialPriority} and id {Id}");
            BeginMessageQueue();
        }

        private void WriteDebugInformation(string info)
        {
            DebugEventArgs debugEventArgs = new DebugEventArgs(info);
            Volatile.Read(ref DebugEventExecuted)?.Invoke(this, debugEventArgs);
        }

        public virtual void EngineDataReceived(object sender, DataReceivedEventArgs e)
        {
            var responseMessage = e.Data;
            //EngineCommandQueue.HandleAwaitedResponse(responseMessage);
            OnCommunicationReceived(new DebugEventArgs(responseMessage));
            HandleMessageFromEngine(responseMessage);
        }

        public void HandleMessageFromEngine(string responseMessage)
        {
            MessageSubscriber.ProcessEngineResponse(responseMessage, _currentCommand);
        }

        public new virtual bool Start()
        {
            return base.Start();
        }

        private void OnProcessExited(object sender, EventArgs e)
        {
            State = UciState.Exited;
            EngineCommandQueue.InterruptIssued.Set();
            Volatile.Read(ref EngineProcessExited)?.Invoke(this, e);
        }


        protected new virtual TextWriter StandardInput => base.StandardInput;


        private void OnCommunicationReceived(DebugEventArgs args)
        {
            Volatile.Read(ref EngineCommunicationReceived)?.Invoke(this, args);
        }

        public void Send(CommandInfo command)
        {

            EngineCommandQueue.Enqueue(command);
        }

        public virtual void SetPriority(ProcessPriorityClass priorityClass)
        {
            PriorityClass = priorityClass;
        }

        public new virtual void BeginErrorReadLine()
        {
            base.BeginErrorReadLine();
        }

        public new virtual void BeginOutputReadLine()
        {
            base.BeginOutputReadLine();
        }

        /// <summary>
        /// Sends a command to the engine's stdinput
        /// </summary>
        /// <param name="command"></param>
        private void Send(string command)
        {
            LastCommandSent = command;
            WriteDebugInformation($"[To {Id}] {command}");
            StandardInput.Write(command + LineEnding);
            StandardInput.Flush();
        }


        private void BeginMessageQueue()
        {
            var exit = false;
            WriteDebugInformation("Starting message queue.");
            while (!exit)
            {
                var waits = EngineCommandQueue.CommandIssuedEvents;
                if (_currentCommand != null && _currentCommand is AwaitableCommandInfo awaitableCommand)
                {
                    var handle = awaitableCommand.ResetEvent;
                    var waitResult = WaitHandle.WaitAny(new WaitHandle[] { handle, EngineCommandQueue.InterruptIssued });
                    if (waitResult == 1)
                    {
                        awaitableCommand.ResetEvent.Set();
                    }
                    awaitableCommand.Dispose();
                    awaitableCommand= null;
                    _currentCommand = null;
                }
                else
                {
                    if (!EngineCommandQueue.Any())
                    {
                        var waitResult = WaitHandle.WaitAny(waits);
                    }
                    else
                    {
                        EngineCommandQueue.TryDequeue(out var commandToIssue);
                        _currentCommand = commandToIssue;
                        Send(commandToIssue.ToString());
                        exit = commandToIssue.CommandSent == AppToUCICommand.Quit;
                    }
                }
            }
        }

        public new void Dispose()
        {
            base.Dispose();
        }
    }
}
