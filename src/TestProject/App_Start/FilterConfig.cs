using Recodify.Logging.Mvc;
using Recodify.Logging.Trace;
using Recodify.Logging.Common;
using System.Web.Mvc;
using Recodify.Logging.Trace.Sanitisation;

namespace TestProject
{
	public class FilterConfig
	{
		public static void RegisterGlobalFilters(GlobalFilterCollection filters)
		{
			filters.Add(new HandleErrorAttribute());
			filters.Add(
				new LogFilter(new SanitisedTraceSource("requestTraceSource", new WebDataEnricher(), new Sanitiser()),
				new SanitisedTraceSource("responseTraceSource", new WebDataEnricher(), new Sanitiser()), 
				new Recodify.Logging.Common.HttpContext(),
				new Options()));
		}
	}
}
