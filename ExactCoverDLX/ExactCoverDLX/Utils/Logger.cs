using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace ExactCoverDLX.Utils
{
    public static class Logger
    {
        private static StreamWriter _logger;
        private static string _logPath;
        public static bool DoLog { get; set; }
        public static bool DoAppend { get; set; }

        public static void initialize(string logPath)
        {
            Destroy();

            _logPath = logPath;
            if (DoAppend) _logger = File.AppendText(_logPath);
            else _logger = File.CreateText(_logPath);
        }

        public static void Destroy()
        {
            if (_logger != null)
            {
                _logger.Flush();
                _logger.Close();
            }
            _logger = null;
            _logPath = null;
        }

        public static void Log(String msg)
        {
            if (!DoLog) return;
            if(_logger == null) initialize("execution.log");
            _logger.WriteLine(msg);
        }

    }
}
