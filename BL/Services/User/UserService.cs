using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using MoneyKeeper.BL.Dtos.User;
using MoneyKeeper.BL.Extensions;
using MoneyKeeper.DAL.Repositories;
using Microsoft.Extensions.Caching.Memory;
using MoneyKeeper.Utils.Extensions;
using MoneyKeeper.Utils.Results;
using MoneyKeeper.Globals.Errors;

namespace MoneyKeeper.BL.Services
{
	internal class UserService : IUserService
	{
		private readonly IUsersRepository usersRepository;
		private readonly ICategoriesRepository categoriesRepository;
		private readonly IMemoryCache memoryCache;

		public UserService(IUsersRepository usersRepository, ICategoriesRepository categoriesRepository, IMemoryCache memoryCache)
		{
			this.usersRepository = usersRepository;
			this.categoriesRepository = categoriesRepository;
			this.memoryCache = memoryCache;
		}

		public async Task<Result<User>> GetUser(int id)
		{
			var user = await usersRepository.GetUser(id);
			return user is not null
				? user.AsDto()
				: new Error(ApiResultErrorCodes.NOT_FOUND, $"Cannot find user with id: {id}");
		}

		public async Task<Result<User>> GetUser(string email)
		{
			var user = await usersRepository.GetUser(email);
			return user is not null
				? user.AsDto()
				: new Error(ApiResultErrorCodes.NOT_FOUND, $"Cannot find user with email: {email}");
		}

		public async Task<Result<User>> CreateUser(CreateUser userDto)
		{
			if ((await GetUser(userDto.Email).Unwrap()).Value is not null)
			{
				return new Error(ApiResultErrorCodes.ALREADY_EXISTS, $"Already have a user with email: {userDto.Email}");
			}

			User newUser = new()
			{
				FirstName = userDto.FirstName,
				LastName = userDto.LastName,
				Email = userDto.Email,
				Password = userDto.Password.Hash()
			};

			int createdId = await usersRepository.CreateUser(newUser.AsEntity());

			await AddDefaultCategoriesToUser(createdId);

			User created = newUser with
			{
				Id = createdId
			};

			memoryCache.Set(
				created.Email, 
				created, 
				new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5))
			);

			return created;
		}

		private async Task AddDefaultCategoriesToUser(int userId)
		{
			foreach (var defaultCategoryName in ICategoryService.DefaultCategoriesNames)
			{
				await categoriesRepository.CreateCategory(new()
				{
					Name = defaultCategoryName,
					UserId = userId
				});
			}
		}

		public async Task<Result<User>> UpdateUser(int id, UpdateUser userDto)
		{
			Func<object?, object, object> returnDefaultIfNull = (object? nullable, object @default)
				=> nullable is null ? @default : nullable;

			var (existingUser, error) = await GetUser(id).Unwrap();

			if (error)
			{
				return error.Wrap();
			}

			User updatedUser = existingUser with
			{
				FirstName = returnDefaultIfNull(userDto.FirstName, existingUser.FirstName).ToString()!,
				LastName = returnDefaultIfNull(userDto.LastName, existingUser.LastName).ToString()!,
				Email = returnDefaultIfNull(userDto.Email, existingUser.Email).ToString()!,
				Password = returnDefaultIfNull(userDto.Password?.Hash(), existingUser.Password).ToString()!
			};

			return await usersRepository.UpdateUser(updatedUser.AsEntity())
				? updatedUser
				: new Error(ApiResultErrorCodes.CANNOT_UPDATE, $"Error occured while updating user: #{existingUser.Id}");
		}

		public async Task<Result<User>> DeleteUser(int id)
		{
			var (toDelete, error) = await GetUser(id).Unwrap();

			if (error)
			{
				return error.Wrap();
			}

			return await usersRepository.DeleteUser(id)
				? toDelete
				: new Error(ApiResultErrorCodes.CANNOT_DELETE, $"Error occured while user: #{id}");
		}
	}
}
