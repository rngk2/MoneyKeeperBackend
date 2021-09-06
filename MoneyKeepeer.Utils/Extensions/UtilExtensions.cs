using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyKeepeer.Utils.Extensions
{
	public static class UtilExtensions
	{
		public static bool IsAny<T>(this IEnumerable<T> data)
		{
			return data != null && data.Any();
		}
	}
}
