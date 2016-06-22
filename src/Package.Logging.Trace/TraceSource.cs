using Recodify.Logging.Common;
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

		public virtual void TraceData(TraceEventType eventType, int id, object data)
		{
			traceSource.TraceData(eventType, id, data);
		}

		public virtual void TraceData(TraceEventType eventType, int id, params object[] data)
		{
			traceSource.TraceData(eventType, id, data);
		}

		public virtual void TraceEvent(TraceEventType eventType, int id, string message)
		{
			traceSource.TraceEvent(eventType, id, message);
		}

		public virtual void TraceEvent(TraceEventType eventType, int id, string format, params object[] args)
		{
			traceSource.TraceEvent(eventType, id, format, args);
		}

		public virtual void TraceInformation(string message)
		{
			traceSource.TraceInformation(message);
		}

		public virtual void TraceInformation(string format, params object[] args)
		{
			traceSource.TraceInformation(format, args);
		}

		public virtual void TraceTransfer(int id, string message, Guid relatedActivityId)
		{
			traceSource.TraceTransfer(id, message, relatedActivityId);
		}

		public virtual void TraceRequest(string requestMethod, string headers, string content, string url, string ipAddress, string identity, string sessionId = null)
		{			
			var fallbackTraceSource = new System.Diagnostics.TraceSource(fallbackKey);
			
			try
			{
				TraceData(
				   TraceEventType.Information,
				   (int)EventId.RequestReceived,
				   new KeyValuePair<string, object>("requestUrl", url),
				   new KeyValuePair<string, object>("httpMethod", requestMethod),				   
				   new KeyValuePair<string, object>("headers", headers),
				   new KeyValuePair<string, object>("message", content),
				   new KeyValuePair<string, object>("tags", new[] { "request", "http" }),
				   new KeyValuePair<string, object>("sessionId", sessionId),
				   new KeyValuePair<string, object>("clientip", ipAddress),
				   new KeyValuePair<string, object>("identity", identity));
			}
			catch(Exception exp)
			{
				fallbackTraceSource.TraceData(TraceEventType.Error, (int)Event.LoggingExceptionFallingBack, exp);
			}
		}

		public virtual void TraceResponse(int statusCode, string headers, string content, long timing, string url, string identity, string sessionId = null)
		{
			try
			{
				if (statusCode < 399)
				{
					RaiseTraceResponse(TraceEventType.Information, statusCode, headers, content, timing, url, identity, sessionId);
				}
				else if (statusCode >= 400 && statusCode <= 499)
				{
					RaiseTraceResponse(TraceEventType.Warning, statusCode, headers, content, timing, url, identity, sessionId);
				}
				else if (statusCode > 499)
				{
					RaiseTraceResponse(TraceEventType.Error, statusCode, headers, content, timing, url, identity, sessionId);
				}
			}
			catch (Exception exp)
			{
				fallbackTraceSource.TraceData(TraceEventType.Error, (int)Event.LoggingExceptionFallingBack, exp);
			}
		}

		private void RaiseTraceResponse(TraceEventType eventType, int statusCode, string headers, string content, long timing, string url, string identity, string sessionId)
		{
			TraceData(
				eventType,
				statusCode,
				new KeyValuePair<string, object>("requestUrl", url),
				new KeyValuePair<string, object>("responseTime", timing),				
				new KeyValuePair<string, object>("headers", headers),
				new KeyValuePair<string, object>("message", content),
				new KeyValuePair<string, object>("tags", new[] { "response", "http" }),
				new KeyValuePair<string, object>("sessionId", sessionId ?? string.Empty),
				new KeyValuePair<string, object>("identity", identity));
		}
	}
}
