using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyKeeper.Globals.Errors
{
	public class ApiResultErrorCodes
	{
		public static readonly string USER_IS_MISSING = "USER_IS_MISSING";
		public static readonly string ALREADY_EXISTS = "ALREADY_EXISTS";
		public static readonly string NOT_FOUND = "NOT_FOUND";
		public static readonly string PROHIBITED = "PROHIBITED";
		public static readonly string INVALID_REFRESH_TOKEN = "INVALID_REFRESH_TOKEN";
		public static readonly string CANNOT_UPDATE = "CANNOT_UPDATE";
		public static readonly string CANNOT_DELETE = "CANNOT_DELETE";
	}
}
