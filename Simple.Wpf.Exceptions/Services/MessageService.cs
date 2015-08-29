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
    using Models;

    public sealed class MessageService : IMessageService, IDisposable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IDisposable _disposable;
        private readonly Subject<Message> _show;
        private readonly Queue<Message> _waitingMessages = new Queue<Message>();
		
		private readonly object _sync = new object();

        public MessageService()
        {
            _show = new Subject<Message>();

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

        public void Post(string header, CloseableViewModel viewModel)
        {
            var newMessage = new Message(header, viewModel);
			
            newMessage.ViewModel.Closed
				.Take(1)
			    .Subscribe(x =>
                {
					Message nextMessage = null;
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
        }

        public IObservable<Message> Show { get { return _show; } }
    }
}
