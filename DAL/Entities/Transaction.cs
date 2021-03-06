using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Entities
{
	public record Transaction
	{
		public int Id { get; init; }

		public int UserId { get; init; }
			
		public int CategoryId { get; init; }

		public string CategoryName { get; init; }

		public decimal Amount { get; init; }

		public DateTimeOffset Timestamp { get; init; }

		public string Comment { get; init; }
	}
}
