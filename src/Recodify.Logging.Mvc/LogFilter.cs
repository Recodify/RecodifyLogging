using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Recodify.Logging.Trace;
using Recodify.Logging.Common;
using System;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using System.Threading;

namespace Recodify.Logging.Mvc
{
	public class LogFilter : IActionFilter
	{
		private readonly ITraceSource requestTraceSource;
		private readonly ITraceSource responseTraceSource;
		private readonly System.Diagnostics.TraceSource fallbackTraceSource;
		private readonly IContext context;
		private readonly IOptions options;
		private const string outputFilterKey = "OutputFilter";
		private const string timerKey = "Timer";
		private const string fallbackKey = "Fallback";

		public LogFilter(
		   ITraceSource requestTraceSource,
		   ITraceSource responseTraceSource,
		   IContext context,
		   IOptions options)
		{
			this.requestTraceSource = requestTraceSource;
			this.responseTraceSource = responseTraceSource;
			this.context = context;
			this.options = options;
			fallbackTraceSource = new System.Diagnostics.TraceSource(fallbackKey);
		}

		public void OnActionExecuting(ActionExecutingContext filterContext)
		{
			try
			{
				var currentRequest = filterContext.HttpContext.Request;

				if (IsExcluded(currentRequest.Url.ToString())) return;
				
				SetupTimer(filterContext);

				var requestContent = currentRequest.ToRaw();
				requestTraceSource.TraceRequest(
					currentRequest.HttpMethod,
					GetObjectContent(currentRequest.Headers.ToDictionary()), 
					requestContent);
			}
			catch (Exception exp)
			{
				fallbackTraceSource.TraceData(TraceEventType.Error, (int)Event.LoggingExceptionFallingBack, exp);
			}
		}

		public void OnActionExecuted(ActionExecutedContext filterContext)
		{
			try
			{
				var currentRequest = filterContext.HttpContext.Request;
				if (IsExcluded(currentRequest.Url.ToString())) return;
				
				var responseTime = GetResponseTime(filterContext);

				var responseContent = GetModelContent(filterContext);

				var currentResponse = filterContext.HttpContext.Response;
				responseTraceSource.TraceResponse(
					currentResponse.StatusCode,
					currentResponse.Headers.ToString(),
					responseContent,
					responseTime);
			}
			catch (Exception exp)
			{				
				fallbackTraceSource.TraceData(TraceEventType.Error, (int)Event.LoggingExceptionFallingBack, exp);
			}
		}

		private string GetModelContent(ActionExecutedContext filterContext)
		{
			if (!options.LogResponse)
			{
				return string.Empty;
			}

			var responseContent =  GetObjectContent(filterContext.Controller.ViewData.Model);

			if (responseContent.Length > options.MaximumResposneSize)
			{
				return string.Empty;
			}

			return responseContent;
		}

		private static long GetResponseTime(ActionExecutedContext filterContext)
		{
			var timer = filterContext.HttpContext.Items[timerKey] as Stopwatch;
			long responseTime = 0;
			if (timer != null)
			{
				timer.Stop();
				responseTime = timer.ElapsedMilliseconds;
			}

			return responseTime;
		}

		private static void SetupTimer(ActionExecutingContext filterContext)
		{
			var sw = new Stopwatch();
			sw.Start();
			filterContext.HttpContext.Items[timerKey] = sw;
		}

		private static void SetupOutputFilter(ActionExecutingContext filterContext)
		{
			var currentResponse = filterContext.HttpContext.Response;
			var filter = new OutputFilterStream(currentResponse.Filter);
			currentResponse.Filter = filter;
			filterContext.HttpContext.Items[outputFilterKey] = filter;
		}

		private static string GetObjectContent(object obj, ReferenceLoopHandling referenceLoopHandling = ReferenceLoopHandling.Ignore)
		{
			var jsonSettings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver(), ReferenceLoopHandling = referenceLoopHandling};
			return JsonConvert.SerializeObject(obj, Formatting.Indented, jsonSettings);
		}		
	
		private bool IsExcluded(string url)
		{
			return options.ExcludeUrls.Any(x => url.ToLower().Contains(x));			
		}
	}
}