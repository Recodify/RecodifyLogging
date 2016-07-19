using System;
using System.Collections.Generic;

namespace Recodify.Logging.Common
{
    public interface IOptions
    {
        IEnumerable<string> ExcludeUrls { get; set; }
		bool LogResponseModel { get; set; }
		double MaximumResponseSize { get; set; }
    }

    public class Options : IOptions
    {
        public Options()
        {
            this.ExcludeUrls = new List<string>();
        }

        public IEnumerable<string> ExcludeUrls { get; set; }

		public bool LogResponseModel { get; set;  }

		public double MaximumResponseSize { get; set; }		
	}
}
