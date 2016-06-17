using Recodify.Logging.Trace;
using Recodify.Logging.Web;
using System;
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
		private const string fallbackKey = "Fallback";

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
			var fallbackTraceSource = new System.Diagnostics.TraceSource(fallbackKey);

			try
			{
				// Request
				if (options.ExcludeUrls.Any(x => request.RequestUri.AbsoluteUri.ToLower().Contains(x)))
				{
					return await base.SendAsync(request, cancellationToken);
				}
			}
			catch (Exception exp)
			{
				fallbackTraceSource.TraceData(TraceEventType.Error, 9883, exp);
				return await base.SendAsync(request, cancellationToken);
			}

			var sw = new Stopwatch();
			sw.Start();

			var requestContent = await request.Content.ReadAsStringAsync();

			requestTraceSource.TraceRequest(request.Method.ToString(), request.Headers.ToString(), requestContent, context.GetFullUrlWithMethod(), context.GetClientIp(), context.GetSessionId());

			// Response
			var response = await base.SendAsync(request, cancellationToken);
			
			if (response == null || response.Content == null)
				return response;
						
			var responseContent = await response.Content.ReadAsStringAsync();

			try
			{
				sw.Stop();
				var statusCode = (int)response.StatusCode;
				var responseHeaders = response.Content.Headers.ToString() + " " + response.Headers.ToString();

				responseTraceSource.TraceResponse((int)response.StatusCode, responseHeaders, responseContent, sw.ElapsedMilliseconds, context.GetFullUrlWithMethod(), context.GetSessionId());
			}
			catch (Exception exp)
			{			
				fallbackTraceSource.TraceData(TraceEventType.Error, 9883, exp);
			}

			return response;
		}
	}
}