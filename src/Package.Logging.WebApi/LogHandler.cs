using Recodify.Logging.Trace;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Recodify.Logging.WebApi
{
    public class LogHandler : DelegatingHandler
    {
        private readonly ITraceSource requestTraceSource;
        private readonly ITraceSource responseTraceSource;
        private readonly IContext context;
        private readonly IOptions options;

        public LogHandler(
            ITraceSource requestTraceSource,
            ITraceSource responseTraceSource,
            IContext context,
            IOptions options)
        {
            this.requestTraceSource = requestTraceSource;
            this.responseTraceSource = responseTraceSource;
            this.context = context;
            this.options = options;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {            
            if (options.ExcludeUrls.Any(x => request.RequestUri.AbsoluteUri.ToLower().Contains(x)))
            {
                return await base.SendAsync(request, cancellationToken); 
            }

            var sw = new Stopwatch();
            sw.Start();

            var requestContent = await request.Content.ReadAsStringAsync();

            requestTraceSource.TraceData(
                TraceEventType.Information,
                (int)EventId.RequestReceived,                
                new KeyValuePair<string, object>("method", request.Method),
                new KeyValuePair<string, object>("requestUrl", context.GetFullUrlWithMethod()),
                new KeyValuePair<string, object>("headers", request.Headers.ToString()),
                new KeyValuePair<string, object>("message", requestContent),
                new KeyValuePair<string, object>("tags", new[] { "request", "http" }),
                new KeyValuePair<string, object>("sessionId", context.GetSessionId()));            

            var response = await base.SendAsync(request, cancellationToken);

            if (response == null)
                return response;

            if (response.Content == null)
                return response;

            string responseContent;
          
            responseContent = await response.Content.ReadAsStringAsync();

            var statusCode = (int)response.StatusCode;

            sw.Stop();

            if (statusCode < 399)
                TraceResponse(TraceEventType.Information, statusCode, response, sw.ElapsedMilliseconds, responseContent);
            else if (statusCode >= 400 && statusCode <= 499)
                TraceResponse(TraceEventType.Warning, statusCode, response, sw.ElapsedMilliseconds, responseContent);
            else if (statusCode > 499)
                TraceResponse(TraceEventType.Error, statusCode, response, sw.ElapsedMilliseconds, responseContent);

            return response;
        }

        private void TraceResponse(TraceEventType eventType, int statusCode, HttpResponseMessage response, long timing, string content)
        {                        
            responseTraceSource.TraceData(
                eventType, 
                statusCode, 
                new KeyValuePair<string, object>("responseTime", timing),
                new KeyValuePair<string, object>("requestUrl", context.GetFullUrlWithMethod()),
                new KeyValuePair<string, object>("headers", response.Content.Headers.ToString() + " " + response.Headers.ToString()),
                new KeyValuePair<string, object>("message", content),
                new KeyValuePair<string, object>("tags", new[] { "response", "http" }),                
                new KeyValuePair<string, object>("sessionId", context.GetSessionId()));
        }
    }
}