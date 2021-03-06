﻿using Recodify.Logging.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Recodify.Logging.Trace
{
	public class TraceSource : ITraceSource
	{
		private readonly System.Diagnostics.TraceSource traceSource;
		private readonly System.Diagnostics.TraceSource fallbackTraceSource;
		private const string fallbackKey = "Fallback";
		private readonly IEnricher enricher;

		public TraceSource(string name)
			: this (new System.Diagnostics.TraceSource(name), new NullEnricher())
		{			
			
		}

		public TraceSource(string name, IEnricher enricher)
			: this(new System.Diagnostics.TraceSource(name), enricher)
		{

		}


		public TraceSource(System.Diagnostics.TraceSource traceSource, IEnricher enricher)
		{
			this.traceSource = traceSource;			
			fallbackTraceSource = new System.Diagnostics.TraceSource(fallbackKey);
			this.enricher = enricher;
		}

		public virtual void TraceData(TraceEventType eventType, int id, object data)
		{
			Execute(() =>
			{
				var args = enricher.Enrich(new[] { data }, true);
				traceSource.TraceData(eventType, id, args);
			});
		}

		public virtual void TraceData(TraceEventType eventType, int id, params object[] data)
		{
			Execute(() =>
			{
				data = enricher.Enrich(data);
				traceSource.TraceData(eventType, id, data);
			});
		}

		public virtual void TraceEvent(TraceEventType eventType, int id, string message)
		{
			Execute(() =>
			{
				var args = enricher.Enrich(new[] { message }, true);
				traceSource.TraceData(eventType, id, args);
			});
		}

		public virtual void TraceEvent(TraceEventType eventType, int id, string format, params object[] args)
		{
			Execute(() => 
			{
				args = enricher.Enrich(args);
				traceSource.TraceData(eventType, id, args);
			});
		}

		public virtual void TraceInformation(string message)
		{
			Execute(() =>
			{
				var args = enricher.Enrich(new[] { message }, true);
				traceSource.TraceData(TraceEventType.Information, (int)Event.Information, args);
			});
		}

		public virtual void TraceInformation(string format, params object[] args)
		{
			Execute(() =>
			{
				args = enricher.Enrich(args);
				traceSource.TraceInformation(format, args);
			});
		}

		public virtual void TraceTransfer(int id, string message, Guid relatedActivityId)
		{
			Execute(() => traceSource.TraceTransfer(id, message, relatedActivityId));
		}

		public virtual void TraceRequest(string requestMethod, string headers, string content)
		{
			Execute(() =>
			{
				TraceData(
				   TraceEventType.Information,
				   (int)EventId.RequestReceived,
				   new KeyValuePair<string, object>("httpMethod", requestMethod),
				   new KeyValuePair<string, object>("headers", headers),
				   new KeyValuePair<string, object>("message", content),
				   new KeyValuePair<string, object>("tags", new[] { "request", "http" }));
			});
		}

		public virtual void TraceResponse(int statusCode, string headers, string content, long timing)
		{
			Execute(() =>
			{
				if (statusCode < 399)
				{
					RaiseTraceResponse(TraceEventType.Information, statusCode, headers, content, timing);
				}
				else if (statusCode >= 400 && statusCode <= 499)
				{
					RaiseTraceResponse(TraceEventType.Warning, statusCode, headers, content, timing);
				}
				else if (statusCode > 499)
				{
					RaiseTraceResponse(TraceEventType.Error, statusCode, headers, content, timing);
				}
			});
		}

		public void Close()
		{
			Execute(() =>
			{
				this.traceSource.Close();
				this.fallbackTraceSource.Close();
			});
		}

		protected void Execute(Action action)
		{
			try
			{
				action();
			}
			catch (Exception exp)
			{
				fallbackTraceSource.TraceData(TraceEventType.Error, (int)Event.LoggingExceptionFallingBack, exp);
			}
		}

		private void RaiseTraceResponse(TraceEventType eventType, int statusCode, string headers, string content, long timing)
		{
			TraceData(
				eventType,
				statusCode,				
				new KeyValuePair<string, object>("responseTime", timing),				
				new KeyValuePair<string, object>("headers", headers),
				new KeyValuePair<string, object>("message", content),
				new KeyValuePair<string, object>("statusCode", statusCode),
				new KeyValuePair<string, object>("tags", new[] { "response", "http" }));
		}
	}
}
