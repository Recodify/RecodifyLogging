using System;
using System.Diagnostics;
using System.Linq;
using Recodify.Logging.Trace.Sanitisation;
using System.Collections.Generic;
using Recodify.Logging.Web;

namespace Recodify.Logging.Trace
{   
    public class SanitisedTraceSource : ITraceSource
    {
        private readonly TraceSource traceSource;
        private readonly ISanitiser sanitiser;

        public SanitisedTraceSource(string name, ISanitiser sanitiser)
        {
            this.traceSource = new TraceSource(name);
            this.sanitiser = sanitiser;
        }

        public SanitisedTraceSource(string name, SourceLevels defaulLevel, ISanitiser sanitiser)
        {
            this.traceSource = new TraceSource(name, defaulLevel);
            this.sanitiser = sanitiser;
        }

        public void TraceData(TraceEventType eventType, int id, object data)
        {            
            traceSource.TraceData(eventType, id, Sanitise(data == null ? string.Empty : data.ToString()));
        }

        public void TraceData(TraceEventType eventType, int id, params object[] data)
        {
            var sanitisedData = data;

            if (data != null && data.Any())
            {
               sanitisedData = data.Select(o => o.GetType() == typeof(string) ? sanitiser.Sanitise(o.ToString()) : o).ToArray();
            }

            traceSource.TraceData(eventType, id, sanitisedData);
        }

        public void TraceEvent(TraceEventType eventType, int id, string message)
        {
            traceSource.TraceEvent(eventType, id, Sanitise(message));
        }

        public void TraceEvent(TraceEventType eventType, int id, string format, params object[] args)
        {
            traceSource.TraceEvent(eventType, id, format, Sanitise(args));
        }

        public void TraceInformation(string message)
        {
            traceSource.TraceInformation(Sanitise(message));
        }

        public void TraceInformation(string format, params object[] args)
        {
            traceSource.TraceInformation(format, Sanitise(args));
        }

        public void TraceTransfer(int id, string message, Guid relatedActivityId)
        {
            traceSource.TraceTransfer(id, Sanitise(message), relatedActivityId);
        }

		public void TraceRequest(string requestMethod, string headers, string content, string url, string ipAddress, string sessionId = null)
		{
			TraceData(
			   TraceEventType.Information,
			   (int)EventId.RequestReceived,
			   new KeyValuePair<string, object>("method", requestMethod),
			   new KeyValuePair<string, object>("requestUrl", url),
			   new KeyValuePair<string, object>("headers", headers),
			   new KeyValuePair<string, object>("message", content),
			   new KeyValuePair<string, object>("tags", new[] { "request", "http" }),
			   new KeyValuePair<string, object>("sessionId", sessionId),
			   new KeyValuePair<string, object>("clientip", ipAddress));
		}

		public void TraceResponse(int statusCode, string headers, string content, long timing, string url, string sessionId = null)
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

		private string Sanitise(string input)
        {
            return sanitiser.Sanitise(input);
        }

        private string[] Sanitise(params object[] args)
        {         
            return args.Select(arg => sanitiser.Sanitise(Convert.ToString((object) arg))).ToArray();
        }
    }
}
