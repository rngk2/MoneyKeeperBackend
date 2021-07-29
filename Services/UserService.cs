using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MoneyKeeper.Dtos;
using MoneyKeeper.Entities;
using MoneyKeeper.Extensions;
using MoneyKeeper.Repositories;

namespace MoneyKeeper.Services
{
	public class UserService : IUserService
	{
		private readonly IUsersRepository repository;

		public UserService(IUsersRepository usersRepository)
		{
			repository = usersRepository;
		}

		public async Task<IEnumerable<UserDto>> GetUsers()
		{
			return (await repository.GetUsers()).Select(user => user.AsDto());
		}

		public async Task<ActionResult<UserDto>> GetUser(int id)
		{
			var user = await repository.GetUser(id);
			return user?.AsDto();
			//return user is null ? NotFound() : user.AsDto();
		}

		public async Task<UserDto> CreateUser(CreateUserDto userDto)
		{
			User newUser = new()
			{
				FirstName = userDto.FirstName,
				LastName = userDto.LastName,
				Email = userDto.Email,
				Password = userDto.Password.AsSHA256Hash()
			};

			int createdId = await repository.CreateUser(newUser);

			User created = newUser with
			{
				Id = createdId
			};

			return created.AsDto();
		}

		public async Task<ActionResult> UpdateUser(int id, UpdateUserDto userDto)
		{
			Func<object, object, object> returnDefaultIfNull = (object nullable, object @default)
				=> nullable is null ? @default : nullable;

			var existingUser = await repository.GetUser(id);

			if (existingUser is null)
			{
				return new NotFoundResult();
			}

			var userWithEmail = await repository.GetUser(userDto.Email);

			if (userWithEmail != null && userWithEmail.Id != existingUser.Id)
			{
				return new ConflictResult();
			}
			
			User updatedUser = existingUser with
			{
				FirstName = returnDefaultIfNull(userDto.FirstName, existingUser.FirstName).ToString(),
				LastName = returnDefaultIfNull(userDto.LastName, existingUser.LastName).ToString(),
				Email = returnDefaultIfNull(userDto.Email, existingUser.Email).ToString(),
				Password = returnDefaultIfNull(userDto.Password?.AsSHA256Hash(), existingUser.Password).ToString()
			};

			await repository.UpdateUser(updatedUser);

			return new NoContentResult();

		}

		public async Task<ActionResult> DeleteUser(int id)
		{
			await repository.DeleteUser(id);

			return new NoContentResult();
		}
	}
}
