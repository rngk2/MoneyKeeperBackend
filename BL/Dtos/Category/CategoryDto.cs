using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BL.Dtos.Category
{
	public record CategoryDto
	{
		public int Id { get; init; }

		public string Name { get; init; }

		public int UserId { get; init; }
	}
}
