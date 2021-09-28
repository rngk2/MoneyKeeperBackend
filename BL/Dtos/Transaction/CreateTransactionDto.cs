﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MoneyKeeper.BL.Dtos.Transaction
{
	public record CreateTransaction
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
