using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Dtos.Transaction
{
	public record CreateTransactionDto
	{
		[Required]
		public int CategoryId { get; init; }

		[Required]
		public decimal Amount { get; init; }

	}
}
