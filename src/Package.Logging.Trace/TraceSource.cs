using System;
using System.Diagnostics;

namespace Recodify.Logging.Trace
{
    public class TraceSource : ITraceSource
    {
        private readonly System.Diagnostics.TraceSource traceSource;

        public TraceSource(string name)
        {
            traceSource = new System.Diagnostics.TraceSource(name);
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
    }
}
