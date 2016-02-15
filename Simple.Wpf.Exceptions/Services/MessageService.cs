namespace Simple.Wpf.Exceptions.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive.Disposables;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using Extensions;
    using Models;
    using ViewModels;

    public sealed class MessageService : BaseService, IMessageService
    {
        private readonly Subject<Message> _show;
        private readonly Queue<Message> _waitingMessages = new Queue<Message>();
		
		private readonly object _sync = new object();

        public MessageService()
        {
            _show = new Subject<Message>()
                .DisposeWith(this);

            Add(Disposable.Create(() => _waitingMessages.Clear()));
        }

        public void Post(string header, ICloseableViewModel viewModel)
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

        public IObservable<Message> Show => _show;
    }
}
