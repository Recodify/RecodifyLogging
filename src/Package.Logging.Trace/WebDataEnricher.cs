using Newtonsoft.Json;
using Recodify.Logging.Common;
using System.Collections.Generic;
using System.Linq;

namespace Recodify.Logging.Trace
{
	public class WebDataEnricher
	{
		public object[] Enrich(object[] data, bool preserveOrder = false)
		{
			var httpContext = new HttpContext();
			var dataList = data.ToList();			
			dataList.Insert(preserveOrder ? data.Length : 0, new KeyValuePair<string, object>("requestUrl", httpContext.GetFullUrlWithMethod()));			
			dataList.Add(new KeyValuePair<string, object>("sessionId", httpContext.GetSessionId()));
			dataList.Add(new KeyValuePair<string, object>("clientip", httpContext.GetClientIp()));
			dataList.Add(new KeyValuePair<string, object>("identity", SerializationHelper.GetObjectContent(httpContext.GetIdentity(), ReferenceLoopHandling.Ignore)));
			
			return dataList.ToArray();
		}	
	}
}
