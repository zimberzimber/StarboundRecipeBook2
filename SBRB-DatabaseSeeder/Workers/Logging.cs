using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SBRB_DatabaseSeeder.Workers
{
    sealed class Logger
    {
        // Value indicating for how long the writing thread should sleep when waiting for a message in the message queue.
        const int LOGGER_WAIT_FOR_MESSAGE_MILISECONDS = 10;

        /// <summary>
        /// Property determining whether the logger should print messages to console or not.
        /// </summary>
        public bool SilentLogging = false;

        /// <summary>
        /// Property indicating whether the logger is done workng.
        /// </summary>
        public bool DoneWroking { get; private set; } = false;

        /// <summary>
        /// Get the singleton instance of the class
        /// </summary>
        public static Logger Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                        _instance = new Logger();
                }
                return _instance;
            }
        }

        // Singleton instance
        static Logger _instance = null;
        static readonly object _lock = new object();

        // Value used to determine whether the log should keep accepting input
        bool _keep_logger = true;

        // List of warning messages. Warnings are not displayed outright, but are stored, and can be displayed later.
        ConcurrentQueue<string> _warningMessages;

        // Thread safe queue containing messages to be logged
        ConcurrentQueue<string> _messageQueque;

        // Reference to the stream writer
        StreamWriter _logFile;


        // Private constructor for singleton class
        Logger()
        {
            // Create a file to contain the logged messages if it doesn't already exist
            Directory.CreateDirectory("logs");

            // Create the log file
            _logFile = new StreamWriter("logs\\" + DateTime.Now.ToString("yyyy-MM-dd HH.mm.ss.ff") + ".txt");
            _logFile.AutoFlush = true;

            // Create the message queue for async log writing
            _messageQueque = new ConcurrentQueue<string>();

            // Create the warnings list
            _warningMessages = new ConcurrentQueue<string>();

            // Start the writer loop on a separate thread
            Task.Factory.StartNew(WriterLoop, TaskCreationOptions.LongRunning).ConfigureAwait(false);
        }


        /// <summary>
        /// Tells the logger there will be no more messages to log.
        /// </summary>
        public void Stop_logger()
            => _keep_logger = false;


        /// <summary>
        /// Log and print a formatted message
        /// </summary>
        /// <param name="message">Format string</param>
        /// <param name="args">Formatting arguements</param>
        public void Log(string message, params object[] args)
            => Log(string.Format(message, args));

        /// <summary>
        /// Log and print a message. Leave message empty for new line.
        /// </summary>
        /// <param name="message"></param>
        public void Log(string message = "")
        {
            // Do nothing if the logger doesn't accept new messages any more.
            if (!_keep_logger) return;

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
        public void AddWarning(string warning)
        {
            if (!DoneWroking)
                _warningMessages.Enqueue($"WARNING - {warning}");
        }

        /// <summary>
        /// Add a formatted warning to be displayed at a later time. Use PrintWarnings to log them, and clear the warnings list.
        /// </summary>
        /// <param name="warning">Warning message</param>
        /// <param name="args">Formatting arguements</param>
        public void AddWarning(string warning, params object[] args)
              => AddWarning(string.Format(warning, args));

        /// <summary>
        /// Log warnings added via the AddWarning method. Returns a boolean telling whether there were warnings at all.
        /// </summary>
        /// <param name="clearWarnings">Whether the list should be cleared after the warnings were logged.</param>
        /// <returns>Whether there were any warnings at all.</returns>
        public bool PrintWarnings()
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
        /// Method that keeps writing messages from within the message queue. Stops when _keep_logger is false, and the queue is empty.
        /// Closes the stream and marks the logger as finished working.
        /// </summary>
        void WriterLoop()
        {
            // To INFINITY and B E Y O N D !
            while (true)
            {
                if (_messageQueque.Count == 0)
                {
                    // Sleep for 'LOGGER_WAIT_FOR_MESSAGE_MILISECONDS' if the logger should keep working but there are no messages to log.
                    if (_keep_logger)
                        Thread.Sleep(LOGGER_WAIT_FOR_MESSAGE_MILISECONDS);

                    // Dispose of the stream writer, and set the logger as done working if its done _logger and there are no more messages in the queue
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
