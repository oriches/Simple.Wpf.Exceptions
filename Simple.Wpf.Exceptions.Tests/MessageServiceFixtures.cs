using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using Simple.Wpf.Exceptions.Models;
using Simple.Wpf.Exceptions.Services;
using Simple.Wpf.Exceptions.ViewModels;

namespace Simple.Wpf.Exceptions.Tests
{
    [TestFixture]
    public sealed class MessageServiceFixtures
    {
        [Test]
        public void posts_message_with_lifetime()
        {
            // ARRANGE
            var contentViewModel = new Mock<CloseableViewModel>();
            var service = new MessageService();

            Message message = null;
            service.Show.Subscribe(x => message = x);

            // ACT
            service.Post("header 1", contentViewModel.Object);

            // ASSERT
            Assert.That(message.Header, Is.EqualTo("header 1"));
            Assert.That(message.ViewModel, Is.EqualTo(contentViewModel.Object));
        }

        [Test]
        public void posts_message_without_lifetime()
        {
            // ARRANGE
            var contentViewModel = new Mock<CloseableViewModel>();

            var service = new MessageService();

            Message message = null;
            service.Show.Subscribe(x => message = x);

            // ACT
            service.Post("header 1", contentViewModel.Object);

            // ASSERT
            Assert.That(message.Header, Is.EqualTo("header 1"));
            Assert.That(message.ViewModel, Is.EqualTo(contentViewModel.Object));
        }

        [Test]
        public void posts_mulitple_messages()
        {
            // ARRANGE
            var contentViewModel1 = new Mock<CloseableViewModel>();
            var contentViewModel2 = new Mock<CloseableViewModel>();

            var service = new MessageService();

            var messages = new List<Message>();
            service.Show.Subscribe(x => messages.Add(x));

            service.Post("header 1", contentViewModel1.Object);
            service.Post("header 2", contentViewModel2.Object);

            // ACT
            messages.First().ViewModel.CloseCommand.Execute(null);

            // ASSERT
            Assert.That(messages.Count(x => x.Header == "header 1") == 1, Is.True);
            Assert.That(messages.Count(x => x.Header == "header 2") == 1, Is.True);
        }

        [Test]
        public void disposing_completes_show_stream()
        {
            // ARRANGE
            var completed = false;

            var service = new MessageService();
            service.Show.Subscribe(x => { }, () => { completed = true; });

            // ACT
            service.Dispose();

            // ASSERT
            Assert.That(completed, Is.True);
        }
    }
}
