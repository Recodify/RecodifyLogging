using System;
using System.Diagnostics;
using System.Linq;
using Recodify.Logging.Trace.Sanitisation;
using System.Collections.Generic;

namespace Recodify.Logging.Trace
{
	public class SanitisedTraceSource : TraceSource
	{	
		private readonly ISanitiser sanitiser;
		private readonly WebDataEnricher enricher;

		public SanitisedTraceSource(string name, ISanitiser sanitiser)
			: base (name)
		{
			this.sanitiser = sanitiser;
		}

		public SanitisedTraceSource(string name, IEnricher enricher, ISanitiser sanitiser)
			: base(name, enricher)
		{
			this.sanitiser = sanitiser;
		}


		public SanitisedTraceSource(System.Diagnostics.TraceSource traceSource, IEnricher enricher, ISanitiser sanitiser)
			: base(traceSource, enricher)
		{
			this.sanitiser = sanitiser;
		}

		public override void TraceData(TraceEventType eventType, int id, object data)
		{
			base.TraceData(eventType, id, Sanitise(data == null ? string.Empty : data.ToString()));
		}

		public override void TraceData(TraceEventType eventType, int id, params object[] data)
		{			
			var sanitisedData = data;

			if (data != null && data.Any())
			{
				sanitisedData = data.Select(o => SantitiseObject(o)).ToArray();
			}

			base.TraceData(eventType, id, sanitisedData);
		}

		public override void TraceEvent(TraceEventType eventType, int id, string message)
		{
			base.TraceEvent(eventType, id, Sanitise(message));
		}

		public override void TraceEvent(TraceEventType eventType, int id, string format, params object[] args)
		{			
			base.TraceEvent(eventType, id, format, Sanitise(args));
		}

		public override void TraceInformation(string message)
		{
			base.TraceInformation(Sanitise(message));
		}

		public override void TraceInformation(string format, params object[] args)
		{			
			base.TraceInformation(format, Sanitise(args));
		}

		public override void TraceTransfer(int id, string message, Guid relatedActivityId)
		{
			base.TraceTransfer(id, Sanitise(message), relatedActivityId);
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
	}
}