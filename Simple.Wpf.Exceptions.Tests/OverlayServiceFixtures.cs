using System;
using Moq;
using NUnit.Framework;
using Simple.Wpf.Exceptions.Services;
using Simple.Wpf.Exceptions.ViewModels;

namespace Simple.Wpf.Exceptions.Tests
{
    using Models;

    [TestFixture]
    public sealed class OverlayServiceFixtures
    {
        [Test]
        public void posts_overlay()
        {
            // ARRANGE
            var contentViewModel = new Mock<CloseableViewModel>();
            
            var service = new OverlayService();

            Message message = null;
            service.Show.Subscribe(x => message = x);

            // ACT
            service.Post("header 1", contentViewModel.Object);

            // ASSERT
            Assert.That(message.Header, Is.EqualTo("header 1"));
            Assert.That(message.ViewModel, Is.EqualTo(contentViewModel.Object));
        }

        [Test]
        public void disposing_completes_show_stream()
        {
            // ARRANGE
            var completed = false;

            var service = new OverlayService();
            service.Show.Subscribe(x => { }, () => { completed = true; });

            // ACT
            service.Dispose();
            
            // ASSERT
            Assert.That(completed, Is.True);
        }
    }
}