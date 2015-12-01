using Collinson.Logging.Trace;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;

namespace Collinson.Logging.WebApi
{
    public class LogHandler : DelegatingHandler
    {
        private readonly ITraceSource requestTraceSource;
        private readonly ITraceSource responseTraceSource;
        private readonly IContext context;

        public LogHandler(
            ITraceSource requestTraceSource,
            ITraceSource responseTraceSource,
            IContext context)
        {
            this.requestTraceSource = requestTraceSource;
            this.responseTraceSource = responseTraceSource;
            this.context = context;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            var sw = new Stopwatch();
            sw.Start();

            var content = await request.Content.ReadAsStringAsync();

            requestTraceSource.TraceEvent(TraceEventType.Information, (int)EventId.RequestReceived, "{0} content {1}", request.ToString(), content);

            var response = await base.SendAsync(request, cancellationToken);

            if (response == null)
                return response;

            if (response.Content == null)
                return response;

            string responseContent;
            if (request.RequestUri.AbsoluteUri.ToLower().Contains("content"))
                responseContent = "IMAGE";
            else
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
            var requestUrl = context.GetFullUrlWithMethod();
            var message = string.Format("request url: {0} response: {1} content: {2}", requestUrl, response, content);
            responseTraceSource.TraceData(eventType, statusCode, new KeyValuePair<string, object>("responseTime", timing), new KeyValuePair<string, object>("message", message));
        }
    }
}