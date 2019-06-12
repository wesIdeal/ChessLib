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
        #region Constructors / Descructor
        protected Engine()
        {
            _preProcessQueue.Enqueue(CommandInfo.UCI());
            _preProcessQueue.Enqueue(CommandInfo.IsReady());
        }

        public Engine(string description, string command)
        {
            UserEngineDescription = description;
            CommandLineString = command;
            Priority = ProcessPriorityClass.Normal;
        }
        public Engine(string description, string command, bool ignoreMoveCalculationLines)
            : this(description, command)
        {
            IgnoreMoveCalculationLines = ignoreMoveCalculationLines;
        }
        public Engine(string description, string command, bool ignoreMoveCalculationLines, ProcessPriorityClass priorityClass)
            : this(description, command, ignoreMoveCalculationLines)
        {
            Priority = priorityClass;
        }
        public Engine(string description, string command, bool ignoreMoveCalculationLines = true, ProcessPriorityClass priority = ProcessPriorityClass.Normal, Guid? userEngineId = null)
            : this(description, command, ignoreMoveCalculationLines, priority)
        {
            _userAssignedId = userEngineId ?? Guid.NewGuid();
        }
        public Engine(string description, string command, Dictionary<string, string> uciOptions, bool ignoreMoveCalculationLines = true, ProcessPriorityClass priority = ProcessPriorityClass.Normal, Guid? userEngineId = null)
            : this(description, command, ignoreMoveCalculationLines, priority, userEngineId)
        {

            UciOptions = uciOptions;
        }

        private void ProcessUCIOptions()
        {
            if (UciOptions == null) return;
            foreach (var option in UciOptions)
            {
                var message = "";
                if (!EngineInformation.SupportsOption(option.Key))
                {
                    throw new UCICommandException($"Option passed from command line {option.Key} is not valid.");
                }
                if (EngineInformation.IsOptionValid(option.Key, option.Value, out message))
                {
                    throw new UCICommandException(message);
                }
                WriteToEngine(new SetOption(option.Key, option.Value));
            }

        }
        #endregion


        #region Properties
        public Guid UserAssignedId => _userAssignedId;
        protected Guid _userAssignedId;

        /// <summary>
        /// Initial options to set on uci engine
        /// </summary>
        public Dictionary<string, string> UciOptions { get; private set; }

        /// <summary>
        /// Represents whether single move calculation lines should be sent to the client.
        /// </summary>
        public bool IgnoreMoveCalculationLines { get; set; }
        /// <summary>
        /// Command line path + additional arguemtns
        /// </summary>
        public string CommandLineString { get; private set; }

        /// <summary>
        /// Engine description supplied by user
        /// </summary>
        public string UserEngineDescription { get; set; }

        public ProcessPriorityClass Priority
        {
            get => _priority;
            private set => _priority = value;
        }
        #endregion

        private ProcessPriorityClass _priority;

        #region Non-Serialized Fields/Properties
        /// <summary>
        /// Gets the isready value from engine. This should only be false while processing the isready command and waiting for readyok.
        /// </summary>
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

        /// <summary>
        /// Represents whether UCI information was sent back to the engine. 
        /// </summary>
        public bool UCIOk
        {
            get { return _uciOk; }
            protected set
            {
                _uciOk = value;
                if (value)
                {
                    OnEngineStateChanged(new StateChangeEventArgs(StateChangeField.UCIOk, value));
                }
            }
        }

        /// <summary>
        /// Represents whether engine is currently analyzing a position
        /// </summary>
        public bool IsAnalyizing
        {
            get => _isAnalyizing;
            private set
            {
                _isAnalyizing = value;
                OnEngineStateChanged(new StateChangeEventArgs(StateChangeField.IsCalculating, value));
            }
        }

        /// <summary>
        /// Name and Id from uci command
        /// </summary>
        public string UCIName => $"{EngineInformation.Name} {EngineInformation.Id}";

        [NonSerialized] private StringBuilder _errorBuilder = new StringBuilder();

        [NonSerialized] private TaskCompletionSource<bool> errorCloseEvent = new TaskCompletionSource<bool>();
        [NonSerialized] private TaskCompletionSource<bool> outputCloseEvent = new TaskCompletionSource<bool>();

        [NonSerialized] private string _fen;
        [NonSerialized] private bool _uciOk = false;
        [NonSerialized] private bool _isReady = true;
        [NonSerialized] private bool _isAnalyizing = true;
        [NonSerialized] private bool _optionsSet = false;

        [NonSerialized] private StringBuilder _sbUciResponse = new StringBuilder();
        [NonSerialized] public UCIEngineInformation EngineInformation = new UCIEngineInformation();
        [NonSerialized] private CommandQueue _uciCommandQueue = new CommandQueue();
        [NonSerialized] private CommandInfo _currentCommand;
        [NonSerialized] private CommandQueue _preProcessQueue = new CommandQueue();
        [NonSerialized] private readonly AutoResetEvent ReadyOkReceived = new AutoResetEvent(false);
        [NonSerialized] private readonly AutoResetEvent UCIInfoReceived = new AutoResetEvent(false);

        #endregion

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


        public async Task<EngineProcessResult> StartAsync()
        {
            EngineInformation = new UCIEngineInformation();
            var result = new EngineProcessResult();

            using (_process = new Process())
            {
                _process.ErrorDataReceived += OnErrorDataReceived;
                StartEngineProcess();
                ExecuteEngine();
                var waitForExit = WaitForExitAsync();
                // Create task to wait for process exit and closing all output streams
                var processTask = Task.WhenAll(waitForExit, outputCloseEvent.Task, errorCloseEvent.Task);

                // Waits process completion and then checks it was not completed by timeout
                await Task.WhenAny(processTask)
                    .ContinueWith((a) =>
                        {
                            result.Completed = true;
                            result.ExitCode = _process.ExitCode;

                            if (_process.ExitCode != 0)
                            {
                                var message = $"Exited with code {_process.ExitCode}";
                                var dbMessage = new DebugEventArgs(message);
                                OnDebugEventExecuted(dbMessage);

                            }
                            else
                            {
                                OnDebugEventExecuted(new DebugEventArgs($"Process exited normally."));
                            }
                        });
            }
            return result;
        }



        private Task WaitForExitAsync()
        {
            return Task.Run(() => _process.WaitForExit());
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
                var engineInformation = new UCIEngineInformation(UserAssignedId, _sbUciResponse.ToString());
                _sbUciResponse.Clear();
                responseObject = engineInformation;
                UCIInfoReceived.Set();
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

        private void ExecuteEngine()
        {
            var exit = false;
            _process.BeginErrorReadLine();
            _process.BeginOutputReadLine();
            int waitResult = WaitHandle.WaitTimeout;
            int InterruptHandle = 1;

            WriteToEngine(new CommandInfo(AppToUCICommand.UCI));
            WaitHandle.WaitAll(new[] { UCIInfoReceived }, 5 * 1000);
            WriteToEngine(new CommandInfo(AppToUCICommand.IsReady));
            var handle = WaitHandle.WaitAll(new[] { ReadyOkReceived }, (int)10 * 1000);
            ProcessUCIOptions();
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
            var exitTimeout = 5 * 1000;
            if (!_process.WaitForExit(exitTimeout))
            {
                OnDebugEventExecuted(new DebugEventArgs($"Engine process didn't shutdown in {exitTimeout}ms. Killing process."));
                _process.Kill();
                OnDebugEventExecuted(new DebugEventArgs($"Engine process killed."));
            }
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
            _process.StartInfo.FileName = CommandLineString;
            _process.EnableRaisingEvents = true;
            _process.OutputDataReceived += OnUCIResponseRecieved;
            _process.Exited += OnProcessExited;
            _process.Start();
            OnDebugEventExecuted(new DebugEventArgs($"Process started. PID {_process.Id}."));
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
    public struct EngineProcessResult
    {
        public bool Completed;
        public int? ExitCode;
        public string Output;
    }
}
