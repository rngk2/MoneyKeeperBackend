using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BL.Dtos.Transaction
{
	public record CreateTransactionDto
	{
		[JsonIgnore]
		public int UserId { get; init; }

		[Required]
		public int CategoryId { get; init; }

		[Required]
		public decimal Amount { get; init; }

		[Required]
		public DateTimeOffset Timestamp { get; init; }

		[Required]
		public string Comment { get; init; }

	}
}
