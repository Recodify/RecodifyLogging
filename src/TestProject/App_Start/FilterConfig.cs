﻿using Recodify.Logging.Mvc;
using Recodify.Logging.Trace;
using Recodify.Logging.Web;
using System.Web.Mvc;

namespace TestProject
{
	public class FilterConfig
	{
		public static void RegisterGlobalFilters(GlobalFilterCollection filters)
		{
			filters.Add(new HandleErrorAttribute());
			filters.Add(
				new LogFilter(new TraceSource("requestTraceSource"),
				new TraceSource("responseTraceSource"), 
				new Recodify.Logging.Web.HttpContext(),
				new Options()));
		}
	}
}