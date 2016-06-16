using NUnit.Framework;
using Recodify.Logging.Listeners.RabbitMq;
using System;

namespace UnitTests
{
	public class TraceListenerTests
    {
		[Test]
		public void Should_LogExceptions_WhenTheyContainAnInnerException()
		{
			var trace = new TraceListener("events-live,logs-live,azuresync");
			try
			{
				try
				{
					throw new Exception("tatatata");
				}
				catch (Exception inner)
				{
					throw new Exception("blah", inner);
				}
				
			}
			catch (Exception exp)
			{
				trace.TraceData(null, "mimimim", System.Diagnostics.TraceEventType.Error, 0, "shix", exp);
			}
		}
    }
}
