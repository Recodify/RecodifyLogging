using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Recodify.Logging.Trace
{
	public class SerializationHelper
	{
		public static string GetObjectContent(object obj, ReferenceLoopHandling referenceLoopHandling = ReferenceLoopHandling.Ignore)
		{
			var result = "";
			try
			{
				var jsonSettings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver(), ReferenceLoopHandling = referenceLoopHandling };
				result = JsonConvert.SerializeObject(obj, Formatting.Indented, jsonSettings);
			}
			catch (Exception exp)
			{
				result = $"Error serializing object to type {obj?.GetType()?.FullName}";
			}

			return result;
		}
	}
}
