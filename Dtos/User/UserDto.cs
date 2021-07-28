using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoneyKeeper.Dtos
{
	public record UserDto
	{
		public int Id { get; init; }

		public string FirstName { get; init; }

		public string LastName { get; init; }

		public string Email { get; init; }
	}
}
