using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using NLog;
using Simple.Wpf.Exceptions.ViewModels;

namespace Simple.Wpf.Exceptions.Services
{
    public sealed class MessageService : IMessageService, IDisposable
    {
        private readonly ISchedulerService _schedulerService;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IDisposable _disposable;
        private readonly Subject<MessageViewModel> _show;
        private readonly Queue<MessageViewModel> _waitingMessages = new Queue<MessageViewModel>();

        public MessageService(ISchedulerService schedulerService)
        {
            _schedulerService = schedulerService;
            _show = new Subject<MessageViewModel>();

            _disposable = Disposable.Create(() =>
            {
                _waitingMessages.Clear();

                _show.OnCompleted();
                _show.Dispose();
            });
        }

        public void Dispose()
        {
            using (Duration.Measure(Logger, "Dispose"))
            {
                _disposable.Dispose();
            }
        }

        public void Post(string header, CloseableViewModel viewModel, IDisposable lifetime)
        {
            var newMessage = new MessageViewModel(header, viewModel, lifetime);
            newMessage.ViewModel.Closed
                .ObserveOn(_schedulerService.Dispatcher)
                .Subscribe(x =>
                {
                    using (_waitingMessages.Dequeue().ViewModel)
                    {
                        if (_waitingMessages.Any())
                        {
                            _show.OnNext(_waitingMessages.Peek());
                        }
                    }
                });

            _waitingMessages.Enqueue(newMessage);
            if (_waitingMessages.Count == 1)
            {
                _show.OnNext(_waitingMessages.Peek());
            }
        }

        public IObservable<MessageViewModel> Show { get { return _show; } }
    }
}