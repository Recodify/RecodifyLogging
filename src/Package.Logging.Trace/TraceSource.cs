using Recodify.Logging.Web;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Recodify.Logging.Trace
{
	public class TraceSource : ITraceSource
	{
		private readonly System.Diagnostics.TraceSource traceSource;
		private readonly System.Diagnostics.TraceSource fallbackTraceSource;
		private const string fallbackKey = "Fallback";

		public TraceSource(string name)
		{
			traceSource = new System.Diagnostics.TraceSource(name);
			fallbackTraceSource = new System.Diagnostics.TraceSource(fallbackKey);
		}

		public TraceSource(string name, SourceLevels defaultLevel)
		{
			traceSource = new System.Diagnostics.TraceSource(name, defaultLevel);
		}

		public void TraceData(TraceEventType eventType, int id, object data)
		{
			traceSource.TraceData(eventType, id, data);
		}

		public void TraceData(TraceEventType eventType, int id, params object[] data)
		{
			traceSource.TraceData(eventType, id, data);
		}

		public void TraceEvent(TraceEventType eventType, int id, string message)
		{
			traceSource.TraceEvent(eventType, id, message);
		}

		public void TraceEvent(TraceEventType eventType, int id, string format, params object[] args)
		{
			traceSource.TraceEvent(eventType, id, format, args);
		}

		public void TraceInformation(string message)
		{
			traceSource.TraceInformation(message);
		}

		public void TraceInformation(string format, params object[] args)
		{
			traceSource.TraceInformation(format, args);
		}

		public void TraceTransfer(int id, string message, Guid relatedActivityId)
		{
			traceSource.TraceTransfer(id, message, relatedActivityId);
		}

		public void TraceRequest(string requestMethod, string headers, string content, string url, string ipAddress, string sessionId = null)
		{
			var fallbackTraceSource = new System.Diagnostics.TraceSource(fallbackKey);

			try
			{
				TraceData(
				   TraceEventType.Information,
				   (int)EventId.RequestReceived,
				   new KeyValuePair<string, object>("httpMethod", requestMethod),
				   new KeyValuePair<string, object>("requestUrl", url),
				   new KeyValuePair<string, object>("headers", headers),
				   new KeyValuePair<string, object>("message", content),
				   new KeyValuePair<string, object>("tags", new[] { "request", "http" }),
				   new KeyValuePair<string, object>("sessionId", sessionId),
				   new KeyValuePair<string, object>("clientip", ipAddress));
			}
			catch(Exception exp)
			{
				fallbackTraceSource.TraceData(TraceEventType.Error, 9883, exp);
			}
		}

		public void TraceResponse(int statusCode, string headers, string content, long timing, string url, string sessionId = null)
		{
			try
			{
				if (statusCode < 399)
				{
					RaiseTraceResponse(TraceEventType.Information, statusCode, headers, content, timing, url, sessionId);
				}
				else if (statusCode >= 400 && statusCode <= 499)
				{
					RaiseTraceResponse(TraceEventType.Warning, statusCode, headers, content, timing, url, sessionId);
				}
				else if (statusCode > 499)
				{
					RaiseTraceResponse(TraceEventType.Error, statusCode, headers, content, timing, url, sessionId);
				}
			}
			catch (Exception exp)
			{
				fallbackTraceSource.TraceData(TraceEventType.Error, 9883, exp);
			}
		}

		private void RaiseTraceResponse(TraceEventType eventType, int statusCode, string headers, string content, long timing, string url, string sessionId)
		{
			TraceData(
				eventType,
				statusCode,
				new KeyValuePair<string, object>("responseTime", timing),
				new KeyValuePair<string, object>("requestUrl", url),
				new KeyValuePair<string, object>("headers", headers),
				new KeyValuePair<string, object>("message", content),
				new KeyValuePair<string, object>("tags", new[] { "response", "http" }),
				new KeyValuePair<string, object>("sessionId", sessionId ?? string.Empty));
		}
	}
}
