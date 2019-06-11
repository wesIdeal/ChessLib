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
    public partial class Engine : IDisposable
    {
        public Guid Id => _uid;
        private Guid _uid;
        private bool _isReady;
        private bool _uciFinished;
        private bool _isAnalyizing;
        public bool IgnoreMoveCalculationLines { get; set; }

        public bool IsReady
        {
            get { return _isReady; }
            protected set
            {
                _isReady = value;
                if (value)
                {
                    OnEngineStateChanged(new StateChangeEventArgs(StateChangeField.IsReady, value));
                }
            }
        }
        public bool UCIOk
        {
            get { return _uciFinished; }
            protected set
            {
                _uciFinished = value;
                if (value)
                {
                    OnEngineStateChanged(new StateChangeEventArgs(StateChangeField.UCIOk, value));
                }
            }
        }

        public string Description { get; set; }
        public string Command { get; private set; }
        public string[] UciArguments { get; private set; }
        protected Engine() { }
        public Engine(string description, string command, string[] uciArguments, Guid? id = null, bool ignoreMoveCalculationLines = true, ProcessPriorityClass priority = ProcessPriorityClass.Normal)
        {
            _uid = id ?? Guid.NewGuid();
            Description = description;
            Command = command;
            UciArguments = uciArguments;
            Priority = priority;
            IgnoreMoveCalculationLines = ignoreMoveCalculationLines;
        }

        [NonSerialized]
        public string _fen;

        public bool IsAnalyizing
        {
            get => _isAnalyizing;
            private set
            {
                _isAnalyizing = value;
                OnEngineStateChanged(new StateChangeEventArgs(StateChangeField.IsCalculating, value));
            }
        }


        private AutoResetEvent UCIResponseRecieved = new AutoResetEvent(false);
        private StringBuilder _sbUciResponse = new StringBuilder();
        [NonSerialized]
        public UCIEngineInformation EngineInformation;
        [NonSerialized]
        private CommandQueue _uciCommandQueue = new CommandQueue();
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
            OnDebugEventExecuted(new DebugEventArgs($"Adding {commandInfo.CommandText} to queue."));
            commandInfo.SetCommandArguments(args);
            _uciCommandQueue.Enqueue(commandInfo);
        }

        private bool GetIsProcessRunning()
        {
            return _process != null;
        }

        private event EventHandler<EngineInfoArgs> _responseReceived;

        public void SendStop()
        {

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

        public Task _task;
        public async Task StartAsync()
        {
            EngineInformation = new UCIEngineInformation();
            using (_process = new Process())
            {
                await Task.Run(() => { ExecuteEngine(null); });
            }
        }

        private void ProcessBestMoveResponse(string engineResponse, out IEngineResponse bestMoveResponse)
        {
            var moves = MakeBestMoveArrayFromUCI(engineResponse, out string variationDisplay);
            var bestMove = moves != null && moves.Count() > 0 ? moves[0] : null;
            var ponderMove = moves != null && moves.Count() > 1 ? moves[1] : null;
            IsAnalyizing = false;
            bestMoveResponse = new BestMoveResponse(bestMove, ponderMove);
        }
        private void ProcessInfoResponse(string engineResponse, EngineToAppCommand responseCommand, out IEngineResponse response)
        {
            response = null;
            if (IgnoreMoveCalculationLines && InfoResponse.GetTypeOfInfo(engineResponse) == InfoResponse.InfoTypes.CalculationInfo)
            {
                return;
            }
            response = new InfoResponse(_fen, engineResponse);
        }

        private void ProcessUCIResponse(string engineResponse, EngineToAppCommand commandResponse, out IEngineResponse responseObject)
        {
            _sbUciResponse.AppendLine(engineResponse);
            if (commandResponse == EngineToAppCommand.UCIOk)
            {
                var engineInformation = new UCIEngineInformation(Id, _sbUciResponse.ToString());
                _sbUciResponse.Clear();
                responseObject = engineInformation;
            }
            else
            {
                responseObject = null;
            }
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
                    OnDebugEventExecuted(new DebugEventArgs("Waiting for command..."));
                    waitResult = WaitHandle.WaitAny(CommandQueue.CommandIssuedEvents);
                }

                if (_uciCommandQueue.TryPeek(out CommandInfo commandToIssue))
                {
                    _uciCommandQueue.TryDequeue(out _);
                    _currentCommand = commandToIssue;
                    if (_currentCommand.CommandSent == AppToUCICommand.IsReady)
                    {
                        IsReady = false;
                    }
                    else if (_currentCommand.CommandSent == AppToUCICommand.UCI)
                    {
                        UCIOk = false;
                    }
                    if (waitResult == InterruptHandle || EngineHelpers.IsInterruptCommand(commandToIssue.CommandSent))
                    {
                        OnDebugEventExecuted(new DebugEventArgs($"Received interrupt command in queue - {commandToIssue.CommandText}"));
                        WriteToEngine(commandToIssue);
                        exit = commandToIssue.CommandSent == AppToUCICommand.Quit;

                    }
                    else
                    {
                        OnDebugEventExecuted(new DebugEventArgs($"Received command in queue - {commandToIssue.CommandText}"));
                        WriteToEngine(commandToIssue);
                    }
                }
            }
            OnDebugEventExecuted(new DebugEventArgs("Waiting for process to exit."));
            _process.WaitForExit();
        }

        public void WriteToEngine(CommandInfo command)
        {
            _process.StandardInput.WriteLine(command.ToString());
            _process.StandardInput.Flush();
            OnEngineCommunication(new EngineCommunicationArgs(EngineCommunicationArgs.TextSource.UI, command.ToString()));
        }



        private void StartEngineProcess()
        {
            _process.StartInfo = startInfo;
            _process.StartInfo.FileName = Command;
            _process.EnableRaisingEvents = true;
            _process.OutputDataReceived += OnUCIResponseRecieved;
            _process.OutputDataReceived += OnEngineCommunication;
            _process.Exited += OnProcessExited;
            _process.Start();
            OnDebugEventExecuted(new DebugEventArgs($"Process started. PID {_process.Id}."));
            _process.BeginErrorReadLine();
            _process.BeginOutputReadLine();
            _process.PriorityClass = _priority;

        }

        private void OnProcessExited(object sender, EventArgs e)
        {
            OnDebugEventExecuted(new DebugEventArgs($"Process exited."));
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
