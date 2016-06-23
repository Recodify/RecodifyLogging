using Recodify.Logging.Trace;
using Recodify.Logging.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Threading;

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
			if (options.ExcludeUrls.Any(x => request.RequestUri.AbsoluteUri.ToLower().Contains(x)))
			{
				return await base.SendAsync(request, cancellationToken);
			}

			var fallbackTraceSource = new System.Diagnostics.TraceSource(fallbackKey);
			var sw = new Stopwatch();
			sw.Start();			

			await LogRequest(request, fallbackTraceSource);
			
			return await LogResponse(request, cancellationToken, fallbackTraceSource, sw);
		}

		private async Task LogRequest(HttpRequestMessage request, System.Diagnostics.TraceSource fallbackTraceSource)
		{
			var requestContent = await request.Content.ReadAsStringAsync();

			try
			{
				var requestHeaders = request.Headers.ToDictionary(k => k.Key, v => v.Value);
				requestTraceSource.TraceRequest(
					request.Method.ToString(), 
					GetRequestHeaders(request),
					requestContent);
			}
			catch (Exception exp)
			{
				fallbackTraceSource.TraceData(TraceEventType.Error, (int)Event.LoggingExceptionFallingBack, exp);
			}
		}

		private async Task<HttpResponseMessage> LogResponse(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken, System.Diagnostics.TraceSource fallbackTraceSource, Stopwatch sw)
		{
			var response = await base.SendAsync(request, cancellationToken);

			if (response == null || response.Content == null)
				return response;

			var responseContent = await response.Content.ReadAsStringAsync();

			try
			{
				sw.Stop();
				var statusCode = (int)response.StatusCode;
				var responseHeaders = response.Content.Headers.ToString() + " " + response.Headers.ToString();

				responseTraceSource.TraceResponse(
					(int)response.StatusCode, 
					responseHeaders,
					responseContent,
					sw.ElapsedMilliseconds);
			}
			catch (Exception exp)
			{
				fallbackTraceSource.TraceData(TraceEventType.Error, (int)Event.LoggingExceptionFallingBack, exp);
			}

			return response;
		}

		private static string GetRequestHeaders(HttpRequestMessage request)
		{
			var headers = request.Headers.ToDictionary(k => k.Key, v => v.Value.Aggregate((c, n) => c + "," + n));
			return GetObjectContent(headers);
		}

		private static string GetObjectContent(object obj, ReferenceLoopHandling referenceLoopHandling = ReferenceLoopHandling.Ignore)
		{
			var jsonSettings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver(), ReferenceLoopHandling = referenceLoopHandling };
			return JsonConvert.SerializeObject(obj, Formatting.Indented, jsonSettings);
		}
	}
}