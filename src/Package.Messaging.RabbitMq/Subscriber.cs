using System;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Recodify.Messaging.RabbitMq
{
    public interface ISubscriber
    {
        void Subscribe<T>(Action<T> onMessageReceived) where T : class;
    }

    public class Subscriber : Messaging, ISubscriber
    {
        private readonly JsonSerializerSettings jsonSettings;

        public Subscriber(IBusFactory busFactory, Settings settings)
            : base(busFactory, settings)
        {
            jsonSettings = new JsonSerializerSettings {ContractResolver = new CamelCasePropertyNamesContractResolver()};
        }

        public void Subscribe<T>(Action<T> onMessageReceived) where T : class
        {
            AdvancedBus.Consume(EventQueue,
                                   (b ,p, i) =>
                                       {
                                           var jsonString = Encoding.UTF8.GetString(b);
                                           var message = JsonConvert.DeserializeObject<T>(jsonString);
                                           return Task.Factory.StartNew(() => onMessageReceived(message));
                                       });
        }
    }
}