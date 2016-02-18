using System.Windows;
using System.Linq;

namespace Simple.Wpf.Exceptions.Services
{
    using NLog;
    using NLog.Targets;

    public sealed class ApplicationService : IApplicationService
    {
        private string _logFolder;

        public string LogFolder
        {
            get
            {
                if (!string.IsNullOrEmpty(_logFolder))
                {
                    return _logFolder;
                }

                _logFolder = GetLogFolder();
                return _logFolder;
            }
        }

        public void CopyToClipboard(string text)
        {
            Clipboard.SetText(text);
        }

        public void Exit()
        {
            Application.Current.Shutdown();
        }

        public void Restart()
        {
            System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
            Application.Current.Shutdown();
        }

        public void OpenFolder(string folder)
        {
            System.Diagnostics.Process.Start("explorer.exe", folder);
        }

        private static string GetLogFolder()
        {
            var logFile = LogManager.Configuration.AllTargets
                .OfType<FileTarget>()
                .Select(x => x.FileName as NLog.Layouts.SimpleLayout)
                .Select(x => x.Text)
                .FirstOrDefault();

            return System.IO.Path.GetDirectoryName(logFile);
        }
    }
}