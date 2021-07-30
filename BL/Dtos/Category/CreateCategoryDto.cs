using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BL.Dtos.Category
{
	public record CreateCategoryDto
	{
		[Required]
		public string Name { get; init; }

		[Required]
		public int UserId { get; init; }
	}
}
