using System.Reactive.Concurrency;

namespace Simple.Wpf.Exceptions.Services
{
    public interface ISchedulerService
    {
        IScheduler Dispatcher { get; }

        IScheduler Current { get; }

        IScheduler TaskPool { get; }

        IScheduler EventLoop { get; }

        IScheduler NewThread { get; }

        IScheduler StaThread { get; }
    }
}
