using System;
using System.Linq;
using System.Collections.Generic;
using System.Reactive.Disposables;
using Microsoft.Reactive.Testing;
using Moq;
using NUnit.Framework;
using Simple.Wpf.Exceptions.Services;
using Simple.Wpf.Exceptions.ViewModels;

namespace Simple.Wpf.Exceptions.Tests
{
    [TestFixture]
    public sealed class MessageServiceFixtures
    {
        private ISchedulerService _schedulerService;
        private TestScheduler _testScheduler;

        [SetUp]
        public void Setup()
        {
            _testScheduler = new TestScheduler();
            _schedulerService = new MockSchedulerService(_testScheduler);
        }

        [Test]
        public void posts_message_with_lifetime()
        {
            // ARRANGE
            var contentViewModel = new Mock<CloseableViewModel>();
            var lifetime = Disposable.Empty;

            var service = new MessageService(_schedulerService);

            MessageViewModel messageViewModel = null;
            service.Show.Subscribe(x => messageViewModel = x);

            // ACT
            service.Post("header 1", contentViewModel.Object, lifetime);

            // ASSERT
            Assert.That(messageViewModel.HasLifetime, Is.True);
            Assert.That(messageViewModel.Lifetime, Is.EqualTo(lifetime));
            Assert.That(messageViewModel.Header, Is.EqualTo("header 1"));
            Assert.That(messageViewModel.ViewModel, Is.EqualTo(contentViewModel.Object));
        }

        [Test]
        public void posts_message_without_lifetime()
        {
            // ARRANGE
            var contentViewModel = new Mock<CloseableViewModel>();

            var service = new MessageService(_schedulerService);

            MessageViewModel messageViewModel = null;
            service.Show.Subscribe(x => messageViewModel = x);

            // ACT
            service.Post("header 1", contentViewModel.Object, null);

            // ASSERT
            Assert.That(messageViewModel.HasLifetime, Is.False);
            Assert.That(messageViewModel.Lifetime, Is.Null);
            Assert.That(messageViewModel.Header, Is.EqualTo("header 1"));
            Assert.That(messageViewModel.ViewModel, Is.EqualTo(contentViewModel.Object));
        }

        [Test]
        public void posts_mulitple_messages()
        {
            // ARRANGE
            var contentViewModel1 = new Mock<CloseableViewModel>();
            var contentViewModel2 = new Mock<CloseableViewModel>();

            var service = new MessageService(_schedulerService);

            var messages = new List<MessageViewModel>();
            service.Show.Subscribe(x => messages.Add(x));

            service.Post("header 1", contentViewModel1.Object, Disposable.Empty);
            service.Post("header 2", contentViewModel2.Object, Disposable.Empty);

            // ACT
            messages.First().ViewModel.CloseCommand.Execute(null);

            _testScheduler.AdvanceBy(TimeSpan.FromSeconds(2));

            // ASSERT
            Assert.That(messages.Count(x => x.Header == "header 1") == 1, Is.True);
            Assert.That(messages.Count(x => x.Header == "header 2") == 1, Is.True);
        }

        [Test]
        public void disposing_completes_show_stream()
        {
            // ARRANGE
            var completed = false;

            var service = new MessageService(_schedulerService);
            service.Show.Subscribe(x => { }, () => { completed = true; });

            // ACT
            service.Dispose();

            // ASSERT
            Assert.That(completed, Is.True);
        }
    }
}