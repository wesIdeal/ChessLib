using System;
using System.Diagnostics;
using System.Linq;
using System.Collections;
using System.Threading;
using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;
using System.Text;
using EnumsNET;
using ChessLib.Data.Helpers;
using ChessLib.Data.MoveRepresentation;

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
        public string _fen;
        private AutoResetEvent UCICommandIssued = new AutoResetEvent(false);

        private AutoResetEvent UCIResponseRecieved = new AutoResetEvent(false);
        private StringBuilder _sbUciResponse = new StringBuilder();
        [NonSerialized]
        public UCIEngineInformation EngineInformation;
        [NonSerialized]
        private ConcurrentQueue<UCICommandInfo> _uciCommandQueue = new ConcurrentQueue<UCICommandInfo>();
        private UCICommandInfo _currentCommand;
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

        public void QueueCommand(UCICommandInfo commandInfo, params string[] args)
        {
            if (!GetIsProcessRunning())
            {
                throw new NullReferenceException("Process must be started before sending command.");
            }
            Debug.WriteLine($"SENDING\t{commandInfo.CommandText}");
            commandInfo.SetCommandArguments(args);
            _uciCommandQueue.Enqueue(commandInfo);

        }

        private bool GetIsProcessRunning()
        {
            return _process != null;
        }

        public ReceiveOutput _recieveOutput;


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

        public Engine(string description, string command, string[] uciArguments, ReceiveOutput recieveOutput = null, Guid? id = null, ProcessPriorityClass priority = ProcessPriorityClass.Normal)
        {
            _uid = id ?? Guid.NewGuid();
            Description = description;
            Command = command;
            UciArguments = uciArguments;
            Priority = priority;
            _recieveOutput = recieveOutput;

        }

        public async Task StartAsync()
        {
            EngineInformation = new UCIEngineInformation();
            using (_process = new Process())
            {
                await Task.Run(() => { ExecuteEngine(null); });
            }
        }

        private void OnUCIResponseRecieved(object sender, DataReceivedEventArgs e)
        {

            string engineResponse = e.Data;

            if (string.IsNullOrEmpty(engineResponse))
            {
                return;
            }

            if (_currentCommand != null)
            {
                var command = _currentCommand;
                if (command.AwaitResponse)
                {
                    _sbUciResponse.AppendLine(engineResponse);
                    if (command.CommandSent == AppToUCICommand.UCI && command.IsResponseTheExpectedResponse(engineResponse))
                    {
                        FinalizeUCIOkResponse(command);
                    }
                    else if (command.CommandSent == AppToUCICommand.IsReady && command.IsResponseTheExpectedResponse(engineResponse))
                    {
                        FinalizeIsReadyResponse(command);
                    }
                    return;
                }
                else
                {
                    switch (command.CommandSent)
                    {
                        case AppToUCICommand.Stop:
                            FinalizeStopResponse(command, engineResponse);
                            return;
                        case AppToUCICommand.Go:
                            if (command.IsResponseTheExpectedResponse(engineResponse))
                            {
                                var moves = MakeBestMoveArrayFromUCI(engineResponse);
                                //Console.WriteLine($"Best: {moves[0].()}");
                            }
                            break;
                        default: break;

                    }
                    _recieveOutput(Id, Description, engineResponse);
                }


            }

        }

        private void CleanupAwaitedResponseFields()
        {
            var optionsInfoString = _sbUciResponse.ToString();
            _currentCommand?.OnCommandFinished(Id, EngineInformation, optionsInfoString);
            _recieveOutput(Id, Description, _sbUciResponse.ToString());
            _sbUciResponse.Clear();
            _currentCommand = null;
        }

        private void FinalizeUCIOkResponse(UCICommandInfo command)
        {
            EngineInformation = new UCIEngineInformation(_sbUciResponse.ToString());
            CleanupAwaitedResponseFields();
        }

        private void FinalizeIsReadyResponse(UCICommandInfo command)
        {
            CleanupAwaitedResponseFields();
        }

        private void FinalizeStopResponse(UCICommandInfo command, string engineResponse)
        {
            var moveArray = (MoveExt[])null;
            if (command.OnCommandFinished != null)
            {
                moveArray = MakeBestMoveArrayFromUCI(engineResponse);
            }
            _currentCommand?.OnCommandFinished(Id, moveArray, engineResponse);
            _recieveOutput(Id, Description, engineResponse);
            _sbUciResponse.Clear();
            _currentCommand = null;
        }

        private static MoveExt[] MakeBestMoveArrayFromUCI(string engineResponse)
        {
            var strBestMove = engineResponse.GetValueForUCIKeyValuePair(UCIToAppCommand.BestMove.AsString(EnumFormat.Description));
            var strPonder = engineResponse.GetValueForUCIKeyValuePair(UCIToAppCommand.Ponder.AsString(EnumFormat.Description));
            return new[] { MakeMoveFromUCIMove(strBestMove), MakeMoveFromUCIMove(strPonder) };
        }

        private static MoveExt MakeMoveFromUCIMove(string uciMove)
        {
            if (string.IsNullOrWhiteSpace(uciMove))
            {
                return null;
            }
            return MoveHelpers.GenerateMove(uciMove.Substring(0, 2).SquareTextToIndex().Value, uciMove.Substring(2, 2).SquareTextToIndex().Value);
        }

        private void ExecuteEngine(object state)
        {
            StartEngineProcess();
            this.SendUCI();
            var exit = false;
            int waitResult = WaitHandle.WaitTimeout;
            var commandEvents = new[] { UCICommandIssued, UCICommandQueue.InterruptIssued };
            int InterruptHandle = 1;
            StreamWriter engineInputStream = _process.StandardInput;
            while (!_process.HasExited && !exit)
            {
                if (!_uciCommandQueue.Any())
                {
                    waitResult = WaitHandle.WaitAny(commandEvents);
                }

                if (_uciCommandQueue.TryPeek(out UCICommandInfo queuedCommand))
                {
                    if (waitResult == InterruptHandle)
                    {
                        engineInputStream.WriteToEngine(queuedCommand);
                        exit = queuedCommand.CommandSent == AppToUCICommand.Quit;
                    }
                    else
                    {
                        if (_currentCommand != null && _currentCommand.AwaitResponse)
                        {

                        }

                        else
                        {
                            if (_uciCommandQueue.TryDequeue(out UCICommandInfo commandToIssue))
                            {
                                _currentCommand = commandToIssue;

                                engineInputStream.WriteLine(commandToIssue.GetFullCommand());
                                engineInputStream.Flush();
                            }
                        }
                    }
                }
            }
            _process.WaitForExit(1000);
        }

        private void StartEngineProcess()
        {
            _process.StartInfo = startInfo;
            _process.StartInfo.FileName = Command;
            _process.OutputDataReceived += OnUCIResponseRecieved;
            _process.Start();
            _process.BeginErrorReadLine();
            _process.BeginOutputReadLine();
            _process.PriorityClass = _priority;
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
