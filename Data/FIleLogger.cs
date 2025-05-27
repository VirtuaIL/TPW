using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;

namespace TP.ConcurrentProgramming.Data
{
    public static class FileLogger
    {
        private static readonly string LogFilePath = "simulation_log.txt";
        private static readonly ConcurrentQueue<string> _logQueue = new ConcurrentQueue<string>();
        private static readonly Thread _loggerThread;
        private static volatile bool _isRunning = true;
        private static readonly object _fileWriteLock = new object();

        static FileLogger()
        {
            try
            {
                lock (_fileWriteLock)
                {
                    File.WriteAllText(LogFilePath, $"Log Started: {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}{Environment.NewLine}", Encoding.ASCII);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error] Failed to initialize log file '{LogFilePath}': {ex.Message}");
            }

            _loggerThread = new Thread(ProcessLogQueueLoop)
            {
                IsBackground = true,
                Name = "FileLoggerThread"
            };
            _loggerThread.Start();
        }

        public static void Log(string message)
        {
            if (!_isRunning && _logQueue.IsEmpty) return;
            string logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} | {message}";
            _logQueue.Enqueue(logEntry);
        }

        private static void ProcessLogQueueLoop()
        {
            while (_isRunning || !_logQueue.IsEmpty)
            {
                if (_logQueue.TryDequeue(out string? logEntryToSave))
                {
                    if (logEntryToSave != null)
                    {
                        try
                        {
                            lock (_fileWriteLock)
                            {
                                File.AppendAllText(LogFilePath, logEntryToSave + Environment.NewLine, Encoding.ASCII);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"[Error] Unexpected error writing log: {ex.Message}. Log: \"{logEntryToSave}\"");
                        }
                    }
                }
                else
                {
                    if (_isRunning)
                    {
                        Thread.Sleep(50);
                    }
                }
            }
        }

        public static void Shutdown()
        {
            FileLogger.Log("[FileLogger] Logger shutdown requested...");
            _isRunning = false;

            if (_loggerThread != null && _loggerThread.IsAlive)
            {
                // Czekaj na zakończenie wątku z timeoutem. Dajemy mu czas na przetworzenie pozostałych logów w kolejce
                if (!_loggerThread.Join(TimeSpan.FromSeconds(2)))
                {
                    Console.WriteLine("[FileLogger Warning] Logger thread did not shut down in time.");
                }
            }
        }
    }
}
