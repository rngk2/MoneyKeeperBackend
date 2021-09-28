using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MoneyKeeper.BL.Dtos.Category
{
	public record CreateCategory
	{
		[Required]
		public string Name { get; init; }

		[JsonIgnore]
		public int UserId { get; init; }
	}
}
