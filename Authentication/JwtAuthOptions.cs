using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyKeeper.Authentication
{
	public sealed class JwtAuthOptions
	{
		public static readonly int TTL_MINUTES = 5;
		public static readonly string AUDIENCE = "MK_Audience";
		public static readonly string ISSUER = "MK_Issuer";
	}
}
