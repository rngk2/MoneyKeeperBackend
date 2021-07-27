using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoneyKeeper.Entities
{
	public record Transaction
	{
		public int Id { get; init; }

		public int CategoryId { get; init; }

		public decimal Amount { get; init; }

		public DateTime Timestamp { get; init; }
	}
}
