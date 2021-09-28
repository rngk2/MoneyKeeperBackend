using BL.Dtos.User;
using Enity = DAL.Entities;

namespace BL.Extensions
{
    public static class UserExtensions
    {
        public static User AsDto(this Enity.User user) => new User
		{
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
        };

        public static Enity.User AsEntity(this User user) => new Enity.User
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
        };
    }
}
