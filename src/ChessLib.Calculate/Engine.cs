using System;
using System.Diagnostics;
using System.Linq;
using System.Collections;
using System.Threading;
using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;
using System.Text;

namespace ChessLib.UCI
{
    [Serializable]
    public class Engine : IDisposable
    {
        public Guid Id => _uid;
        private Guid _uid;
        public bool IsReady { get; set; }

        public string Description { get; set; }
        public string Command { get; private set; }
        public string[] UciArguments { get; private set; }
        private AutoResetEvent UCICommandIssued = new AutoResetEvent(false);
        private AutoResetEvent UCIShutdownIssued = new AutoResetEvent(false);
        private AutoResetEvent UCIResponseRecieved = new AutoResetEvent(false);
        private StringBuilder sbUciResponse = new StringBuilder();
        [NonSerialized]
        public UCIEngineInformation EngineInformation;
        [NonSerialized]
        private ConcurrentQueue<UCICommandInfo> _uciCommandQueue = new ConcurrentQueue<UCICommandInfo>();
        private UCICommandInfo? _currentCommand;
        public ProcessPriorityClass Priority
        {
            get => _priority;
            private set => _priority = value;
        }
        ~Engine()
        {
            Dispose();
        }

        public void SetPriority(ProcessPriorityClass priority)
        {
            _priority = priority;
            if (_process != null)
            {
                _process.PriorityClass = priority;
            }
        }

        public void SendCommand(UCICommandInfo commandInfo, params string[] args)
        {
            if (!GetIsProcessRunning())
            {
                throw new NullReferenceException("Process must be started before sending command.");
            }
            Debug.WriteLine($"SENDING\t{commandInfo.Command}");
            commandInfo.SetCommandArguments(args);
            _uciCommandQueue.Enqueue(commandInfo);
        }

        private bool GetIsProcessRunning()
        {
            return _process != null;
        }

        public ReceiveOutput _recieveOutput;
        private OnUCIInfoReceived _engineInfoReceived;


        [NonSerialized] public Process _process;
        [NonSerialized]
        ProcessStartInfo startInfo = new ProcessStartInfo()
        {
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardError = true,
            RedirectStandardInput = true,
            RedirectStandardOutput = true
        };
        private ProcessPriorityClass _priority;

        public Engine(string description, string command, string[] uciArguments, ReceiveOutput recieveOutput = null, OnUCIInfoReceived engineInfoReceived = null, Guid? id = null, ProcessPriorityClass priority = ProcessPriorityClass.Normal)
        {
            _uid = id ?? Guid.NewGuid();
            Description = description;
            Command = command;
            UciArguments = uciArguments;
            Priority = priority;
            _recieveOutput = recieveOutput;
            _engineInfoReceived = engineInfoReceived;
        }

        public async Task StartAsync()
        {
            EngineInformation = new UCIEngineInformation();
            using (_process = new Process())
            {
                _process.StartInfo = startInfo;
                _process.StartInfo.FileName = Command;
                _process.Start();
                _process.PriorityClass = _priority;
                _process.BeginErrorReadLine();
                _process.BeginOutputReadLine();
                _process.OutputDataReceived += OnUCIResponseRecieved;
                this.SendUCI();
                await Task.Run(() => { ExecuteEngine(null); });
            }
        }

        private void OnUCIResponseRecieved(object sender, DataReceivedEventArgs e)
        {

            string engineResponse = e.Data;
            if (!_currentCommand.HasValue)
            {
                _recieveOutput?.Invoke(Id, Description, engineResponse);
                return;
            }
            var command = _currentCommand.Value;


            if (string.IsNullOrEmpty(engineResponse))
            {
                return;
            }

            if (_currentCommand.HasValue && _currentCommand.Value.AwaitResponse)
            {
                if (command.Command == "uci")
                {
                    sbUciResponse.AppendLine(engineResponse);
                    if (command.IsResponseTheExpectedResponse(engineResponse))
                    {
                        EngineInformation = new UCIEngineInformation(sbUciResponse.ToString());
                        _engineInfoReceived?.Invoke(Id, EngineInformation);
                        _currentCommand = null;
                        UCIResponseRecieved.Set();
                        return;
                    }
                }
                else if (command.Command == "isready")
                {
                    if (command.IsResponseTheExpectedResponse(engineResponse))
                    {
                        _currentCommand = null;
                        IsReady = true;
                    }
                }

            }
            else
            {
                UCIResponseRecieved.Set();
            }
            _recieveOutput?.Invoke(Id, Description, engineResponse);

        }

        private void ExecuteEngine(object state)
        {
            var exit = false;
            int waitResult = WaitHandle.WaitTimeout;
            var commandEvents = new[] { UCICommandIssued, UCIShutdownIssued };
            var finishedEvents = new[] { UCIResponseRecieved, UCIShutdownIssued };
            const int ShutdownHandle = 1;
            StreamWriter engineInputStream = _process.StandardInput;
            while (!exit)
            {
                //Wait for new commands to come in if none are on the queue
                if (!_uciCommandQueue.Any())
                {
                    waitResult = WaitHandle.WaitAny(commandEvents);
                }
                //Exit if we get a shutdown handle
                if (waitResult == ShutdownHandle)
                {
                    exit = true;
                    continue;
                }
                else
                {
                    if (_uciCommandQueue.TryDequeue(out UCICommandInfo commandToIssue))
                    {
                        _currentCommand = commandToIssue;
                        if (_currentCommand?.Command == "quit")
                        {
                            exit = true;
                            UCIShutdownIssued.Set();
                        }
                        engineInputStream.WriteLine(commandToIssue.GetFullCommand());
                    }
                    waitResult = WaitHandle.WaitAny(finishedEvents);
                    if (waitResult == ShutdownHandle)
                    {
                        exit = true;
                    }
                }
            }

        }

        public void Dispose()
        {
            _process.OutputDataReceived -= OnUCIResponseRecieved;
            // Dispose of the process
            _process.Dispose();
            GC.SuppressFinalize(this);
        }


    }
}
