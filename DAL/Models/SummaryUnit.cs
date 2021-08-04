﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
	public record SummaryUnit
	{
		public int UserId { get; init; }

		public string CategoryName { get; init; }

		public decimal Amount { get; init; }

		public DateTime Timestamp { get; init; }
	}
}