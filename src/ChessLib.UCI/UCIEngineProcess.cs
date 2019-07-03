//using System.Diagnostics;
//using System.IO;
//using System;
//using System.Threading;
//using System.Text;
//using ChessLib.UCI.Commands;
//using ChessLib.UCI.Commands.FromEngine;
//using ChessLib.Data.MoveRepresentation;

//namespace ChessLib.UCI
//{
//    public class UCIEngineProcess : Process
//    {
//        public enum UCIState { NeverStarted, Running, Exited }

//        public UCIEngineProcess(Guid id) : base()
//        {
//            EngineId = id;
//            _responseFactory = new UCIResponseFactory(EngineId);
//        }
//        ~UCIEngineProcess()
//        {
//            Dispose();
//        }

//        private StringWriter stringWriter = new StringWriter();
//        private UCIState _state = UCIState.NeverStarted;
//        private string _fen;

//        public UCIState State => _state;
//        public string LastCommandSent { get; set; }
//        public new virtual int Id { get => base.Id; }
//        public event EventHandler<EngineResponseArgs> EngineResponseObjectReceived;
//        public event DataReceivedEventHandler ErrorReceived;
//        public event EventHandler<EngineCommunicationArgs> EngineCommunicationReceived;
//        public event EventHandler EngineExited;
//        private readonly UCIResponseFactory _responseFactory;
//        /// <summary>
//        /// Fired each time the app communicates with the engine or a response is received.
//        /// Contains raw text responses and commands.
//        /// </summary>
//        public event EventHandler<EngineCommunicationArgs> EngineCommunication;

//        public virtual bool Start(string cliString, ProcessStartInfo startInfo)
//        {
//            StartInfo = startInfo;
//            StartInfo.FileName = cliString;
//            EnableRaisingEvents = true;
//            OutputDataReceived += EngineResponseReceived;
//            Exited += EngineExited;
//            ErrorDataReceived += ErrorReceived;
//            ErrorReceived += OnErrorReceived;
//            EngineExited += OnEngineExit;
//            var result = base.Start();
//            _state = UCIState.Running;
//            return result;
//        }

//        public virtual new bool Start()
//        {
//            return base.Start();
//        }

//        private void OnEngineExit(object sender, EventArgs e)
//        {
//            _state = UCIState.Exited;
//        }

//        private void OnErrorReceived(object sender, DataReceivedEventArgs e)
//        {

//        }

//        protected virtual new TextWriter StandardInput => base.StandardInput;
//        private readonly string[] UCIFlags = new string[] { "id", "option" }; //EngineToAppCommand.Id | EngineToAppCommand.Option | EngineToAppCommand.UCIOk;

//        public virtual void SetPosition(string fen)
//        {
//            _fen = fen;
//        }

//        private void EngineResponseReceived(object sender, DataReceivedEventArgs e)
//        {

//            string engineResponse = e.Data;
//            if (string.IsNullOrEmpty(engineResponse)) { return; }

//            OnEngineCommunication(new EngineCommunicationArgs(EngineCommunicationArgs.TextSource.Engine, e.Data));

//            var response = _responseFactory.MakeResponseArgs(_fen, engineResponse, out string error);
//            var isUci = false;
//            foreach (var uci in UCIFlags)
//            {
//                isUci |= engineResponse.StartsWith(uci);
//            }
//            if (response != null)
//            {
//                OnEngineObjectReceived(response);
//            }

//            else if (!isUci)
//            {
//                var message = $"**Message with no corresponding command received**\r\n\t{engineResponse}\r\n**End Message**";
//                ///OnDebugEventExecuted(new DebugEventArgs(message));
//            }
//        }

//        private void OnEngineObjectReceived(EngineResponseArgs response)
//        {
//            Volatile.Read(ref EngineResponseObjectReceived)?.Invoke(this, response);
//        }

//        private void OnEngineCommunication(EngineCommunicationArgs engineCommunicationArgs)
//        {
//            Volatile.Read(ref EngineCommunication)?.Invoke(this, engineCommunicationArgs);
//        }

//        private new ProcessPriorityClass PriorityClass { get => base.PriorityClass; set => base.PriorityClass = value; }
//        public Guid EngineId { get; private set; }

//        public virtual void SetPriority(ProcessPriorityClass priorityClass)
//        {
//            PriorityClass = priorityClass;
//        }

//        public new virtual bool WaitForExit(int waitTime)
//        {
//            if (State == UCIState.Running) { return base.WaitForExit(waitTime); }
//            else return true;
//        }

//        public new virtual void BeginErrorReadLine()
//        {
//            base.BeginErrorReadLine();
//        }

//        public new virtual void BeginOutputReadLine()
//        {
//            base.BeginErrorReadLine();
//        }

//        public virtual void SendCommmand(string command)
//        {
//            LastCommandSent = command;
//            StandardInput.WriteLine(command);
//            StandardInput.Flush();
//        }

//        public new void Dispose()
//        {
//            base.Dispose();
//            stringWriter?.Dispose();

//        }
//    }
//}
