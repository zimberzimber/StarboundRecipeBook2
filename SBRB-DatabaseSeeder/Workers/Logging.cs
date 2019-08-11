using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace SBRB_DatabaseSeeder.Workers
{
    static class Logging
    {
        public static bool SilentLogging = false;
        public static bool DoneWroking { get; private set; } = false;
        static bool _keepLogging = true;
        static List<string> _warningMessages;
        static FileStream _logFile;
        static ConcurrentQueue<string> _messageQueque;

        static Logging()
        {
            // Create a file to contain the logged messages
            Directory.CreateDirectory("logs");

            // Create the log file
            _logFile = File.Create("logs\\" + DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss") + ".txt");

            // Create the message queue for async log writing
            _messageQueque = new ConcurrentQueue<string>();

            // Create the warnings list
            _warningMessages = new List<string>();

            // Start the writer loop on a separate thread
            ThreadPool.QueueUserWorkItem(delegate { WriterLoop(); });
        }



        /// <summary>
        /// Tells the logger there will be no more messages to log.
        /// </summary>
        public static void StopLogging()
            => _keepLogging = false;



        /// <summary>
        /// Log and print a new line
        /// </summary>
        public static void Log()
            => Log("\n");

        /// <summary>
        /// Log and print a formatted message
        /// </summary>
        /// <param name="message">Format string</param>
        /// <param name="args">Formatting arguements</param>
        public static void Log(string message, params object[] args)
            => Log(string.Format(message, args));

        /// <summary>
        /// Log and print a message
        /// </summary>
        /// <param name="message"></param>
        public static void Log(string message)
        {
            if (!_keepLogging) return;

            _messageQueque.Enqueue(message);
            if (!SilentLogging)
                Console.WriteLine(message);
        }



        /// <summary>
        /// Add a warning to be displayed at a later time. Use PrintWarnings to log them, and clear the warnings list.
        /// </summary>
        /// <param name="warning">Warning message</param>
        public static void AddWarning(string warning)
        {
            if (DoneWroking)
                _warningMessages.Add($"WARNING - {warning}");
        }

        /// <summary>
        /// Add a formatted warning to be displayed at a later time. Use PrintWarnings to log them, and clear the warnings list.
        /// </summary>
        /// <param name="warning">Warning message</param>
        /// <param name="args">Formatting arguements</param>
        public static void AddWarning(string warning, params object[] args)
              => AddWarning(string.Format(warning, args));

        /// <summary>
        /// Log warnings added via the AddWarning method. Returns a boolean telling whether there were warnings at all.
        /// </summary>
        /// <param name="clearWarnings">Whether the list should be cleared after the warnings were logged.</param>
        /// <returns>Whether there were any warnings at all.</returns>
        public static bool PrintWarnings(bool clearWarnings = false)
        {
            if (_warningMessages.Count == 0)
                return false;

            for (int i = 0; i < _warningMessages.Count; i++)
                Log(_warningMessages[i]);

            if (clearWarnings)
                ClearWarnings();

            return true;
        }

        /// <summary>
        /// Clear the warnings log
        /// </summary>
        public static void ClearWarnings()
            => _warningMessages = new List<string>();



        /// <summary>
        /// Method that keeps writing messages from within the message queue. Stops when _keepLogging is false, and the queue is empty.
        /// Closes the stream and marks the logger as finished working.
        /// </summary>
        static void WriterLoop()
        {
            while (true)
            {
                if (_messageQueque.Count == 0)
                {
                    if (_keepLogging)
                        Thread.Sleep(10);
                    else
                    {
                        _logFile.Dispose();
                        DoneWroking = true;
                        return;
                    }
                }

                bool dequeued = _messageQueque.TryDequeue(out string message);
                if (dequeued)
                    WriteToFile(message);
            }
        }

        /// <summary>
        /// Writes the logged messages into the log file.
        /// </summary>
        /// <param name="message">The message to write into the file</param>
        static void WriteToFile(string message)
        {
            byte[] buffer = new UTF8Encoding(true).GetBytes($"\n{message}");
            _logFile.Write(buffer, 0, buffer.Length);
            _logFile.Flush();
        }
    }
}
