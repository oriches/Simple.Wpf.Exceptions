using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using Simple.Wpf.Exceptions.ViewModels;

namespace Simple.Wpf.Exceptions.Views
{
    using System;
    using MahApps.Metro.Controls.Dialogs;
    using Services;

    public partial class MainWindow
    {
        private readonly IDisposable _disposable;

        public MainWindow(IMessageService messageService, ISchedulerService schedulerService)
        {
            InitializeComponent();

            var settings = new MetroDialogSettings
            {
                AnimateShow = true,
                AnimateHide = true,
                ColorScheme = MetroDialogColorScheme.Inverted
            };

            _disposable = messageService.Show
                // Delay to make sure there is time for the animations
                .Delay(TimeSpan.FromMilliseconds(250), schedulerService.TaskPool)
                .ObserveOn(schedulerService.Dispatcher)
                .Select(x => new MessageDialog { Title = x.Header, Content = x.ViewModel })
                .SelectMany(x => this.ShowMetroDialogAsync(x, settings).ToObservable(), (x, y) => x)
                .ObserveOn(schedulerService.Dispatcher)
                .SelectMany(x => ((CloseableViewModel)x.Content).Closing, (x, y) => x)
                .Select(x => this.HideMetroDialogAsync(x).ToObservable())
                .Subscribe();

            Closed += HandleClosed;
        }

        private void HandleClosed(object sender, EventArgs e)
        {
            _disposable.Dispose();
        }
    }
}
