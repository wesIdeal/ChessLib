﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace ChessLib.EngineInterface
{

    public class EngineRunner : IDisposable
    {
        public EngineRunner()
        {
            Engines = new List<Engine>();
        }

        public List<Engine> Engines { get; }

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

        //private UCIEngine engine;


        //public int AddEngine(string description, string command, Guid id, bool ignoreMoveCalcLines = true, ProcessPriorityClass priority = ProcessPriorityClass.Normal)
        //{
        //    engine = new UCIEngine(id, description, command, ignoreMoveCalcLines, priority);
        //    Engines.Add(engine);
        //    return Engines.Count - 1;
        //}

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
