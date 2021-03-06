using System.Diagnostics;
using System.Security.Claims;
using System.Security.Principal;
using System.Linq;

namespace Recodify.Logging.Trace
{

	public static class IIdentityExtensions
	{
		public static SerilizableIdentity ToSerializable(this IIdentity identity)
		{			
			if (identity == null)
			{
				return new SerilizableIdentity();
			}

			var result = new SerilizableIdentity
			{
				Name = identity.Name,
				IsAuthenticated = identity.IsAuthenticated,
				AuthenticationType = identity.AuthenticationType
			};

			var claimsIdentity = identity as ClaimsIdentity;
			if (claimsIdentity != null)
			{
				result.Claims = claimsIdentity.Claims.Select(x =>  new { Type = x.Type, Value = x.Value, ValueType = x.ValueType, Issue = x.Issuer, OriginalIssuer = x.OriginalIssuer });
				result.Actor = claimsIdentity.Actor?.ToString();
				result.Label = claimsIdentity.Label;
				result.NameClaimType = claimsIdentity.NameClaimType;
				result.RoleClaimType = claimsIdentity.RoleClaimType;
			}

			return result;
		}
	}
}