using BL.Dtos.User;
using DAL.Entities;

namespace BL.Extensions
{
	public static class UserExtension
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
