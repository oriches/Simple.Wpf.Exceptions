namespace Simple.Wpf.Exceptions
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Reactive.Disposables;
    using NLog;

    public sealed class Duration : IDisposable
    {
        private readonly string _context;
        private readonly Stopwatch _stopWatch;
        private readonly Logger _logger;

        private Duration(Logger logger, string context)
        {
            _context = context;
            _stopWatch = new Stopwatch();
            _logger = logger;

            _stopWatch.Start();
        }

        public static IDisposable Measure(Logger logger, string context, params object[] args)
        {
            if (!logger.IsDebugEnabled)
            {
                return Disposable.Empty;
            }

            if (args != null)
            {
                context = string.Format(CultureInfo.InvariantCulture, context, args);
            }

            return new Duration(logger, context);
        }

        public void Dispose()
        {
            _stopWatch.Stop();

            _logger.Debug(CultureInfo.InvariantCulture, "{0}, duration = {1} ms", _context, _stopWatch.ElapsedMilliseconds);
        }
    }
}
