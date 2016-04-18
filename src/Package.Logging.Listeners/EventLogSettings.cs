using Recodify.Messaging.RabbitMq;
using System.Collections.Specialized;
using System.Configuration;

namespace Recodify.Logging.Listeners.RabbitMq
{
    public class EventLogSettings : AppSettings
    {
        private readonly NameValueCollection applicationSettings;

        public EventLogSettings()
        {
            applicationSettings = ConfigurationManager.AppSettings;
        }

        public override string QueueName { get; set; }

        public override string ExchangeName { get; set; }
    }
}
