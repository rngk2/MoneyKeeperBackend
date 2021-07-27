using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MoneyKeeper.Dtos;
using MoneyKeeper.Entities;

namespace MoneyKeeper.Extensions
{
	public static class UserExtension
	{
		public static UserDto AsDto(this User user) => new UserDto
        {
            Id        = user.Id,
            FirstName = user.FirstName,
            LastName  = user.LastName,
            Email     = user.Email,
            Password  = user.Password
        };
    }
}
