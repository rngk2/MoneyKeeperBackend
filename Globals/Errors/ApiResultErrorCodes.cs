using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyKeeper.Globals.Errors
{
	public enum ApiResultErrorCodes
	{
		USER_IS_MISSING,
		ALREADY_EXISTS,
		NOT_FOUND,
		CANNOT_UPDATE,
		CANNOT_DELETE
	}
}
