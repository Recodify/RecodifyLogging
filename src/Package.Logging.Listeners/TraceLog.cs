using System;
using System.Collections.Generic;
using System.Dynamic;

namespace Recodify.Logging.Listeners.RabbitMq
{
    public class TraceLog
    {
        public TraceLog()
        {
            LogicalOperations = new List<string>();
            Data = new List<string>();
        }

        public string Environment { get; set; }
        public string RelatedActivityId { get; set; }
        public int Level { get; set; }
        public uint EventId { get; set; }
        public DateTime TimeCreated { get; set; }
        public long EventCacheTimestamp { get; set; }
        public int Severity { get; set; }
        public string Type { get; set; }
        public string CorrelationActivityId { get; set; }
        public string SourceName { get; set; }
        public List<string> LogicalOperations { get; set; }
        public string LogicalOperation { get; set; }
        public string CallStack { get; set; }
        public List<string> Data { get; set; }
        public string Message { get; set; }
        public string MachineName { get; set; }
        public string Host { get; set; }
        public string Component { get; set; }
        public Guid RequestId { get; set; }
        public string UrlLocalPath { get; set; }
       

        public ExpandoObject ToExpando()
        {
            dynamic expando = new ExpandoObject();
            expando.RelatedActivityId = RelatedActivityId;
            expando.Level = Level;
            expando.EventId = EventId;
            expando.TimeCreated = TimeCreated;
            expando.EventCacheTimestamp = EventCacheTimestamp;
            expando.Severity = Severity;
            expando.Type = Type;
            expando.CorrelationActivityId = CorrelationActivityId;
            expando.SourceName = SourceName;
            expando.LogicalOperations = LogicalOperations;
            expando.LogicalOperation = LogicalOperation;
            expando.CallStack = CallStack;
            expando.Data = Data;
            expando.Message = Message;
            expando.MachineName = MachineName;
            expando.Host = Host;
            expando.Component = Component;
            expando.RequestId = RequestId;
            expando.UrlLocalPath = UrlLocalPath;

            return expando;
        }
    }
}
