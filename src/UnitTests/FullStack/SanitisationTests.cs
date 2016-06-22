using Moq;
using NUnit.Framework;
using Recodify.Logging.Common;
using Recodify.Logging.Trace;
using Recodify.Logging.Trace.Sanitisation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests.FullStack
{
	public class TraceSourceTests
	{

		[Test]
		public void SessionIdIsLogged()
		{
			var mockTrace = new Mock<ITraceSource>();
			var traceSource = new Recodify.Logging.Trace.SanitisedTraceSource("Request", new Sanitiser(), mockTrace.Object);
			
			var sessionId = Guid.NewGuid().ToString();
			traceSource.TraceRequest("GET", "someHeader", "someContent");
			mockTrace.Setup(x => x.TraceData(TraceEventType.Information, (int)EventId.RequestReceived, It.Is<object[]>(y => y.Any(o => ((KeyValuePair<string,object>)o).Value.ToString() == sessionId))));
		}		
	}

	public class SanitisedTraceSourceTests
	{
		

		public void SessionIdIsLogged()
		{

		}
	}
}
