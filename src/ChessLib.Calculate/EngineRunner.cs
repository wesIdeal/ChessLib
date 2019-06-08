using EnumsNET;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace ChessLib.UCI
{
    public interface IUCIOption
    {
        string Name { get; set; }
    }

    public class UCIComboOption : IUCIOption
    {
        public string Name { get; set; }
        public string Default { get; set; }
        public string[] Options { get; set; }

    }

    public class UCISpinOption : IUCIOption
    {
        public string Name { get; set; }
        public double? Min { get; internal set; }
        public double? Default { get; internal set; }
        public double? Max { get; internal set; }
    }

    public class UCIButtonOption : IUCIOption
    {
        public string Name { get; set; }
    }

    public class UCICheckOption : IUCIOption
    {
        public string Name { get; set; }
        public bool Default { get; set; }
    }

    public class UCIStringOption : IUCIOption
    {
        public string Name { get; set; }
        public string Default { get; set; }
    }

    public class EngineRunner : IDisposable
    {
        public EngineRunner()
        {
            Engines = new List<Engine>();
        }

        public List<Engine> Engines { get; private set; }

        public void SaveEngineConfig(string engineConfigFilePath)
        {
            var formatter = new BinaryFormatter();
            using (var stream = new FileStream(engineConfigFilePath, FileMode.OpenOrCreate, FileAccess.Write,
                FileShare.None))
            {
                formatter.Serialize(stream, Engines);
                stream.Close();
            }
        }

        public void LoadEngineConfig(string engineConfigFilePath)
        {
            var formatter = new BinaryFormatter();
            using (var stream = new FileStream(engineConfigFilePath, FileMode.Open, FileAccess.Read,
                FileShare.Read))
            {
                var engines = (List<Engine>)formatter.Deserialize(stream);
                stream.Close();
            }
        }

        public int AddEngine(string description, string command, string[] uciArguments, ReceiveOutput recieveOutputEventHandler = null, OnUCIInfoReceived engineInfoReceived = null, Guid? id = null, ProcessPriorityClass priority = ProcessPriorityClass.Normal)
        {
            ReceiveOutput outputEventHandler = _receiveOutput;
            outputEventHandler += recieveOutputEventHandler;
            var engine = new Engine(description, command, uciArguments, outputEventHandler, engineInfoReceived, id, priority);
            Engines.Add(engine);
            return Engines.Count - 1;
        }

        private void _receiveOutput(Guid engineId, string engineName, string strOutput)
        {
            Debug.WriteLine($"<{engineName}({engineId}) Output>");
            Debug.WriteLine($"\t{strOutput}");
            Debug.WriteLine($"</{engineName}({engineId}) Output>");
        }

        public void Dispose()
        {
            foreach (var eng in Engines)
            {
                eng.Dispose();
            }
        }
    }
}
