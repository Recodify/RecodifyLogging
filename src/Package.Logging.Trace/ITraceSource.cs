using System;
using System.Diagnostics;

namespace Recodify.Logging.Trace
{
	public interface ITraceSource
	{
		void TraceData(TraceEventType eventType, int id, object data);
		void TraceData(TraceEventType eventType, int id, params object[] data);
		void TraceEvent(TraceEventType eventType, int id, string message);
		void TraceEvent(TraceEventType eventType, int id, string format, params object[] args);
		void TraceInformation(string message);
		void TraceInformation(string format, params object[] args);
		void TraceTransfer(int id, string message, Guid relatedActivityId);
		void TraceResponse(int statusCode, string headers, string content, long timing, string url, string sessionId = null);
		void TraceRequest(string requestMethod, string headers, string content, string url, string ipAddress, string sessionId = null);
	}
}