using System;
using System.IO;
using System.Configuration;
using System.Reactive.Linq;

using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.File;

using Serilog;

namespace EnterprisePics.CloudSync
{
    class Program
    {
        private static ILogger _log;

        private static CloudFileShare _share;

        private static CloudFileDirectory _rootDirectory;

        static void Main(string[] args)
        {
            _log = new LoggerConfiguration()
                .WriteTo.ColoredConsole(outputTemplate: "{Timestamp:HH:mm:ss} [{Level}] {Message}{NewLine}{Exception}")
                .CreateLogger();
            _log.Information("Starting EnterprisePics.CloudSync");

            // Read config.
            var localPath = ConfigurationManager.AppSettings["LocalPath"];
            _log.Information("Local Path: {localPath}", localPath);
            var connectionString = ConfigurationManager.AppSettings["StorageConnectionString"];
            _log.Information("Connection String: {connectionString}", connectionString);
            var shareName = ConfigurationManager.AppSettings["ShareName"];
            _log.Information("ShareName: {shareName}", shareName);

            // Get the share.
            _share = CloudStorageAccount.Parse(connectionString)
                .CreateCloudFileClient()
                .GetShareReference("pictures");
            _share.CreateIfNotExists();
            _rootDirectory = _share.GetRootDirectoryReference();

            // Init watch.
            var watcher = new RxFileSystemWatcher.ObservableFileSystemWatcher(cfg =>
            {
                cfg.Path = localPath;
                cfg.IncludeSubdirectories = true;
            });
            watcher.Created.Subscribe(OnFileCreated);
            watcher.Changed.Throttle((eventArgs => Observable.Timer(TimeSpan.FromSeconds(1)))).Subscribe(OnFileChanged);
            watcher.Deleted.Subscribe(OnFileDeleted);
            watcher.Errors.Subscribe(OnErrors);
            watcher.Renamed.Subscribe(OnFileRenamed);
            watcher.Start();

            // Wait.
            Console.ReadLine();
        }

        private static void OnFileCreated(FileSystemEventArgs fileSystemEventArgs)
        {
            try
            {
                // We ignore directory events.
                if (!fileSystemEventArgs.Name.Contains("\\"))
                    return;

                _log.Information("File created locally: {name}", fileSystemEventArgs.Name);

                var file = new FileInfo(fileSystemEventArgs.FullPath);

                // Delete the file.
                var shareDirectory = TryCreateDirectory(file);
                var shareFile = shareDirectory.GetFileReference(file.Name);
                shareFile.UploadFromFile(fileSystemEventArgs.FullPath, FileMode.Open);

                _log.Information("The file {Name} has been created on the share.", file.Name);
            }
            catch (Exception ex)
            {
                _log.Error(ex, "Error changing file.");
            }
        }

        private static void OnFileChanged(FileSystemEventArgs fileSystemEventArgs)
        {
            try
            {
                // We ignore directory events.
                if (!fileSystemEventArgs.Name.Contains("\\"))
                    return;

                _log.Information("File changed locally: {name}. Size: {length}.", fileSystemEventArgs.Name, new FileInfo(fileSystemEventArgs.FullPath).Length);

                var file = new FileInfo(fileSystemEventArgs.FullPath);

                // Delete the file.
                var shareDirectory = TryCreateDirectory(file);
                var shareFile = shareDirectory.GetFileReference(file.Name);
                shareFile.UploadFromFile(fileSystemEventArgs.FullPath, FileMode.Open);

                _log.Information("The file {Name} has been changed on the share.", file.Name);
            }
            catch (Exception ex)
            {
                _log.Error(ex, "Error changing file.");
            }
        }

        private static void OnFileDeleted(FileSystemEventArgs fileSystemEventArgs)
        {
            try
            {
                // We ignore directory events.
                if (!fileSystemEventArgs.Name.Contains("\\"))
                    return;

                _log.Warning("File deleted locally: {name}.", fileSystemEventArgs.Name);

                var file = new FileInfo(fileSystemEventArgs.FullPath);

                // Delete the file.
                var shareDirectory = TryCreateDirectory(file);
                var shareFile = shareDirectory.GetFileReference(file.Name);
                if (shareFile.Exists())
                    shareFile.Delete();

                _log.Information("The file {Name} has been deleted from the share.", file.Name);
            }
            catch (Exception ex)
            {
                _log.Error(ex, "Error deleting file.");
            }
        }

        private static void OnFileRenamed(RenamedEventArgs fileSystemEventArgs)
        {
            try
            {
                // We ignore directory events.
                if (!fileSystemEventArgs.Name.Contains("\\"))
                    return;

                _log.Information("File renamed from {oldName} to {name}.", fileSystemEventArgs.Name, fileSystemEventArgs.OldName);

                var file = new FileInfo(fileSystemEventArgs.FullPath);

                // Delete the file.
                var shareDirectory = TryCreateDirectory(file);
                var shareFile = shareDirectory.GetFileReference(file.Name);
                if (shareFile.Exists())
                {
                    // Rename here.
                }

                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                _log.Error(ex, "Error renaming file.");
            }
        }

        private static void OnErrors(ErrorEventArgs fileSystemEventArgs)
        {
            _log.Error("Exception: {exception}.", fileSystemEventArgs.GetException());
        }

        private static CloudFileDirectory TryCreateDirectory(FileInfo file)
        {
            var shareDirectory = _rootDirectory.GetDirectoryReference(file.Directory.Name);
            if (!shareDirectory.Exists())
            {
                shareDirectory.CreateIfNotExists();

                _log.Information("Created directory in share: {Name}", shareDirectory.Name);
            }

            return shareDirectory;
        }
    }
}
