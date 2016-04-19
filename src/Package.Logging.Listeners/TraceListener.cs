using Recodify.Messaging.RabbitMq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Web;

namespace Recodify.Logging.Listeners.RabbitMq
{
    public class TraceListener : System.Diagnostics.TraceListener
    {
        private const string requestIdKey = "RequestId";
        private readonly string machineName = Environment.MachineName;
        private readonly IPublisher publisher;
        private readonly string componentName;
        private readonly TraceSource fallbackSource;

        [ResourceExposure(ResourceScope.Machine)]
        [ResourceConsumption(ResourceScope.Machine)]
        public TraceListener(string data)
        {
            if (string.IsNullOrWhiteSpace(data) || !data.Contains(","))
                throw new ArgumentException("You must supply an exchange and queue as a single string comman delimited string: e.g. LogExchange,LogQueue");

            var initData = data.Split(',');
            var exchangeName = initData.First();
            var queueName = initData.Skip(1).First();
            componentName = initData.Last();

            fallbackSource = new TraceSource("Fallback");

            var busFactory = new BusFactory();
            publisher = new Publisher(busFactory, new EventLogSettings { ExchangeName = exchangeName, QueueName = queueName});
        }

        public override void Write(string message)
        {
            WriteLine(message);
        }

        public override void WriteLine(string message)
        {
            TraceEvent(null, "Trace", TraceEventType.Information, 0, message);
        }

        public override void Fail(string message, string detailMessage)
        {
            var failMessage = new StringBuilder(message);
            if (detailMessage != null)
            {
                failMessage.Append(" ");
                failMessage.Append(detailMessage);
            }

            TraceEvent(null, "Trace", TraceEventType.Error, 0, failMessage.ToString());
        }

        public override void TraceEvent(TraceEventCache eventCache, String source, TraceEventType eventType, int id, string format, params object[] args)
        {
            if (Filter != null && !Filter.ShouldTrace(eventCache, source, eventType, id, format, args, null, null))
                return;

            var traceLog = new TraceLog();
            var eventTimeStamp = DateTime.Now;           

            WriteHeader(source, eventType, id, eventCache, eventTimeStamp, traceLog);

            string message = String.Format(CultureInfo.InvariantCulture, format, args);;

            WriteData(message, traceLog);

            WriteFooter(eventCache, traceLog);

            Publish(traceLog);
        }

        public override void TraceEvent(TraceEventCache eventCache, String source, TraceEventType eventType, int id, string message)
        {
            if (Filter != null && !Filter.ShouldTrace(eventCache, source, eventType, id, message, null, null, null))
                return;

            var traceLog = new TraceLog();

            WriteHeader(source, eventType, id, eventCache, DateTime.Now, traceLog);
            WriteData(message, traceLog);
            WriteFooter(eventCache, traceLog);

            Publish(traceLog);
        }

        public override void TraceData(TraceEventCache eventCache, String source, TraceEventType eventType, int id, object data)
        {
            if (Filter != null && !Filter.ShouldTrace(eventCache, source, eventType, id, null, null, data, null))
                return;

            var traceLog = new TraceLog();

            WriteHeader(source, eventType, id, eventCache, DateTime.Now, traceLog);
            
            if (data != null)
            {
                WriteData(data, traceLog);
            }

            WriteFooter(eventCache, traceLog);

            Publish(traceLog);
        }

        public override void TraceData(TraceEventCache eventCache, String source, TraceEventType eventType, int id, params object[] data)
        {
            if (Filter != null && !Filter.ShouldTrace(eventCache, source, eventType, id, null, null, null, data))
                return;

            var traceLog = new TraceLog();

            WriteHeader(source, eventType, id, eventCache, DateTime.Now, traceLog);
            WriteFooter(eventCache, traceLog);

            var expando = AddDynamicProperties(data, traceLog);

            Publish(expando);
        }

        private IDictionary<string, object> AddDynamicProperties(object[] data, TraceLog traceLog)
        {
            var expando = traceLog.ToExpando() as IDictionary<string, object>;
            if (data != null)
            {
                for (int i = 0; i < data.Length; i++)
                {
                    if (data[i] != null && data[i].GetType() == typeof (KeyValuePair<string, object>))
                    {
                        var kvp = (KeyValuePair<string, object>) data[i];
                        expando.Add(kvp.Key, kvp.Value);
                    }
                    else
                    {
                        WriteData(data[i], traceLog);
                    }
                }
            }
            return expando;
        }

        public override void TraceTransfer(TraceEventCache eventCache, String source, int id, string message, Guid relatedActivityId)
        {
            var traceLog = new TraceLog();
            
            WriteHeader(source, TraceEventType.Transfer, id, eventCache, relatedActivityId, DateTime.Now, traceLog);
            WriteData(message, traceLog);
            WriteFooter(eventCache, traceLog);

            Publish(traceLog);
        }

        internal bool IsEnabled(TraceOptions opts)
        {
            return (opts & TraceOutputOptions) != 0;
        }

        private void Publish(IDictionary<string, object> log)
        {
            try
            {
                publisher.Publish(log);
            }
            catch(Exception exp)
            {
                fallbackSource.TraceData(TraceEventType.Error, 9883, exp);
            }
        }

        private void Publish(TraceLog log)
        {
            try
            {
                publisher.Publish(log);
            }
            catch (Exception exp)
            {
                fallbackSource.TraceData(TraceEventType.Error, 9883, exp);
            }
        }        

        private void WriteHeader(String source, TraceEventType eventType, int id, TraceEventCache eventCache, Guid relatedActivityId, DateTime eventTimeStamp, TraceLog log)
        {
            WriteHeaderDetail(source, eventType, id, eventCache, log);
            log.RelatedActivityId = relatedActivityId.ToString("B");
        }

        private void WriteHeader(String source, TraceEventType eventType, int id, TraceEventCache eventCache, DateTime eventTimeStamp, TraceLog log)
        {
            WriteHeaderDetail(source, eventType, id, eventCache, log);
        }

        private void WriteData(object data, TraceLog log)
        {
            log.Data.Add(data);
            log.Message = data;
        }

        private void WriteHeaderDetail(String source, TraceEventType eventType, int id, TraceEventCache eventCache, TraceLog log)
        {            
            var sev = (int)eventType;
            if (sev > 255)
                sev = 255;
            if (sev < 0)
                sev = 0;

            log.Component = componentName;
            log.Host = GetHostHeader();
            log.UrlLocalPath = GetRequestLocalPath();
            log.EventId = ((uint) id);
            log.Type = eventType.ToString();
            log.Severity = sev;
            log.TimeCreated = DateTime.Now; ;
            log.SourceName = source;
            log.CorrelationActivityId = eventCache != null ? System.Diagnostics.Trace.CorrelationManager.ActivityId.ToString() : Guid.Empty.ToString("B");
            log.MachineName = machineName;
            log.RequestId = GetRequestId();

            WriteFooter(eventCache, log);
        }

        private Guid GetRequestId()
        {
            if (HttpContext.Current == null || !HttpContext.Current.Items.Contains(requestIdKey))
                return Guid.Empty;

            var guid = HttpContext.Current.Items[requestIdKey];

            if (guid is Guid)
                return (Guid)HttpContext.Current.Items[requestIdKey];

            return Guid.Empty;
        }

        private string GetHostHeader()
        {
            if (HttpContext.Current == null || HttpContext.Current.Request == null)
                return string.Empty;
            
            return HttpContext.Current.Request.Headers.GetValues("HOST").FirstOrDefault();
        }

        private string GetRequestLocalPath()
        {
            if (HttpContext.Current == null || HttpContext.Current.Request == null || HttpContext.Current.Request.Url == null)
                return string.Empty;

            return HttpContext.Current.Request.Url.LocalPath;
        }

        private void WriteFooter(TraceEventCache eventCache, TraceLog log)
        {
            bool writeLogicalOps = IsEnabled(TraceOptions.LogicalOperationStack);
            bool writeCallstack = IsEnabled(TraceOptions.Callstack);

            if (eventCache != null && (writeLogicalOps || writeCallstack))
            {
                if (writeLogicalOps)
                {
                    var s = eventCache.LogicalOperationStack;

                    if (s != null)
                    {
                        foreach (var correlationId in s)
                        {
                            log.LogicalOperation = correlationId.ToString();
                        }
                    }
                }
                
                log.EventCacheTimestamp = eventCache.Timestamp;

                if (writeCallstack)
                    log.CallStack = eventCache.Callstack;
            }
        }
    }
}
