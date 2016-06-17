using Recodify.Logging.Trace;
using Recodify.Logging.Web;
using Recodify.Logging.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace TestProject
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

			config.MessageHandlers.Add(new LogHandler(new TraceSource("requestTraceSource"),
				new TraceSource("responseTraceSource"),
				new Recodify.Logging.Web.HttpContext(),
				new Options()));
        }
    }
}
