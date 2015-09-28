using NUnit.Framework;
using Rhino.Mocks;
using StructureMap;

namespace EventAggregator.Tests
{
    [TestFixture]
    public class EventAggregatorTests
    {
        private IEventAggregator _eventAggregator;

        [SetUp]
        public void SetUp()
        {
            ObjectFactory.Initialize(x => x.AddRegistry<EventAggregatorRegistry>());

            _eventAggregator = ObjectFactory.GetInstance<IEventAggregator>();
        }

        [Test]
        public void all_listeners_must_be_notified_of_published_messages()
        {
            var listener1 = MockRepository.GenerateMock<IListenTo<SomeEvent>.All>();
            var listener2 = MockRepository.GenerateMock<IListenTo<SomeEvent>.All>();

            var someEvent = new SomeEvent();
            listener1.Expect(x => x.Handle(someEvent)).Repeat.Once();
            listener2.Expect(x => x.Handle(someEvent)).Repeat.Once();

            _eventAggregator.AddListener(listener1);
            _eventAggregator.AddListener(listener2);

            _eventAggregator.SendMessage(someEvent);

            listener1.VerifyAllExpectations();
            listener2.VerifyAllExpectations();
        }

        [Test]
        public void directed_message_listeners_must_be_notified_only_when_message_were_sent_to_them()
        {
            var listener1 = MockRepository.GenerateMock<IListenTo<SomeDirectedEvent>.SentToMe>();
            var listener2 = MockRepository.GenerateMock<IListenTo<SomeDirectedEvent>.SentToMe>();
            var listener3 = MockRepository.GenerateMock<IListenTo<SomeDirectedEvent>.All>();

            var someEvent = new SomeDirectedEvent { Receiver = listener1 };
            listener1.Expect(x => x.Handle(someEvent)).Repeat.Once();
            listener2.Expect(x => x.Handle(someEvent)).Repeat.Never();
            listener3.Expect(x => x.Handle(someEvent)).Repeat.Once();

            _eventAggregator.AddListener(listener1);
            _eventAggregator.AddListener(listener2);
            _eventAggregator.AddListener(listener3);

            _eventAggregator.SendMessage(someEvent);

            listener1.VerifyAllExpectations();
            listener2.VerifyAllExpectations();
            listener3.VerifyAllExpectations();
        }

        [Test]
        public void filtered_listeners_must_be_notified_only_when_condition_were_satisfied()
        {
            var listener1 = MockRepository.GenerateMock<IListenTo<SomeEvent>.ThatSatisfy>();
            var listener2 = MockRepository.GenerateMock<IListenTo<SomeEvent>.ThatSatisfy>();
            var listener3 = MockRepository.GenerateMock<IListenTo<SomeEvent>.All>();

            var someEvent = new SomeEvent();

            listener1.Expect(x => x.Handle(someEvent)).Repeat.Once();
            listener1.Expect(x => x.SatisfiedBy(someEvent)).Return(true);

            listener2.Expect(x => x.Handle(someEvent)).Repeat.Never();
            listener2.Expect(x => x.SatisfiedBy(someEvent)).Return(false);

            listener3.Expect(x => x.Handle(someEvent)).Repeat.Once();

            _eventAggregator.AddListener(listener1);
            _eventAggregator.AddListener(listener2);
            _eventAggregator.AddListener(listener3);

            _eventAggregator.SendMessage(someEvent);

            listener1.VerifyAllExpectations();
            listener2.VerifyAllExpectations();
            listener3.VerifyAllExpectations();
        }

        public class SomeEvent : IEvent { }
        public class SomeDirectedEvent : IDirectedEvent
        {
            public object Receiver { get; set; }
        }
    }
}
