namespace Simple.Wpf.Exceptions.Views
{
    using System;
    using System.Reactive;
    using System.Reactive.Linq;
    using System.Reactive.Threading.Tasks;
    using MahApps.Metro.Controls;
    using MahApps.Metro.Controls.Dialogs;
    using Services;

    public partial class MainWindow : MetroWindow
    {
        private readonly IDisposable _disposable;

        public MainWindow(IMessageService messageService, ISchedulerService schedulerService)
        {
            InitializeComponent();

            _disposable = messageService.Show
                // Delay to make sure there is time for the animations
                .Delay(TimeSpan.FromMilliseconds(250), schedulerService.TaskPool)
                .ObserveOn(schedulerService.Dispatcher)
                .Select(x => new MessageDialog(x))
                .SelectMany(ShowDialogAsync, (x, y) => x)
                .Subscribe();

            Closed += HandleClosed;
        }

        private void HandleClosed(object sender, EventArgs e)
        {
            _disposable.Dispose();
        }
        
        private IObservable<Unit> ShowDialogAsync(MessageDialog dialog)
        {
            var settings = new MetroDialogSettings
            {
                AnimateShow = true,
                AnimateHide = true,
                ColorScheme = MetroDialogColorScheme.Accented
            };

            return this.ShowMetroDialogAsync(dialog, settings)
                .ToObservable()
                .SelectMany(x => dialog.CloseableContent.Closed, (x, y) => x)
                .SelectMany(x => this.HideMetroDialogAsync(dialog).ToObservable(), (x, y) => x);
        }
    }
}
