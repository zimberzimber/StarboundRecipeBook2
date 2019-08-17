using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;

namespace SBRB_DatabaseSeeder.Workers
{
    static class Logging
    {
        // Value indicating for how long the writing thread should sleep when waiting for a message in the message queue.
        const int LOGGER_WAIT_FOR_MESSAGE_MILISECONDS = 10;

        /// <summary>
        /// Property determining whether the logger should print messages to console or not.
        /// </summary>
        public static bool SilentLogging = false;

        /// <summary>
        /// Property indicating whether the logger is done workng.
        /// </summary>
        public static bool DoneWroking { get; private set; } = false;

        // Value used to determine whether the log should keep accepting input
        static bool _keepLogging = true;

        // List of warning messages. Warnings are nit displayed outright, but are stored, and can be displayed later.
        static ConcurrentQueue<string> _warningMessages;

        // Reference to the stream writer
        static StreamWriter _logFile;

        // Thread safe queue containing messages to be logged
        static ConcurrentQueue<string> _messageQueque;

        static Logging()
        {
            // Create a file to contain the logged messages
            Directory.CreateDirectory("logs");

            // Create the log file
            _logFile = new StreamWriter("logs\\" + DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss") + ".txt");
            _logFile.AutoFlush = true;

            // Create the message queue for async log writing
            _messageQueque = new ConcurrentQueue<string>();

            // Create the warnings list
            _warningMessages = new ConcurrentQueue<string>();

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
            // Do nothing if the logger doesn't accept new messages any more.
            if (!_keepLogging) return;

            // Add the message to the queue
            _messageQueque.Enqueue(message);

            // Print the message to console if the logger is not silent
            if (!SilentLogging)
                Console.WriteLine(message);
        }



        /// <summary>
        /// Add a warning to be displayed at a later time. Use PrintWarnings to log them, and clear the warnings list.
        /// </summary>
        /// <param name="warning">Warning message</param>
        public static void AddWarning(string warning)
        {
            if (!DoneWroking)
                _warningMessages.Enqueue($"WARNING - {warning}");
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
        public static bool PrintWarnings()
        {
            // Return false if there are no warnings
            if (_warningMessages.Count == 0)
                return false;

            // Run a loop to queue all warnings to be displayed
            while (true)
            {
                // Break the loop if there are no messages left
                bool hasMessage = _warningMessages.TryDequeue(out string message);
                if (!hasMessage) break;
                Log(message);
            }

            // Return true, indicating there were warnings.
            return true;
        }

        /// <summary>
        /// Method that keeps writing messages from within the message queue. Stops when _keepLogging is false, and the queue is empty.
        /// Closes the stream and marks the logger as finished working.
        /// </summary>
        static void WriterLoop()
        {
            // To INFINITY and B E Y O N D !
            while (true)
            {
                if (_messageQueque.Count == 0)
                {
                    // Sleep for 'LOGGER_WAIT_FOR_MESSAGE_MILISECONDS' if the logger should keep working but there are no messages to log.
                    if (_keepLogging)
                        Thread.Sleep(LOGGER_WAIT_FOR_MESSAGE_MILISECONDS);

                    // Dispose of the stream writer, and set the logger as done working if its done logging and there are no more messages in the queue
                    else
                    {
                        _logFile.Dispose();
                        DoneWroking = true;
                        return;
                    }
                }

                // Check if a message was dequeued, and log it if one did.
                bool dequeued = _messageQueque.TryDequeue(out string message);
                if (dequeued)
                    _logFile.WriteLine(message);
            }
        }
    }
}
