using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BL.Dtos.User
{
	public record UpdateUser
	{
		public string FirstName { get; init; }

		public string LastName { get; init; }

		[EmailAddress]
		public string Email { get; init; }

		[DataType(DataType.Password)]
		[
			RegularExpression("^((?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])|(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[^a-zA-Z0-9])|(?=.*?[A-Z])(?=.*?[0-9])(?=.*?[^a-zA-Z0-9])|(?=.*?[a-z])(?=.*?[0-9])(?=.*?[^a-zA-Z0-9])).{8,16}$",
			ErrorMessage = "Passwords must be 8-16 characters and contain at 3 of 4 of the following: upper case (A-Z), lower case (a-z), number (0-9) and special character (e.g. !@#$%^&*)")
		]
		public string Password { get; init; }
	}
}
