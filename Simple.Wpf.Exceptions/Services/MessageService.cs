using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using NLog;
using Simple.Wpf.Exceptions.ViewModels;

namespace Simple.Wpf.Exceptions.Services
{
    public sealed class MessageService : IMessageService, IDisposable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IDisposable _disposable;
        private readonly Subject<MessageViewModel> _show;
        private readonly Queue<MessageViewModel> _waitingMessages = new Queue<MessageViewModel>();
		
		private readonly object _sync = new object();

        public MessageService()
        {
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

        public IObservable<Unit> Post(string header, CloseableViewModel viewModel, IDisposable lifetime)
        {
            var newMessage = new MessageViewModel(header, viewModel, lifetime);
			
            newMessage.ViewModel.Closed
				.Take(1)
			    .Subscribe(x =>
                {
					MessageViewModel nextMessage = null;
					lock(_sync)
					{
						_waitingMessages.Dequeue();
						
						if (_waitingMessages.Any())
						{
							nextMessage = _waitingMessages.Peek();
						}
					}
					
					if (nextMessage != null)
					{
						_show.OnNext(nextMessage);
					}
                });

			bool show;
			lock(_sync)
			{
				_waitingMessages.Enqueue(newMessage);
				show = _waitingMessages.Count == 1;
			}
            
            if (show)
            {
                _show.OnNext(newMessage);
            }

            return viewModel.Closed;
        }

        public IObservable<MessageViewModel> Show { get { return _show; } }
    }
}
