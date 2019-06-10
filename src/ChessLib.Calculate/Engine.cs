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
using System.Collections.Generic;
using ChessLib.UCI.Commands;
using ChessLib.UCI.Commands.ToEngine;
using ChessLib.UCI.Commands.FromEngine;

namespace ChessLib.UCI
{

    [Serializable]
    public class Engine : IDisposable
    {
        public Guid Id => _uid;
        private Guid _uid;
        public bool IsReady { get; set; }
        public bool UCIOkFinished = true;
        public string Description { get; set; }
        public string Command { get; private set; }
        public string[] UciArguments { get; private set; }

        public Engine(string description, string command, string[] uciArguments, Guid? id = null, ProcessPriorityClass priority = ProcessPriorityClass.Normal)
        {
            _uid = id ?? Guid.NewGuid();
            Description = description;
            Command = command;
            UciArguments = uciArguments;
            Priority = priority;
        }

        [NonSerialized]
        public string _fen;

        public bool IsAnalyizing { get; private set; }


        private AutoResetEvent UCIResponseRecieved = new AutoResetEvent(false);
        private StringBuilder _sbUciResponse = new StringBuilder();
        [NonSerialized]
        public UCIEngineInformation EngineInformation;
        [NonSerialized]
        private ConcurrentQueue<CommandInfo> _uciCommandQueue = new ConcurrentQueue<CommandInfo>();
        private CommandInfo _currentCommand;
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

        public void QueueCommand(CommandInfo commandInfo, params string[] args)
        {
            if (!GetIsProcessRunning())
            {
                throw new NullReferenceException("Process must be started before sending command.");
            }
            Debug.WriteLine($"SENDING\t{commandInfo.CommandText}");
            commandInfo.SetCommandArguments(args);
            _uciCommandQueue.Enqueue(commandInfo);
            CommandQueue.CommandIssued.Set();

        }

        private bool GetIsProcessRunning()
        {
            return _process != null;
        }

        private event EventHandler<EngineResponseArgs> _responseReceived;
        /// <summary>
        /// Receives the important information from the engine with relevant object
        /// </summary>
        public event EventHandler<EngineResponseArgs> ResponseReceived
        {
            add
            {
                _responseReceived -= value;
                _responseReceived += value;
            }
            remove { _responseReceived -= value; }
        }


        private event EventHandler<CommandStringArgs> _commandTransferred;
        /// <summary>
        /// Fired each time the app communicates with the engine or a response is received.
        /// Contains raw text responses and commands.
        /// </summary>
        public event EventHandler<CommandStringArgs> CommandTransferred
        {
            add
            {
                _commandTransferred -= value;
                _commandTransferred += value;
            }
            remove
            {
                _commandTransferred -= value;
            }
        }

        private event EventHandler<DebugArgs> _debugEventExecuted;
        public event EventHandler<DebugArgs> DebugEventExecuted
        {
            add
            {
                _debugEventExecuted -= value;
                _debugEventExecuted += value;
            }
            remove
            {
                _debugEventExecuted -= value;
            }
        }


        [NonSerialized]
        public Process _process;

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
            EngineToAppCommand responseType;
            OnEngineTextReceived(engineResponse);
            if (string.IsNullOrEmpty(engineResponse))
            {
                return;
            }
            EngineToAppCommand matchingFlag = EngineToAppCommand.None;
            if (_currentCommand != null)
            {
                var command = _currentCommand;
                IEngineResponse response = null;
                if (command.AwaitResponse && command.IsResponseTheExpectedResponse(engineResponse, out matchingFlag))
                {
                    if (command.CommandSent == AppToUCICommand.UCI)
                    {
                        UCIOkFinished = false;
                        ProcessUCIResponse(command, engineResponse, matchingFlag, out response);
                    }
                    else if (matchingFlag == EngineToAppCommand.Ready)
                    {
                        FinalizeIsReadyResponse(command, engineResponse, out response);
                    }
                    else if (matchingFlag == EngineToAppCommand.Info)
                    {
                        var ignore = (command as Go).IgnoreCalculationInformationLines;
                        ProcessInfoResponse(engineResponse, matchingFlag, ignore, out response);
                    }
                    else if (matchingFlag == EngineToAppCommand.BestMove)
                    {
                        ProcessBestMoveResponse(engineResponse, out response);
                    }
                    return;
                }

                OnResponseReceived(new EngineResponseArgs(engineResponse, _currentCommand.CommandSent, matchingFlag, response));
            }
           

        }

        private void OnDebugEventExecuted(string txt)
        {
            var time = DateTime.Now.TimeOfDay;
            txt = time + ": " + txt;
            _debugEventExecuted?.Invoke(this, new DebugArgs(txt));
            Debug.WriteLine(txt);
        }

        private void OnCommandSent(string commandText)
        {
            var command = new CommandStringArgs(CommandStringArgs.TextSource.UI, commandText);
            _commandTransferred?.Invoke(this, command);
            _debugEventExecuted?.Invoke(this, new DebugArgs(command.ToString()));
        }

        private void OnEngineTextReceived(string engineResponse)
        {
            var receivedTxt = new CommandStringArgs(CommandStringArgs.TextSource.Engine, engineResponse);
            _commandTransferred?.Invoke(this, receivedTxt);
            _debugEventExecuted?.Invoke(this, new DebugArgs(receivedTxt.ToString()));
        }

        protected void OnResponseReceived(EngineResponseArgs eventArgs)
        {
            if (UCIOkFinished)
            {
                _responseReceived?.Invoke(EngineInformation, eventArgs);
            }
        }

        private void ProcessBestMoveResponse(string engineResponse, out IEngineResponse bestMoveResponse)
        {
            var moves = MakeBestMoveArrayFromUCI(engineResponse, out string variationDisplay);
            var bestMove = moves != null && moves.Count() > 0 ? moves[0] : null;
            var ponderMove = moves != null && moves.Count() > 1 ? moves[1] : null;
            IsAnalyizing = false;
            bestMoveResponse = new BestMoveResponse(bestMove, ponderMove);
            CleanupAwaitedResponseFields();
        }
        private void ProcessInfoResponse(string engineResponse, EngineToAppCommand responseCommand, bool ignoreCalculationOfSingleMoves, out IEngineResponse response)
        {
            response = null;
            if (ignoreCalculationOfSingleMoves && InfoResponse.GetTypeOfInfo(engineResponse) == InfoResponse.InfoTypes.CalculationInfo)
            {
                return;
            }
            response = new InfoResponse(_fen, engineResponse);
        }

        private void CleanupAwaitedResponseFields()
        {
            _sbUciResponse.Clear();
            _currentCommand = null;
        }

        private void ProcessUCIResponse(CommandInfo command, string engineResponse, EngineToAppCommand commandResponse, out IEngineResponse responseObject)
        {

            if (commandResponse == EngineToAppCommand.UCIOk)
            {
                var engineInformation = new UCIEngineInformation(Id, _sbUciResponse.ToString());
                CleanupAwaitedResponseFields();
                UCIOkFinished = true;
                responseObject = engineInformation;
            }
            else
            {
                responseObject = null;
                _sbUciResponse.AppendLine(engineResponse);
            }
        }

        private void FinalizeIsReadyResponse(CommandInfo command, string engineResponse, out IEngineResponse response)
        {
            response = new ReadyOk(engineResponse);
            CleanupAwaitedResponseFields();
        }

        private void FinalizeStopResponse(CommandInfo command, string engineResponse)
        {
            CleanupAwaitedResponseFields();
        }

        private MoveExt[] MakeBestMoveArrayFromUCI(string engineResponse, out string sanMoveDisplay)
        {
            var strBestMove = engineResponse.GetValueForUCIKeyValuePair(EngineToAppCommand.BestMove.AsString(EnumFormat.Description));
            var strPonder = engineResponse.GetValueForUCIKeyValuePair(EngineToAppCommand.Ponder.AsString(EnumFormat.Description));
            var rv = InfoResponse.FillMoves(_fen, new[] { strBestMove, strPonder }, out List<string> moveDisplay).ToArray();
            sanMoveDisplay = string.Join(" ", moveDisplay);
            return rv;
        }

        private void ExecuteEngine(object state)
        {
            StartEngineProcess();
            var exit = false;
            int waitResult = WaitHandle.WaitTimeout;

            int InterruptHandle = 1;

            while (!_process.HasExited && !exit)
            {
                if (!_uciCommandQueue.Any())
                {
                    OnDebugEventExecuted("Waiting for command...");
                    waitResult = WaitHandle.WaitAny(CommandQueue.CommandIssuedEvents);
                }

                if (_uciCommandQueue.TryPeek(out CommandInfo commandToIssue))
                {
                    _currentCommand = commandToIssue;
                    if (waitResult == InterruptHandle)
                    {
                        OnDebugEventExecuted("Received interrupt command in queue - " + commandToIssue.CommandText);
                        WriteToEngine(commandToIssue);
                        exit = commandToIssue.CommandSent == AppToUCICommand.Quit;

                    }
                    else if (!IsAnalyizing || !UCIOkFinished)
                    {
                        OnDebugEventExecuted("Received command in queue - " + commandToIssue.CommandText);
                        WriteToEngine(commandToIssue);
                    }
                }
            }
            OnDebugEventExecuted("Waiting to exit.");
            _process.WaitForExit(1000);
        }

        public void WriteToEngine(CommandInfo command)
        {
            _process.StandardInput.WriteLine(command.ToString());
            _process.StandardInput.Flush();
            OnCommandSent(command.CommandText);
        }

        private void StartEngineProcess()
        {
            _process.StartInfo = startInfo;
            _process.StartInfo.FileName = Command;
            _process.OutputDataReceived += OnUCIResponseRecieved;
            _process.Start();
            OnDebugEventExecuted($"Process started. PID {_process.Id}.");
            _process.BeginErrorReadLine();
            _process.BeginOutputReadLine();
            _process.PriorityClass = _priority;
            _process.Exited += (sender, obj) =>
            {
                OnDebugEventExecuted("Process exited.");
            };
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
