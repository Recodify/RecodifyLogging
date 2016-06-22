using System;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace TestProject
{
	public class MvcApplication : System.Web.HttpApplication
    {
		private const string outputFilterKey = "OutputFilter";

		protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

		protected void Application_BeginRequest(object sender, EventArgs e)
		{
			System.Web.HttpContext.Current.Items.Add("x-sessionid",Guid.NewGuid());
		}


		// This captures the HTML response which is normall overkill for logging with the model representing the state and the html being static.
		/*
		protected void Application_BeginRequest(object sender, EventArgs e)
		{
			var currentResponse = HttpContext.Current.Response;
			var filter = new OutputFilterStream(currentResponse.Filter);
			currentResponse.Filter = filter;
			HttpContext.Current.Items[outputFilterKey] = filter;
		}

		protected void Application_EndRequest(object sender, EventArgs e)
		{
			var outputStream = HttpContext.Current.Items[outputFilterKey] as OutputFilterStream;
			var s = outputStream.ReadStream();
		}
		*/
	}
}
