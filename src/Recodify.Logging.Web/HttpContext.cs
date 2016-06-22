using Microsoft.Owin;
using System.Diagnostics;
using System.Net.Http;
using System.Web;

namespace Recodify.Logging.Common
{
	public class HttpContext : IContext
	{
		private readonly UrlContextResolver contextResolver;

		public HttpContext()
		{
			contextResolver = new UrlContextResolver();
		}

		public string GetContextString()
		{
			if (System.Web.HttpContext.Current == null)
			{
				return string.Empty;
			}

			var context = GetUrlContext();

			return $"http.{System.Web.HttpContext.Current.Request.HttpMethod}.{context}";
		}

		public string GetFullUrlWithMethod()
		{			
			var currentContext = System.Web.HttpContext.Current;
			if (currentContext == null)
			{
				return "Unable to determine url";
			}

			return string.Format("{0} {1}", currentContext.Request.HttpMethod, currentContext.Request.Url);
		}

		public string GetSessionId()
		{
			var currentContext = System.Web.HttpContext.Current;
			if (currentContext == null)
			{
				return string.Empty;
			}
			
			var sessionid = currentContext.Request.Headers["x-sessionid"];
			if (sessionid == null)
			{
				sessionid = currentContext.Items["x-sessionid"]?.ToString();
			}

			return sessionid ?? string.Empty;
		}

		public string GetClientIp()
		{		
			if (IsLegacyRequestContext())
			{
				var context = GetLegacyContext();
				return context.Request.UserHostAddress;
			}
			else
			{
				var context = GetOwinContext();				
				return context.Request.RemoteIpAddress;
			}
		}

		private string GetUrlContext()
		{
			return contextResolver.Resolve(System.Web.HttpContext.Current.Request.Url.LocalPath, System.Web.HttpContext.Current.Request.HttpMethod);
		}

		private bool IsLegacyRequestContext()
		{
			if (System.Web.HttpContext.Current != null && System.Web.HttpContext.Current.Request != null && System.Web.HttpContext.Current.Items["owin.Environment"] == null)
				return true;

			return false;
		}

		private System.Web.HttpContext GetLegacyContext()
		{
			return System.Web.HttpContext.Current;
		}

		private IOwinContext GetOwinContext()
		{
		
			return System.Web.HttpContext.Current.GetOwinContext();
		}
	}
}