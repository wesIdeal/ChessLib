using EnumsNET;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace ChessLib.UCI
{

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

        public int AddEngine(string description, string command, string[] uciArguments, Guid? id = null, ProcessPriorityClass priority = ProcessPriorityClass.Normal)
        {
            var engine = new Engine(description, command, uciArguments, id, priority);
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
