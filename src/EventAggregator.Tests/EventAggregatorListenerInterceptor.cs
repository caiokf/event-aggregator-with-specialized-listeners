using System;
using StructureMap;
using StructureMap.Interceptors;
using StructureMap.TypeRules;

namespace EventAggregator.Tests
{
    public class EventAggregatorListenerInterceptor : TypeInterceptor
    {
        public object Process(object target, IContext context)
        {
            context.GetInstance<IEventAggregator>().AddListener((IListenTo)target);
            return target;
        }

        public bool MatchesType(Type type)
        {
            return type.ImplementsInterfaceTemplate(typeof(IListenTo<>));
        }
    }
}