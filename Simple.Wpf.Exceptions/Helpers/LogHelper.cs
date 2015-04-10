using System.Collections.Generic;
using System.Linq;
using NLog;
using Simple.Wpf.Exceptions.Extensions;

namespace Simple.Wpf.Exceptions.Helpers
{
    public static class LogHelper
    {
        private static readonly IEnumerable<LogLevel> AllLevels = new[]
            {
                LogLevel.Trace,
                LogLevel.Debug,
                LogLevel.Info,
                LogLevel.Warn,
                LogLevel.Error,
                LogLevel.Fatal,
            };
        
        public static void ReconfigureLoggerToLevel(LogLevel level)
        {
            var disableLevels = AllLevels.Where(x => x < level)
                .ToArray();

            var enableLevels = AllLevels.Where(x => x >= level)
                .ToArray();

            foreach (var rule in LogManager.Configuration.LoggingRules)
            {
                var localRule = rule;

                disableLevels.ForEach(localRule.DisableLoggingForLevel);
                enableLevels.ForEach(localRule.EnableLoggingForLevel);
            }

            LogManager.ReconfigExistingLoggers();
        }
    }
}
