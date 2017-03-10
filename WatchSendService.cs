using System;
using System.Diagnostics;
using System.IO;
using System.ServiceProcess;

namespace WatchSendService
{
    public class WatchSendService : ServiceBase
    {
        public const string MyServiceName = "WatchSendService";
        private FileSystemWatcher watcher = null;
        
        public WatchSendService()
        {
            InitializeComponent();
        }
        
        private void InitializeComponent()
        {
            this.ServiceName = MyServiceName;
        }
        
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
        
        protected override void OnStart(string[] args)
        {
            watcher = new FileSystemWatcher(@"d:\data\xmlimport\", "*.xml");

            watcher.NotifyFilter = NotifyFilters.LastAccess
                | NotifyFilters.LastWrite
                | NotifyFilters.FileName
                | NotifyFilters.DirectoryName;

            watcher.Changed += OnChanged;
            watcher.Created += OnChanged;
            watcher.Deleted += OnChanged;
            watcher.Renamed += OnRenamed;

            watcher.EnableRaisingEvents = true;
        }
        
        protected override void OnStop()
        {
            watcher.EnableRaisingEvents = false;
            watcher.Dispose();

            LogEvent("Monitoring Stopped");
        }

        void OnChanged(object sender, FileSystemEventArgs e)
        {
            string msg = string.Format("File {0} | {1}",
                                       e.FullPath, e.ChangeType);
            LogEvent(msg);
        }

        void OnRenamed(object sender, RenamedEventArgs e)
        {
            string log = string.Format("{0} | Renamed from {1}",
                                       e.FullPath, e.OldName);
            LogEvent(log);
        }
        
        private void LogEvent(string message)
        {
            string eventSource = "File Monitor and Send Service";
            DateTime dt = new DateTime();
            dt = System.DateTime.UtcNow;
            message = dt.ToLocalTime() + ": " + message;

            EventLog.WriteEntry(eventSource, message);
        }
    }
}
