using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyKeeper.Globals.Errors
{
	public enum SqlErrorCodes : int
	{
		DUBLICATE_KEY_ERROR = 2627,
		FK_CONFLICT_ERROR = 547
	}
}
