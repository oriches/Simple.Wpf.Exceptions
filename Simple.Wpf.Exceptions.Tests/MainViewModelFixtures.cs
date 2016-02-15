namespace Simple.Wpf.Exceptions.Tests
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Reactive.Testing;
    using Moq;
    using NUnit.Framework;
    using Services;
    using ViewModels;

    [TestFixture]
    public sealed class MainViewModelFixtures
    {
        private TestScheduler _testScheduler;
        private ISchedulerService _schedulerService;

        [SetUp]
        public void Setup()
        {
            _testScheduler = new TestScheduler();
            _schedulerService = new MockSchedulerService(_testScheduler);

            var gestureService = new Mock<IGestureService>();
            gestureService.Setup(x => x.SetBusy());

            Extensions.ObservableExtensions.GestureService = gestureService.Object;
        }

        [Test]
        public void throws_exception_on_ui_thread()
        {
            // ARRANGE
            var exceptionText = "This is the exception message!";
            var expectedResult = "This is the exception message! - thrown from UI thread.";

            var viewModel = new MainViewModel(_schedulerService);

            // ACT
            Exception thrownException = null;
            try
            {
                viewModel.ThrowFromUiThreadCommand.Execute(exceptionText);

                _testScheduler.AdvanceBy(TimeSpan.FromMilliseconds(100));
            }
            catch (Exception exn)
            {
                thrownException = exn;
            }

            // ASSERT
            Assert.That(thrownException, Is.Not.Null);
            Assert.That(thrownException.Message, Is.EqualTo(expectedResult));
        }

        [Test]
        public void throws_exception_from_task_startnew()
        {
            // ARRANGE
            var exceptionText = "This is the exception message!";
            var expectedResult = "This is the exception message! - thrown from Task StartNew.";

            var viewModel = new MainViewModel(_schedulerService);

            Exception thrownException = null;
            TaskScheduler.UnobservedTaskException += (s, e) =>
            {
                thrownException = e.Exception.InnerException;
            };

            // ACT
            viewModel.ThrowFromTaskCommand.Execute(exceptionText);

            // Hack to test this...
            Thread.Sleep(5000);

            GC.Collect();
            GC.WaitForPendingFinalizers();

            // ASSERT
            Assert.That(thrownException, Is.Not.Null);
            Assert.That(thrownException.Message, Is.EqualTo(expectedResult));
        }

        [Test]
        public void throws_exception_from_rx_start()
        {
            // ARRANGE
            var exceptionText = "This is the exception message!";
            var expectedResult = "This is the exception message! - thrown from Rx Start.";

            var viewModel = new MainViewModel(_schedulerService);

            Exception thrownException = null;

            // ACT
            try
            {
                viewModel.ThrowFromRxCommand.Execute(exceptionText);

                _testScheduler.AdvanceBy(TimeSpan.FromSeconds(1));
            }
            catch(Exception exn)
            {
                thrownException = exn;
            }

            // ASSERT
            Assert.That(thrownException, Is.Not.Null);
            Assert.That(thrownException.Message, Is.EqualTo(expectedResult));
        }
    }
}
