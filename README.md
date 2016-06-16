Recodify Logging Listeners
===========================

This library is a set of custom implementations for the existing 'System.Diagnostics.TraceListener'.
Currently included is a RabbitMq listener which sends traces to a message queue for consumption.

Getting Started
---------------

To begin using:

1. Use NuGet to `Install-Package Recodify.Logging.Listeners`
2. Add the following keys to web.config appSettings.  
    <!--  This is a tag used to differentiate logs by environment -->
    <add key="RecodifyLogging:Environment" value="Local" />  
    <!--    This is the connection string to your rabbitMQ instance -->
    <add key="RabbitMqConnectionString" value="host=cascadelogs.cloudapp.net;virtualHost=/;username=publisher;password=snowwhite"/>
3. Add the below configuration to `Web.config`

	<system.diagnostics>
		<trace autoflush="true" />
		<sources>
            <source name="Fallback" switchValue="Information, Error, Warning">
                <listeners>    
                    <!-- This is optional and will log any errors publishing to RabbitMQ and can be useful for diagnosing initial setup -->
                    <add name="FallBackTraceListners" 
                    type="System.Diagnostics.TextWriterTraceListener" 
                    initializeData="TextWriterOutput.log" />    
                    </add>
                </listenrs>
            <source>           
            <source name="YourSourceName" switchValue="Information, Error, Warning">
                <listeners>
                <add name="YourListenerName" type="Recodify.Logging.Listeners.RabbitMq.TraceListener, Recodify.Logging.Listeners.RabbitMq" initializeData="exchangeName,queueName,componentName " />
                </listeners>
            </source>		  
		</sources>
	</system.diagnostics>
    
4. You are now ready to start creating some logs.

   If you are using an IOC container, registered the jobLogger with your traceSourceName:

    container.RegisterInstance(typeof(IJobLogger), new JobLogger("BillingDetails"));
    
   Then simply inject IJobLogger as a dependency e.g:

    public MyClass (IJobLogger jobLogger)
    {
       jobLogger.TraceData(TraceEventType.Information, (int)Event.MyEvent, "Your Message");
    }

   If you are not using an IOC container, just new up a logger:

     var jobLogger = new JobLogger("BillingDetails");
     jobLogger.TraceData(TraceEventType.Information, (int)Event.MyEvent, "Your Message");
     
5. If you are operating in a web environment. You can stamp all logs with the request Id by adding the following to your `Global.asax

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            if (HttpContext.Current != null)
            {
                HttpContext.Current.Items.Add("RequestId", Guid.NewGuid());               
            }
        }


Dependencies
------------

- Castle.Core version="3.2.2"
- Castle.Windsor" version="3.2.1"  
- EasyNetQ" version="0.34.0.279"
- EasyNetQ.Management.Client" version="0.39.2.333"
- Iesi.Collections" version="3.2.0.4000"  
- Newtonsoft.Json" version="7.0.1"  
- RabbitMQ.Client" version="3.3.2"


Recodify.Logging.Trace
=========================

This library wraps an existing `System.Diagnostics.TraceSource` with additional sanitisation functionality.
This alleviates the risk of logging sensitive data such as passwords and ending up in unsafe hands.

Getting Started
---------------

To begin using:

1. Use NuGet to `Install-Package Recodify.Logging.Trace`
2. Wherever an existing `TraceSource` is required you can use `ITraceSource` and inject the concrete implementation `SanitisedTraceSource` or general `TraceSource`

Dependencies
------------

None


Recodify.Logging.WebApi
=========================

This library can be used to add logging to existing Web API 2.2 projects. 

Included is `LogHandler`, an implementation of a `DelegatingHandler` that provides trace logging for Web API requests/responses.

An implementation of an `ITraceSource` (based on `System.Diagnostics.TraceSource`) is required for both request and response.

Getting Started
---------------

To begin using:

1. `Install-Package Recodify.Logging.WebApi`
2. Within your `WebApiConfig.cs` do the following (You can also use the general `TraceSouce` if data sanitisation is not required)
3. See below:

.

    public static partial class WebApiConfig
    {
	    public static void Register(HttpConfiguration config)
		{
			var requestTraceSource = new SanitisedTraceSource("Request", new Sanitiser());
			var responseTraceSource = new SanitisedTraceSource("Response", new Sanitiser());
            config.MessageHandlers.Add(new LogHandler(requestTraceSource, responseTraceSource, new HttpContext()));	
        }
    }

Dependencies
------------

- Newtonsoft.Json version="7.0.1"
- Recodify.Logging.Trace
