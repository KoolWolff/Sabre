using System;
using System.IO;

namespace Sabre
{
    class Logger
    {
        public string Time { get; set; }
        public Logger(string time)
        {
            Time = time;
            Directory.CreateDirectory("Logs");
            var f = File.Create(Environment.CurrentDirectory + "\\Logs\\" + "log - " + Time + ".txt");
            f.Dispose();
            f.Close();
            GC.Collect();
        }
        public void Write(string log, WriterType typeOfLog)
        {
            if (typeOfLog == WriterType.WriteMessage)
            {
                WriteMessage(log);
            }
            if (typeOfLog == WriterType.WriteError)
            {
                WriteError(log);
            }
            if (typeOfLog == WriterType.WriteWarning)
            {
                WriteWarning(log);
            }
            if (typeOfLog == WriterType.WriteCrash)
            {
                WriteCrash(log);
            }
        }
        private void WriteMessage(string message)
        {
            File.AppendAllText(Environment.CurrentDirectory + "\\Logs\\" + "log - " + Time + ".txt", "MESSAGE | "
                + DateTime.Now.ToString("HH-mm-ss")
                + " | " + message + Environment.NewLine);
            GC.Collect();
        }
        private void WriteError(string error)
        {
            File.AppendAllText(Environment.CurrentDirectory + "\\Logs\\" + "log - " + Time + ".txt", "ERROR   | "
                + DateTime.Now.ToString("HH-mm-ss") + " | "
                + error + Environment.NewLine);
            GC.Collect();
        }
        private void WriteWarning(string warning)
        {
            File.AppendAllText(Environment.CurrentDirectory + "\\Logs\\" + "log - " + Time + ".txt", "WARNING | "
                + DateTime.Now.ToString("HH-mm-ss") + " | "
                + warning + Environment.NewLine);
            GC.Collect();
        }
        private void WriteCrash(string crash)
        {
            File.AppendAllText(Environment.CurrentDirectory + "\\Logs\\" + "log - " + Time + ".txt", "CRASH   | "
                + DateTime.Now.ToString("HH-mm-ss") + " | "
                + crash + Environment.NewLine);
            GC.Collect();
        }
        public void DeleteLogs()
        {
            string[] logs = Directory.GetFiles(Environment.CurrentDirectory + "\\Logs\\");
            foreach (string s in logs)
            {
                File.Delete(s);
            }
        }
        public enum WriterType
        {
            WriteMessage,
            WriteError,
            WriteWarning,
            WriteCrash
        }
    }
}
