using System;
using System.Diagnostics;
using System.Linq;
using Recodify.Logging.Trace.Sanitisation;
using System.Collections.Generic;

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
