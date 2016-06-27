using NUnit.Framework;
using NUnit.Framework.Internal;
using Recodify.Logging.Common;
using Recodify.Logging.Mvc;
using Recodify.Logging.Trace;
using System;
using System.Diagnostics;
using System.Web;
using System.Web.Mvc;
using System.Collections;
using System.Collections.Specialized;

namespace UnitTests.FullStack
{
	public class TestController : Controller
	{
		public TestController(object model)
		{
			this.ViewData.Model = model;
		}
	}

	public class TestResponse : HttpResponseBase
	{
		public override int StatusCode
		{
			get
			{
				return 200;
			}

			set
			{				
			}
		}

		public override NameValueCollection Headers
		{
			get
			{
				return new NameValueCollection();
			}
		}
	}

	public class TestRequest : HttpRequestBase
	{
		public override Uri Url
		{
			get
			{
				return new Uri("http://blah.com");
			}
		}
	}	

	public class TestContext : HttpContextBase
	{
		public TestContext()
		{
			
		}

		public override IDictionary Items
		{
			get
			{
				return new System.Collections.Hashtable();
			}
		}

		public override HttpRequestBase Request
		{
			get
			{
				return new TestRequest();
			}
		}

		public override HttpResponseBase Response
		{
			get
			{
				return new TestResponse();
			}
		}
	}

	public class SanitisedTraceSourceTests
	{		
		[TestCase(10)]
		[TestCase(100)]
		[TestCase(200)]
		public void DataSizePerformanceTests(int viewModelSize)
		{
			Trace.AutoFlush = true;
			var traceSource = CreateTraceSource();
			var dataModel = new DataFactory().CreateViewModelCollection(viewModelSize);

			Console.WriteLine("View Model Size: " + viewModelSize);

			var filter = new LogFilter(traceSource, traceSource, new Recodify.Logging.Common.HttpContext(), new Options());			

			var action = new Action(() =>filter.OnActionExecuted(new ActionExecutedContext { Controller = new TestController(dataModel), HttpContext = new TestContext() }));

			var test = new PerformanceTest(action, 10, true);

			test.Perform();
        }

		[Test]
		public void Simple()
		{
			var traceSource = CreateTraceSource();
			traceSource.TraceEvent(TraceEventType.Error, 1, "my message");		
		}

		//private System.Diagnostics.TraceSource mySource =  new System.Diagnostics.TraceSource("TraceSourceApp"); 

		private Recodify.Logging.Trace.TraceSource CreateTraceSource()
		{
			var rabbitmqListener =
				new Recodify.Logging.Listeners.RabbitMq.TraceListener("events-live,logs-live,logging-test2");

			var mySource = new System.Diagnostics.TraceSource("TraceSourceApp");
			mySource.Switch = new SourceSwitch("sourceSwitch", "Error");
			mySource.Switch.Level = SourceLevels.All;
			mySource.Listeners.Remove("Default");
			mySource.Listeners.Add(rabbitmqListener);
						
			return new Recodify.Logging.Trace.TraceSource(mySource, new WebDataEnricher());
		}
	}
}
