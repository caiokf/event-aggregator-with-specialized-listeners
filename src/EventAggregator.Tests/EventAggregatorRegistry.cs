using System.Threading;
using StructureMap.Configuration.DSL;

namespace EventAggregator.Tests
{
    public class EventAggregatorRegistry : Registry
    {
        public EventAggregatorRegistry()
        {
            For<IEventAggregator>().Singleton().Use<EventAggregator>();

            RegisterInterceptor(new EventAggregatorListenerInterceptor());

            For<SynchronizationContext>().Singleton().Use(ctx =>
            {
                if (SynchronizationContext.Current == null)
                    SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());
                
                return SynchronizationContext.Current;
            });
        }
    }
}