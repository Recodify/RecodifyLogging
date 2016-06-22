using System.Collections.Generic;
using System.Collections.Specialized;

namespace Recodify.Logging.Common
{
	public static class NameValueCollectionExtensions
	{
		public static IDictionary<string, string> ToDictionary(this NameValueCollection col)
		{
			var dict = new Dictionary<string, string>();
			foreach (var k in col.AllKeys)
			{
				dict.Add(k, col[k]);
			}
			return dict;
		}
	}
}
