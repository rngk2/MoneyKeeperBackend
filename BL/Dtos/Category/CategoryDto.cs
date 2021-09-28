using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoneyKeeper.BL.Dtos.Category
{
	public record Category
	{
		public int Id { get; init; }

		public string Name { get; init; }

		public int UserId { get; init; }
	}
}
