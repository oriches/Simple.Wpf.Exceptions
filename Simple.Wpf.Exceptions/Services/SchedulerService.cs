using System;
using System.Reactive.Concurrency;
using System.Threading;

namespace Simple.Wpf.Exceptions.Services
{
    public sealed class SchedulerService : ISchedulerService
    {
        private readonly DispatcherScheduler _dispatcherScheduler;

        public SchedulerService()
        {
            _dispatcherScheduler = DispatcherScheduler.Current;
        }

        public IScheduler Dispatcher { get { return _dispatcherScheduler; } }

        public IScheduler Current { get { return CurrentThreadScheduler.Instance; } }

        public IScheduler TaskPool { get { return TaskPoolScheduler.Default; } }

        public IScheduler EventLoop { get { return new EventLoopScheduler(); } }

        public IScheduler NewThread { get { return NewThreadScheduler.Default; } }
        
        public IScheduler StaThread
        {
            get
            {
                Func<ThreadStart, Thread> func = x =>
                {
                    var thread = new Thread(x) { IsBackground = true };
                    thread.SetApartmentState(ApartmentState.STA);

                    return thread;
                };

                return new EventLoopScheduler(func);
            }
        }
    }
}
