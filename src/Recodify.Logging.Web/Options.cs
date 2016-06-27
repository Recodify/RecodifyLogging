using System;
using System.Collections.Generic;

namespace Recodify.Logging.Common
{
    public interface IOptions
    {
        IEnumerable<string> ExcludeUrls { get; set; }
		bool LogResponse { get; set; }
		double MaximumResposneSize { get; set; }
    }

    public class Options : IOptions
    {
        public Options()
        {
            this.ExcludeUrls = new List<string>();
        }

        public IEnumerable<string> ExcludeUrls { get; set; }

		public bool LogResponse { get; set;  }

		public double MaximumResposneSize { get; set; }		
	}
}
