using BL.Dtos.User;
using DAL.Entities;

namespace BL.Extensions
{
	public static class UserExtensions
	{
		public static UserDto AsDto(this User user) => new UserDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
        };
    }
}
