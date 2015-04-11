using System;
using System.Windows.Input;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Microsoft.Reactive.Testing;
using NUnit.Framework;
using Simple.Wpf.Exceptions.Extensions;
using Simple.Wpf.Exceptions.Services;
using Simple.Wpf.Exceptions.ViewModels;

namespace Simple.Wpf.Exceptions.Tests
{
    [TestFixture]
    public sealed class MainViewModelFixtures
    {
        private Mock<IGestureService> _gestureService;

        private TestScheduler _testScheduler;
        private ISchedulerService _schedulerService;

        [SetUp]
        public void Setup()
        {
            _testScheduler = new TestScheduler();
            _schedulerService = new MockSchedulerService(_testScheduler);

            _gestureService = new Mock<IGestureService>();
            _gestureService.Setup(x => x.SetBusy());
        }

        [Test]
        public void throws_exception_on_ui_thread()
        {
            // ARRANGE
            var exceptionText = "This is the exception message!";
            var expectedResult = "This is the exception message! - thrown from UI thread.";

            var viewModel = new MainViewModel(_gestureService.Object, _schedulerService);

            // ACT
            Exception thrownException = null;
            try
            {
                viewModel.ThrowFromUiThreadCommand.Execute(exceptionText);
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

            var viewModel = new MainViewModel(_gestureService.Object, _schedulerService);

            Exception thrownException = null;
            TaskScheduler.UnobservedTaskException += (s, e) => { thrownException = e.Exception.InnerException; };

            // ACT
            viewModel.ThrowFromTaskCommand.Execute(exceptionText);

            Thread.Sleep(1000);

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

            var viewModel = new MainViewModel(_gestureService.Object, _schedulerService);

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

        [Test]
        public void disposing_clears_commands()
        {
            // ARRANGE
            var viewModel = new MainViewModel(_gestureService.Object, _schedulerService);

            // ACT
            viewModel.Dispose();

            // ASSERT
            var commandProperties = TestHelper.PropertiesImplementingInterface<ICommand>(viewModel);
            commandProperties.ForEach(x => Assert.That(x.GetValue(viewModel, null), Is.Null));
        }
    }
}
