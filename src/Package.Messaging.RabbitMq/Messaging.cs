using System;
using EasyNetQ;
using EasyNetQ.Management.Client;
using EasyNetQ.Topology;

namespace Recodify.Messaging.RabbitMq
{
    public class Messaging
    {
        private Lazy<IAdvancedBus> advancedBus;
		private Lazy<IManagementClient> managementClient;

        private Lazy<IExchange> eventExchange;
        private Lazy<IQueue> eventQueue;
        private Lazy<IBinding> eventBinding;

        private Lazy<IExchange> deadEventExchange;
        private Lazy<IQueue> deadEventQueue;
        private Lazy<IBinding> deadEventBinding;

        private Lazy<IQueue> commandQueue;
        
        private Lazy<IExchange> deadCommandExchange;
        private Lazy<IQueue> deadCommandQueue;
        private Lazy<IBinding> deadCommandBinding;
        
        public virtual IAdvancedBus AdvancedBus
        {
            get { return advancedBus.Value; }
        }

        public virtual IBinding EventBinding
        {
            get { return eventBinding.Value; }
        }

        public virtual IExchange EventExchange
        {
            get { return eventExchange.Value; }
        }

        public virtual IBinding DeadEventBinding
        {
            get { return deadEventBinding.Value; }
        }

        public virtual IExchange DeadEventExchange
        {
            get { return deadEventExchange.Value; }
        }

        public virtual IExchange DeadCommandExchange
        {
            get { return deadCommandExchange.Value; }
        }

        public virtual IBinding DeadCommandBinding
        {
            get { return deadCommandBinding.Value; }
        }

        public virtual IQueue EventQueue
        {
            get { return eventQueue.Value; }
        }

        public virtual IQueue DeadEventQueue
        {
            get { return deadEventQueue.Value; }
        }

        public virtual IQueue CommandQueue
        {
            get { return commandQueue.Value; }
        }

        public virtual IQueue DeadCommandQueue
        {
            get { return deadCommandQueue.Value; }
        }

	    public virtual IManagementClient ManagementClient
	    {
			get { return managementClient.Value; }
	    }

        public Messaging(IBusFactory busFactory, Settings settings)
        {
            advancedBus = new Lazy<IAdvancedBus>(() => busFactory.CreateBus(settings).Advanced);

            InitializeEventMessaging(settings);
            InitializeCommandMessaging(settings);

            managementClient = new Lazy<IManagementClient>(() =>  new ManagementClient(settings.ManagementUrl, settings.ManagementUsername, settings.ManagementPassword) );
        }

        private void InitializeEventMessaging(Settings settings)
        {
            var deadEventExchangeName = settings.ExchangeName + "-dead";

            eventQueue = new Lazy<IQueue>(() => AdvancedBus.QueueDeclare(settings.QueueName, passive: settings.Passive, durable: settings.Durable, autoDelete: settings.AutoDelete, deadLetterExchange: deadEventExchangeName));
            eventExchange = new Lazy<IExchange>(() => AdvancedBus.ExchangeDeclare(settings.ExchangeName, ExchangeType.Direct, passive: settings.Passive, durable: settings.Durable, autoDelete: settings.AutoDelete));
            eventBinding = new Lazy<IBinding>(() => AdvancedBus.Bind(EventExchange, EventQueue, "#"));

            deadEventExchange = new Lazy<IExchange>(() => AdvancedBus.ExchangeDeclare(deadEventExchangeName, ExchangeType.Fanout, passive: settings.Passive, durable: settings.Durable, autoDelete: settings.AutoDelete));
            deadEventQueue = new Lazy<IQueue>(() => AdvancedBus.QueueDeclare(settings.QueueName + "-dead", passive: settings.Passive, durable: settings.Durable, autoDelete: settings.AutoDelete));
            deadEventBinding = new Lazy<IBinding>(() => AdvancedBus.Bind(DeadEventExchange, DeadEventQueue, ""));
        }

        private void InitializeCommandMessaging(Settings settings)
        {
            var deadCommandExchangeName = settings.ExchangeName + "-command-dead";
            commandQueue = new Lazy<IQueue>(() => AdvancedBus.QueueDeclare(settings.QueueName + "-command", passive: settings.Passive, durable: settings.Durable, autoDelete: settings.AutoDelete, deadLetterExchange: deadCommandExchangeName));
            deadCommandQueue = new Lazy<IQueue>(() => AdvancedBus.QueueDeclare(settings.QueueName + "-command-dead", passive: settings.Passive, durable: settings.Durable, autoDelete: settings.AutoDelete));
            deadCommandExchange = new Lazy<IExchange>(() => AdvancedBus.ExchangeDeclare(deadCommandExchangeName, ExchangeType.Fanout, passive: settings.Passive, durable: settings.Durable, autoDelete: settings.AutoDelete));
            deadCommandBinding = new Lazy<IBinding>(() => AdvancedBus.Bind(DeadCommandExchange, DeadCommandQueue, ""));
        }  
    }
}