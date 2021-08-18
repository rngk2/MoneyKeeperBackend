using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Globals.Errors
{
	public enum SqlErrorCodes : UInt32
	{
		DUBLICATE_KEY_ERROR = 2627,
		FK_CONFLICT_ERROR = 547
	}
}
