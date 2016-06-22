using Recodify.Logging.Trace;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace TestProject.Controllers
{
	public class MyModel
	{
		public string JohnStinks { get; set; }		
	}
	public class HomeController : Controller
	{
		public ActionResult Index()
		{			
			new TraceSource("diagTraceSource").TraceData(System.Diagnostics.TraceEventType.Information, 100, "A message");
			return View(new MyModel { JohnStinks = "a lot" });
		}

		public ActionResult About()
		{
			
			ViewBag.Message = "Your application description page.";

			return View();
		}

		public ActionResult Contact()
		{
			ViewBag.Message = "Your contact page.";

			return View();
		}
	}
}