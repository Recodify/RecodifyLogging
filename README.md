Collinson Logging Web API
=========================

This library can be used to add logging to existing Web API 2.2 projects. 

Included is `LogHandler`, an implementation of a `DelegatingHandler` that provides trace logging for Web API requests/responses.

An implementation of an `ITraceSource` (based on `System.Diagnostics.TraceSource`) is required for both request and response.

Getting Started
---------------

To begin using:

1. `Install-Package Collinson.Logging.WebApi`
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

See: http://collinson.nuget.latitude-dev.local/packages/Collinson.Logging.WebApi/