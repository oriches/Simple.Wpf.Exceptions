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
            ThrowFromUiThreadCommand = ReactiveCommand.Create()
                .DisposeWith(this);

            ThrowFromTaskCommand = ReactiveCommand.Create()
                .DisposeWith(this);

            ThrowFromRxCommand = ReactiveCommand.Create()
                .DisposeWith(this);

            ThrowFromUiThreadCommand
                .ActivateGestures()
                .SafeSubscribe(x =>
                           {
                               Logger.Info("ThrowFromUiThreadCommand executing...");
                               throw new Exception(x + " - thrown from UI thread.");
                           }, schedulerService.Dispatcher)
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
