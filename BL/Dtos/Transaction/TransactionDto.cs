using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Dtos.Transaction
{
	public record TransactionDto
	{
		public int Id { get; init; }

		public int CategoryId { get; init; }

		public decimal Amount { get; init; }

		public DateTimeOffset Timestamp { get; init; }
	}
}
