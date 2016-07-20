using System.Collections.Generic;
using System.Security.Claims;

namespace Recodify.Logging.Trace
{

	public class SerilizableIdentity
	{
		public string Name { get; set; }
		public bool IsAuthenticated { get; set; }
		public string AuthenticationType { get; set; }
		public IEnumerable<object> Claims { get; set; }
		public string Actor { get; set; }		
		public string NameClaimType { get; set; }
		public string RoleClaimType { get; set; }
		public string Label { get; set; }
	}

}