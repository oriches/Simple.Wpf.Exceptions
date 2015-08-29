using System.Reactive.Subjects;
using System.Windows.Input;
using Moq;
using Microsoft.Reactive.Testing;
using NUnit.Framework;
using Simple.Wpf.Exceptions.Extensions;
using Simple.Wpf.Exceptions.Models;
using Simple.Wpf.Exceptions.Services;
using Simple.Wpf.Exceptions.ViewModels;

namespace Simple.Wpf.Exceptions.Tests
{
    [TestFixture]
    public sealed class ChromeViewModelFixtures
    {
        private TestScheduler _testScheduler;
        private ISchedulerService _schedulerService;

        private Mock<IGestureService> _gestureService;
        private Mock<IOverlayService> _overlayService;

        private Subject<Message> _messages;

        private MainViewModel _mainViewModel;

        [SetUp]
        public void Setup()
        {
            _testScheduler = new TestScheduler();
            _schedulerService = new MockSchedulerService(_testScheduler);

            _overlayService = new Mock<IOverlayService>();

            _gestureService = new Mock<IGestureService>();
            _gestureService.Setup(x => x.SetBusy());

            _messages = new Subject<Message>();
            _overlayService.Setup(x => x.Show).Returns(_messages);

            _mainViewModel = new MainViewModel(_gestureService.Object, _overlayService.Object, _schedulerService);
        }

        [Test]
        public void no_overlay_when_created()
        {
            // ARRANGE
            // ACT
            var viewModel = new ChromeViewModel(_mainViewModel, _overlayService.Object);

            // ASSERT
            Assert.That(viewModel.HasOverlay, Is.False);
            Assert.That(viewModel.OverlayHeader, Is.Empty);
            Assert.That(viewModel.Overlay, Is.Null);
        }

        [Test]
        public void shows_overlay()
        {
            // ARRANGE
            var viewModel = new ChromeViewModel(_mainViewModel, _overlayService.Object);
            var overlayViewModel = new OverlayViewModel();
            var message = new Message("header 1", overlayViewModel);

            // ACT
            _messages.OnNext(message);

            // ASSERT
            Assert.That(viewModel.HasOverlay, Is.True);
            Assert.That(viewModel.OverlayHeader, Is.EqualTo("header 1"));
            Assert.That(viewModel.Overlay, Is.EqualTo(overlayViewModel));
        }

        [Test]
        public void clears_overlay()
        {
            // ARRANGE
            var viewModel = new ChromeViewModel(_mainViewModel, _overlayService.Object);
            var overlayViewModel = new OverlayViewModel();
            var message = new Message("header 1", overlayViewModel);

            _messages.OnNext(message);

            // ACT
            viewModel.CloseOverlayCommand.Execute(null);

            // ASSERT
            Assert.That(viewModel.HasOverlay, Is.False);
            Assert.That(viewModel.OverlayHeader, Is.Empty);
            Assert.That(viewModel.Overlay, Is.Null);
        }

        [Test]
        public void replaces_overlay()
        {
            // ARRANGE
            var viewModel = new ChromeViewModel(_mainViewModel, _overlayService.Object);

            var overlayViewModel1 = new OverlayViewModel();
            var message1 = new Message("header 1", overlayViewModel1);

            var overlayViewModel2 = new OverlayViewModel();
            var message2 = new Message("header 2", overlayViewModel2);

            // ACT
            _messages.OnNext(message1);
            _messages.OnNext(message2);

            // ASSERT
            Assert.That(viewModel.HasOverlay, Is.True);
            Assert.That(viewModel.OverlayHeader, Is.EqualTo("header 2"));
            Assert.That(viewModel.Overlay, Is.EqualTo(overlayViewModel2));
        }

        [Test]
        public void disposing_clears_commands()
        {
            // ARRANGE
            var viewModel = new ChromeViewModel(_mainViewModel, _overlayService.Object);

            // ACT
            viewModel.Dispose();

            // ASSERT
            var commandProperties = TestHelper.PropertiesImplementingInterface<ICommand>(viewModel);
            commandProperties.ForEach(x => Assert.That(x.GetValue(viewModel, null), Is.Null));
        }
    }
}