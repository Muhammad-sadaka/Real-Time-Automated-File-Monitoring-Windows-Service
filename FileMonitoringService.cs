using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.IO;

namespace FileMonitoringService
{
    public partial class FileMonitoringService : ServiceBase
    {
        private FileSystemWatcher fileWatcher;
        private string SourceFolder = ConfigurationManager.AppSettings["SourceFolder"];
        private string DestinationFolder = ConfigurationManager.AppSettings["DestinationFolder"];
        private string LogFolder = ConfigurationManager.AppSettings["LogFolder"];

        public FileMonitoringService()
        {
            InitializeComponent();

            if (string.IsNullOrWhiteSpace(SourceFolder))
            {
                SourceFolder = @"F:\FileMonitoring\Source";
                Log("SourceFolder is missing in App.config. Using default: " + SourceFolder);
            }

            if (string.IsNullOrWhiteSpace(DestinationFolder))
            {
                DestinationFolder = @"F:\FileMonitoring\Destination";
                Log("DestinationFolder is missing in App.config. Using default: " + DestinationFolder);
            }

            if (string.IsNullOrWhiteSpace(LogFolder))
            {
                LogFolder = @"F:\FileMonitoring\Logs"; // Default log folder
                Log("LogFolder is missing in App.config. Using default: " + LogFolder);
            }

            // Ensure directories exist
            Directory.CreateDirectory(SourceFolder);
            Directory.CreateDirectory(DestinationFolder);
            Directory.CreateDirectory(LogFolder);
        }

        private void Log(string message)
        {
            string logFilePath = Path.Combine(LogFolder, "ServiceLog.txt");
            string logMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}\n";

            File.AppendAllText(logFilePath, logMessage);

            if (Environment.UserInteractive)
            {
                Console.WriteLine(logMessage);
            }
        }

        protected override void OnStart(string[] args)
        {
            Log("Service Started.");

            fileWatcher = new FileSystemWatcher
            {
                Path = SourceFolder,
                Filter = "*.*",
                EnableRaisingEvents = true,
                IncludeSubdirectories = false
            };

            fileWatcher.Created += OnFileCreated;

            Log("File monitoring started on folder: " + SourceFolder);
        }

        protected override void OnStop()
        {
            fileWatcher.EnableRaisingEvents = false;
            fileWatcher.Dispose();
            Log("Service Stopped.");
        }

        public void StartInConsole()
        {
            OnStart(null);
            Console.WriteLine("Press Enter to stop the service...");
            Console.ReadLine(); 
            OnStop(); 
            Console.ReadKey();
        }

        private void OnFileCreated(Object sender,FileSystemEventArgs e)
        {
            int attempts = 0;
            while (!IsFileReady(e.FullPath))
            {
                System.Threading.Thread.Sleep(500);
                attempts++;

                if (attempts > 10)
                {
                    Log($"File {e.Name} took too long to be ready. Skipping.");
                    return;
                }
            }
            try
            {
                // Log file creation
                Log($"File detected: {e.FullPath}");

                // Generate GUID and prepare new file name
                string newFileName = $"{Guid.NewGuid()}{Path.GetExtension(e.Name)}";
                string destinationFile = Path.Combine(DestinationFolder, newFileName);

                // Move and rename the file
                File.Move(e.FullPath, destinationFile);

                // Log success
                Log($"File moved: {e.FullPath} -> {destinationFile}");
            }
            catch (Exception ex)
            {
                Log($"Error processing file: {e.FullPath}. Exception: {ex.Message}");
            }
        }

        private bool IsFileReady(string filename)
        {
            try
            {
                using (FileStream chkStream = File.Open(filename, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                {
                    if (chkStream != null)
                    {
                        return true;
                    }
                }
            }
            catch (IOException)
            {
                return false;
            }
            return false;
        }


    }
}
