namespace Simple.Wpf.Exceptions.ViewModels
{
    using System;
    using System.Reactive.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Commands;
    using Extensions;
    using Services;
    
    public sealed class MainViewModel : BaseViewModel, IMainViewModel
    {
        public MainViewModel(ISchedulerService schedulerService)
        {
            ThrowFromUiThreadCommand = ReactiveCommand.Create();
            ThrowFromTaskCommand = ReactiveCommand.Create();
            ThrowFromRxCommand = ReactiveCommand.Create();

            ThrowFromUiThreadCommand
                .ActivateGestures()
                .Subscribe(x =>
                           {
                               Logger.Info("ThrowFromUiThreadCommand executing...");

                               schedulerService.Dispatcher.Schedule<object>(null, TimeSpan.FromMilliseconds(50), (s1, s2) =>
                               {
                                   throw new Exception(x + " - thrown from UI thread.");
                               });
                           })
                .DisposeWith(this);

            ThrowFromTaskCommand
                .ActivateGestures()
                .Subscribe(x =>
                           {
                               Logger.Info("ThrowFromTaskCommand executing...");

                               Task.Factory.StartNew(() =>
                                                     {
                                                         Thread.Sleep(1000);

                                                         throw new Exception(x + " - thrown from Task StartNew.");
                                                     }, TaskCreationOptions.LongRunning);
                           })
                .DisposeWith(this);


            ThrowFromRxCommand
                .ActivateGestures()
                .Subscribe(x =>
                                         {
                                             Logger.Info("ThrowFromRxCommand executing...");

                                             Observable.Start(() =>
                                                              {
                                                                  Thread.Sleep(1000);

                                                                  throw new Exception(x + " - thrown from Rx Start.");
                                                              }, schedulerService.TaskPool)
                                                 .Take(1)
                                                 .Subscribe();
                                         })
                                         .DisposeWith(this);
        }

        public ReactiveCommand<object> ThrowFromUiThreadCommand { get; }

        public ReactiveCommand<object> ThrowFromTaskCommand { get; }

        public ReactiveCommand<object> ThrowFromRxCommand { get; }
    }
}
