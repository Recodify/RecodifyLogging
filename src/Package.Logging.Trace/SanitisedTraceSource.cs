using System;
using System.Diagnostics;
using System.Linq;
using Recodify.Logging.Trace.Sanitisation;
using System.Collections.Generic;

namespace Recodify.Logging.Trace
{
	public class SanitisedTraceSource : ITraceSource
	{
		private readonly ITraceSource traceSource;
		private readonly ISanitiser sanitiser;

		public SanitisedTraceSource(string name, ISanitiser sanitiser)
			: this(name, sanitiser, new TraceSource(name))
		{
		}

		public SanitisedTraceSource(string name, ISanitiser sanitiser, ITraceSource traceSource)
		{
			this.traceSource = traceSource;
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
				sanitisedData = data.Select(o => SantitiseObject(o)).ToArray();
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

		private string Sanitise(string input)
		{
			return sanitiser.Sanitise(input);
		}

		private object SantitiseObject(object o)
		{			
			if (o.GetType() == typeof(string))
			{
				return sanitiser.Sanitise(o.ToString());
			}
			else if (o.GetType() == typeof(KeyValuePair<string, object>))
			{
				if (((KeyValuePair<string, object>)o).Value.GetType() == typeof(string))
				{
					var s = ((KeyValuePair<string, object>)o).Value.ToString();
					return sanitiser.Sanitise(s);
				}
			}
			else if (o.GetType() == typeof(IDictionary<string, IEnumerable<string>>))
			{
				var s = ((IDictionary<string, IEnumerable<string>>)o).ToDictionary(k => k.Key, v => v.Value.Select(y => sanitiser.Sanitise(y)));
				return s;
			}

			return o;
		}

		private object[] Sanitise(params object[] args)
		{
			return args.Select(arg => SantitiseObject(arg)).ToArray();
		}

		public void TraceResponse(int statusCode, string headers, string content, long timing, string url, string identity, string sessionId = null)
		{
			traceSource.TraceResponse(statusCode, headers, content, timing, url, identity, sessionId);
		}

		public void TraceRequest(string requestMethod, string headers, string content, string url, string ipAddress, string identity, string sessionId = null)
		{
			traceSource.TraceRequest(requestMethod, headers, content, url, ipAddress, identity, sessionId);
		}
	}
}