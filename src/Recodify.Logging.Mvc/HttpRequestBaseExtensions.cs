using System.IO;
using System.Web;

namespace Recodify.Logging.Mvc
{

	/// <summary>
	/// Extension methods for HTTP Request.
	/// <remarks>
	/// See the HTTP 1.1 specification http://www.w3.org/Protocols/rfc2616/rfc2616.html
	/// for details of implementation decisions.
	/// </remarks>
	/// </summary>
	public static class HttpRequestExtensions
	{	
		public static string ToRaw(this HttpRequestBase request)
		{
			StringWriter writer = new StringWriter();

			WriteStartLine(request, writer);
			WriteHeaders(request, writer);
			WriteBody(request, writer);

			return writer.ToString();
		}

		private static void WriteStartLine(HttpRequestBase request, StringWriter writer)
		{
			const string SPACE = " ";

			writer.Write(request.HttpMethod);
			writer.Write(SPACE + request.Url);
			writer.WriteLine(SPACE + request.ServerVariables["SERVER_PROTOCOL"]);
		}

		private static void WriteHeaders(HttpRequestBase request, StringWriter writer)
		{
			foreach (string key in request.Headers.AllKeys)
			{
				writer.WriteLine(string.Format("{0}: {1}", key, request.Headers[key]));
			}

			writer.WriteLine();
		}

		private static void WriteBody(HttpRequestBase request, StringWriter writer)
		{
			StreamReader reader = new StreamReader(request.InputStream);

			try
			{
				string body = reader.ReadToEnd();
				writer.WriteLine(body);
			}
			finally
			{
				reader.BaseStream.Position = 0;
			}
		}
	}
}
