using System;
using System.Diagnostics;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using Autofac;
using Autofac.Core;
using NLog;
using Simple.Wpf.Exceptions.Helpers;
using Simple.Wpf.Exceptions.Services;
using Simple.Wpf.Exceptions.ViewModels;
using Simple.Wpf.Exceptions.Views;

namespace Simple.Wpf.Exceptions
{
    public partial class App : Application
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly CompositeDisposable _disposable;
        private IMessageService _messageService;
        private ISchedulerService _schedulerService;

        public App()
        {
#if DEBUG
            LogHelper.ReconfigureLoggerToLevel(LogLevel.Debug);
#endif

            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
            Application.Current.DispatcherUnhandledException += DispatcherOnUnhandledException;
            TaskScheduler.UnobservedTaskException += TaskSchedulerOnUnobservedTaskException;

            _disposable = new CompositeDisposable();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            Logger.Info("Starting");
            Logger.Info("Dispatcher managed thread identifier = {0}", System.Threading.Thread.CurrentThread.ManagedThreadId);

            Logger.Info("WPF rendering capability (tier) = {0}", RenderCapability.Tier / 0x10000);
            RenderCapability.TierChanged += (s, a) => Logger.Info("WPF rendering capability (tier) = {0}", RenderCapability.Tier / 0x10000);

            base.OnStartup(e);

            BootStrapper.Start();

            _messageService = BootStrapper.Resolve<IMessageService>();
            _schedulerService = BootStrapper.Resolve<ISchedulerService>();

            var window = new MainWindow(_messageService, _schedulerService);

            // The window has to be created before the root visual - all to do with the idling service initialising correctly...
            window.DataContext = BootStrapper.RootVisual;

            window.Closed += (s, a) =>
            {
                _disposable.Dispose();
                BootStrapper.Stop();
            };

            Current.Exit += (s, a) =>
            {
                Logger.Info("Bye Bye!");
                LogManager.Flush();
            };

            window.Show();

#if DEBUG
            _disposable.Add(ObserveUiFreeze());
#endif
            Logger.Info("Started");
        }

        private static IDisposable ObserveUiFreeze()
        {
            var timer = new DispatcherTimer(DispatcherPriority.Normal)
            {
                Interval = Constants.UiFreezeTimer
            };

            var previous = DateTime.Now;
            timer.Tick += (sender, args) =>
            {
                var current = DateTime.Now;
                var delta = current - previous;
                previous = current;

                if (delta > Constants.UiFreeze)
                {
                    Debug.WriteLine("UI Freeze = {0} ms", delta.TotalMilliseconds);
                }
            };

            timer.Start();
            return Disposable.Create(timer.Stop);
        }

        private void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs args)
        {
            Logger.Info("Unhandled app domain exception");
            HandleException(args.ExceptionObject as Exception);
        }

        private void DispatcherOnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs args)
        {
            Logger.Info("Unhandled dispatcher thread exception");
            args.Handled = true;

            HandleException(args.Exception);
        }

        private void TaskSchedulerOnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs args)
        {
            Logger.Info("Unhandled task exception");
            args.SetObserved();

            HandleException(args.Exception.GetBaseException());
        }

        private void HandleException(Exception exception)
        {
            Logger.Error(exception);

            _schedulerService.Dispatcher.Schedule<object>(null, (s1, s2) =>
            {
                var parameters = new Parameter[] { new NamedParameter("exception", exception) };
                var viewModel = BootStrapper.Resolve<ExceptionViewModel>(parameters);
                
                var disposable = viewModel.Closed
                    .Take(1)
                    .Subscribe(x => ownedViewModel.Dispose());
                
                _messageService.Post("whoops - something's gone wrong!", viewModel);
                
                return disposable;
            });
        }
    }
}
