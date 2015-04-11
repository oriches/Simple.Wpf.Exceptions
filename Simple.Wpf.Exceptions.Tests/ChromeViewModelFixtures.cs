using System.Reactive.Disposables;
using System.Reactive.Subjects;
using System.Windows.Input;
using Moq;
using Microsoft.Reactive.Testing;
using NUnit.Framework;
using Simple.Wpf.Exceptions.Extensions;
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

        private Subject<OverlayViewModel> _show;

        private MainViewModel _mainViewModel;

        [SetUp]
        public void Setup()
        {
            _testScheduler = new TestScheduler();
            _schedulerService = new MockSchedulerService(_testScheduler);

            _overlayService = new Mock<IOverlayService>();

            _gestureService = new Mock<IGestureService>();
            _gestureService.Setup(x => x.SetBusy());

            _show = new Subject<OverlayViewModel>();
            _overlayService.Setup(x => x.Show).Returns(_show);

            _mainViewModel = new MainViewModel(_gestureService.Object, _schedulerService);
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
            var contentViewModel = new Mock<BaseViewModel>();
            var overlayViewModel = new OverlayViewModel("header 1", contentViewModel.Object, Disposable.Empty);

            // ACT
            _show.OnNext(overlayViewModel);

            // ASSERT
            Assert.That(viewModel.HasOverlay, Is.True);
            Assert.That(viewModel.OverlayHeader, Is.EqualTo("header 1"));
            Assert.That(viewModel.Overlay, Is.EqualTo(contentViewModel.Object));
        }

        [Test]
        public void clears_overlay()
        {
            // ARRANGE
            var viewModel = new ChromeViewModel(_mainViewModel, _overlayService.Object);
            var contentViewModel = new Mock<BaseViewModel>();
            var overlayViewModel = new OverlayViewModel("header 1", contentViewModel.Object, Disposable.Empty);

            _show.OnNext(overlayViewModel);

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

            var contentViewModel1 = new Mock<BaseViewModel>();
            var overlayViewModel1 = new OverlayViewModel("header 1", contentViewModel1.Object, Disposable.Empty);

            var contentViewModel2 = new Mock<BaseViewModel>();
            var overlayViewModel2 = new OverlayViewModel("header 2", contentViewModel2.Object, Disposable.Empty);

            // ACT
            _show.OnNext(overlayViewModel1);
            _show.OnNext(overlayViewModel2);

            // ASSERT
            Assert.That(viewModel.HasOverlay, Is.True);
            Assert.That(viewModel.OverlayHeader, Is.EqualTo("header 2"));
            Assert.That(viewModel.Overlay, Is.EqualTo(contentViewModel2.Object));
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