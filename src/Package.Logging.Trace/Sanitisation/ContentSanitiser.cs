using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System.Configuration;
using System.Linq;
using System.Diagnostics;
using Microsoft.Azure;

namespace Recodify.Logging.Trace.Sanitisation
{
    public class ContentSanitiser : IContentSanitiser
    {
        private static readonly JsonSerializerSettings CamelCaseSerializerSettings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver(), Formatting = Formatting.Indented };		

        public ContentSanitiser()
        {
            var enumConverter = new StringEnumConverter();
            CamelCaseSerializerSettings.Converters.Add(enumConverter);
        }		

        public string Sanitise(string source)
        {
            try
            {				
	            if (string.IsNullOrWhiteSpace(source))
	            {
		            return source;
	            }

                var deserialized = JToken.Parse(source);
                return Sanitise(deserialized) ? JsonConvert.SerializeObject(deserialized, CamelCaseSerializerSettings) : source;
            }
            catch
            {
                return source;
            }
        }

		private IEnumerable<string> SanitisedFields
		{
			get
			{				
				var sanitisedFields = CloudConfigurationManager.GetSetting("RecodifyLogging: SantisedFields");
				if (string.IsNullOrWhiteSpace(sanitisedFields))
				{
					return new string[] { };
				}

				return sanitisedFields.Split(',').Select(x => x?.ToLower());
			}
		}

        private bool Sanitise(JToken source)
        {
			if (source.Type == JTokenType.Array)
			{
				return Sanitise((IEnumerable<JToken>)source);
			}

	        var jObject = source as JObject;
	        if (jObject != null) return Sanitise(jObject);

	        return false;
        }

        private bool Sanitise(IEnumerable<JToken> source)
        {
            var sanitized = false;
            foreach (var item in source)
            {
                if (Sanitise(item))
                {
                    sanitized = true;
                }
            }
            return sanitized;
        }

        private bool Sanitise(JObject source)
        {
            var sanitized = false;
            foreach (var p in source.Properties())
            {
                if (SanitisedFields.Contains(p.Name.ToLower()) && !p.Value.HasValues)
                {
                    p.Value = "******";
                    sanitized = true;
                }
                if (p.Value.HasValues && Sanitise(p.Value))
                {
                    sanitized = true;
                }
            }
            return sanitized;
        }
    }
}