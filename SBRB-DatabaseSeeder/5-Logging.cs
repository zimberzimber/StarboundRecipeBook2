using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SBRB_DatabaseSeeder
{
    partial class Program
    {
        static List<string> _warningMessages = new List<string>();
        static bool silentLogging = false;
        static FileStream logFile;

        static void Log(string message)
        {
            WriteToFile(message);
            if (!silentLogging)
                Console.WriteLine(message);
        }

        static void Log()
            => Log("\n");

        static void Log(string message, params object[] args)
            => Log(string.Format(message, args));

        static void WriteToFile(string message)
        {
            byte[] buffer = new UTF8Encoding(true).GetBytes($"\n{message}");
            logFile.Write(buffer, 0, buffer.Length);
        }

        static void AddWarning(string warning)
              => _warningMessages.Add($"WARNING - {warning}");
    }
}
