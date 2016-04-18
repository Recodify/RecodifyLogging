namespace Recodify.Messaging.RabbitMq
{
	public class QueueInfo
	{
		public string Name { get; set; }
		public int TotalMessageCount { get; set; }
		public int MessagesReadyCount { get; set; }
		public int MessagesUnackedCount { get; set; }

		public QueueInfo(string name, int messages, int messagesReady, int messagesUnacknowledged)
		{
			Name = name;
			TotalMessageCount = messages;
			MessagesReadyCount = messagesReady;
			MessagesUnackedCount = messagesUnacknowledged;
		}
	}
}