using System.Collections.Generic;
using System.Linq;
using System.Text;
using EasyNetQ;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Recodify.Messaging.RabbitMq
{
    public interface IPublisher
    {
        void Publish<T>(T message) where T : class;
	    IEnumerable<QueueInfo> GetQueueInfos();
    }

    public class Publisher : Messaging, IPublisher
    {
        private readonly JsonSerializerSettings jsonSettings;

        public Publisher(IBusFactory busFactory, Settings settings) : base(busFactory, settings)
        {
            jsonSettings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver(), ReferenceLoopHandling = ReferenceLoopHandling.Serialize };
        }

        public virtual void Publish<T>(T message) where T : class
        {
            var properties = new MessageProperties {ContentType = "application/json"};

            var json = JsonConvert.SerializeObject(message, Formatting.Indented, jsonSettings);
            var jsonBytes = Encoding.UTF8.GetBytes(json);

            AdvancedBus.Publish(EventExchange, "#", mandatory: false, immediate: false, messageProperties: properties, body: jsonBytes);
        }

	    public IEnumerable<QueueInfo> GetQueueInfos()
	    {
			return ManagementClient
				.GetQueues()
				.Where(q => q.Name != null && q.Name.ToLower() == EventQueue.Name.ToLower())
				.Select(q => new QueueInfo(q.Name, q.Messages, q.MessagesReady, q.MessagesUnacknowledged));
	    }
    }
}