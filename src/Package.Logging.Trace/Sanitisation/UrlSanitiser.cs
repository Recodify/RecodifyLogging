using System.Text.RegularExpressions;

namespace Recodify.Logging.Trace.Sanitisation
{
    public class UrlSanitiser : IUrlSanitiser
    {
		private static readonly Regex passwordParameterRegex = new Regex(@"[&\?]password=[^\p{Cc}]*", RegexOptions.IgnoreCase);

        public string Sanitise(string source)
        {
	        return passwordParameterRegex.Replace(source, match => "********");
        }
    }
}