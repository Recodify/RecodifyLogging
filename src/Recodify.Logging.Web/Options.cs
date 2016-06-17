using System.Collections.Generic;

namespace Recodify.Logging.Web
{
    public interface IOptions
    {
        IEnumerable<string> ExcludeUrls { get; set; }
    }

    public class Options : IOptions
    {
        public Options()
        {
            this.ExcludeUrls = new List<string>();
        }

        public IEnumerable<string> ExcludeUrls { get; set; }        
    }
}
