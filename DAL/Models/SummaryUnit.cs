using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
	public record SummaryUnit
	{
		public int TransactionId { get; init; }

		public int UserId { get; init; }

		public string CategoryName { get; init; }

		public int CategoryId { get; set; }

		public decimal Amount { get; init; }

		public DateTime Timestamp { get; init; }

		public string Comment { get; init; }
	}
}
