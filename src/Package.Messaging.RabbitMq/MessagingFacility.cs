using System;

namespace Recodify.Messaging.RabbitMq
{
    public interface IMessagingFacility
    {
        void Configure();
    }

    public class MessagingFacility : IMessagingFacility
    {
        private readonly Messaging messaging;

        public MessagingFacility(Messaging messaging)
        {
            if (messaging == null) throw new ArgumentNullException("messaging");
            this.messaging = messaging;
        }

        public virtual void Configure()
        {
            var advancedBus = messaging.AdvancedBus;

            var exchange = messaging.EventExchange;
            var eventQueue = messaging.EventQueue;
            var binding = messaging.EventBinding;

            var deadExchange = messaging.DeadEventExchange;
            var deadQueue = messaging.DeadEventQueue;
            var deadbinding = messaging.DeadEventBinding;

            var commandQueue = messaging.CommandQueue;

            var deadCommandExchange = messaging.DeadCommandExchange;
            var deadCommandQueue = messaging.DeadCommandQueue;
            var deadCommandBinding = messaging.DeadCommandBinding;
        }
    }
}