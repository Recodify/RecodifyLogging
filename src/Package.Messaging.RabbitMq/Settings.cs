using System.Collections.Specialized;
using System.Configuration;

namespace Recodify.Messaging.RabbitMq
{
    public class Settings
    {
        public Settings()
        {
            Type = "direct";
            Passive = false;
            Durable = true;
            AutoDelete = false;
        }

        public virtual string ConnectionString { get; set; }
        public virtual string ExchangeName { get; set; }
        public virtual string QueueName { get; set; }
        public virtual string Type { get; set; }
        public virtual bool Passive { get; set; }
        public virtual bool Durable { get; set; }
        public virtual bool AutoDelete { get; set; }

		public virtual string ManagementUrl { get; set; }
		public virtual string ManagementUsername { get; set; }
		public virtual string ManagementPassword { get; set; }
    }

    public class AppSettings : Settings
    {
        private readonly NameValueCollection applicationSettings;

        public AppSettings()
        {
            applicationSettings = ConfigurationManager.AppSettings;
        }

        public override string ConnectionString
        {
            get { return applicationSettings["RabbitMqConnectionString"].ToLower(); }
        }

        public override string ExchangeName
        {
            get { return applicationSettings["RabbitMqExchangeName"].ToLower(); }
        }

        public override string QueueName
        {
            get { return applicationSettings["RabbitMqQueueName"].ToLower(); }
        }

	    public override string ManagementUrl
	    {
			get { return applicationSettings["RabbitMqManagementUrl"].ToLower(); }
	    }

		public override string ManagementUsername
		{
			get { return applicationSettings["RabbitMqManagementUsername"].ToLower(); }
		}

		public override string ManagementPassword
		{
			get { return applicationSettings["RabbitMqManagementPassword"].ToLower(); }
		}
    }  
}