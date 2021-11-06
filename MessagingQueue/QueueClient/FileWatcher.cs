using Infrastructure.Models;
using System;
using System.IO;

namespace QueueClient
{
    internal static class FileWatcher
    {
        public static FileSystemWatcher GenerateWatcher(string pathToFolder, FileSender fileSender, Infrastructure.Models.HealthCheckModel checker)
        {
            if (!Directory.Exists(pathToFolder))
            {
                Directory.CreateDirectory(pathToFolder);
            }

            var watcher = new FileSystemWatcher(pathToFolder)
            {
                NotifyFilter = NotifyFilters.Attributes
                                 | NotifyFilters.CreationTime
                                 | NotifyFilters.DirectoryName
                                 | NotifyFilters.FileName
                                 | NotifyFilters.LastAccess
                                 | NotifyFilters.LastWrite
                                 | NotifyFilters.Security
                                 | NotifyFilters.Size
            };

            watcher.Error += (sender, args) =>
            {
                checker.Status = StatusEnum.Error;
                PrintException(args.GetException());
            };

            watcher.Created += (sender, args) =>
            {
                fileSender.SendFileInQueue(args.FullPath, checker);
            };

            watcher.IncludeSubdirectories = false;
            watcher.EnableRaisingEvents = true;

            return watcher;
        }

        private static void PrintException(Exception ex)
        {
            if (ex != null)
            {
                Console.WriteLine($"Message: {ex.Message}");
                Console.WriteLine("Stacktrace:");
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine();
                PrintException(ex.InnerException);
            }
        }
    }
}
