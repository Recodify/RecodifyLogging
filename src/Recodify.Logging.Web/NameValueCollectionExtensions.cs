using System.Collections.Generic;
using System.Collections.Specialized;

namespace Recodify.Logging.Common
{
	public static class NameValueCollectionExtensions
	{
		public static IDictionary<string, IEnumerable<string>> ToDictionary(this NameValueCollection col)
		{
			var dict = new Dictionary<string, IEnumerable<string>>();
			foreach (var k in col.AllKeys)
			{
				dict.Add(k, new[] { col[k] });
			}
			return dict;
		}
	}
}
