using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using BL.Dtos.User;
using BL.Extensions;
using DAL.Entities;
using DAL.Models;
using DAL.Repositories;
using MoneyKeepeer.Utils.Extensions;
using MoneyKeeper.Api.Results;
using MoneyKeeper.Globals.Errors;
using MoneyKeeper.Utils.Extensions;

namespace BL.Services
{
	internal class UserService : IUserService
	{
		private readonly IUsersRepository usersRepository;
		private readonly ICategoriesRepository categoriesRepository;

		public UserService(IUsersRepository usersRepository, ICategoriesRepository categoriesRepository)
		{
			this.usersRepository = usersRepository;
			this.categoriesRepository = categoriesRepository;
		}

		public async Task<Result<User>> GetUser(int id)
		{
			var user = await usersRepository.GetUser(id);
			return user is not null
				? user
				: new Error(ApiResultErrorCodes.NOT_FOUND, $"Cannot find user with id: {id}");
		}

		public async Task<Result<User>> GetUser(string email)
		{
			var user = await usersRepository.GetUser(email);
			return user is not null
				? user
				: new Error(ApiResultErrorCodes.NOT_FOUND, $"Cannot find user with email: {email}");
		}


		public async Task<Result<IEnumerable<SummaryUnit>>> GetSummaryForUser(int id)
		{
			return new SuccessResult<IEnumerable<SummaryUnit>>(await usersRepository.GetSummaryForUser(id));
		}

		public async Task<Result<Dictionary<string, decimal>>> GetTotalForUserForYear(int id)
		{
			return ComputeTotal(await usersRepository.GetSummaryForUser_ForYear(id));
		}

		public async Task<Result<Dictionary<string, decimal>>> GetTotalForUserForMonth(int id)
		{
			return ComputeTotal(await usersRepository.GetSummaryForUser_ForMonth(id));
		}

		private static Dictionary<string, decimal> ComputeTotal(IEnumerable<SummaryUnit> summaryUnits)
		{
			Dictionary<string, decimal> computed = new();
			foreach (var unit in summaryUnits)
			{
				decimal unitAmount = unit.Amount;
				decimal contained = computed.GetValueOrDefault(unit.CategoryName);
				decimal newVal = contained is default(decimal) ? unitAmount : contained + unitAmount;

				computed[unit.CategoryName] = newVal;
			}

			return computed;
		}

		public async Task<Result<User>> CreateUser(CreateUserDto userDto)
		{
			if (await GetUser(userDto.Email) is not null)
			{
				return new Error(ApiResultErrorCodes.ALREADY_EXISTS, $"Already have user with email: {userDto.Email}");
			}

			User newUser = new()
			{
				FirstName = userDto.FirstName,
				LastName = userDto.LastName,
				Email = userDto.Email,
				Password = userDto.Password.Hash()
			};

			int createdId = await usersRepository.CreateUser(newUser);

			await AddDefaultCategoriesToUser(createdId);

			User created = newUser with
			{
				Id = createdId
			};

			return created;
		}

		private async Task AddDefaultCategoriesToUser(int userId)
		{
			foreach (var defaultCategoryName in ICategoryService.DEFAULT_CATEGORIES_NAMES)
			{
				await categoriesRepository.CreateCategory(new()
				{
					Name = defaultCategoryName,
					UserId = userId
				});
			}
		}



		public async Task<Result<User>> UpdateUser(int id, [NotNull] UpdateUserDto userDto)
		{
			Func<object, object, object> returnDefaultIfNull = (object nullable, object @default)
				=> nullable is null ? @default : nullable;

			var (existingUser, error) = await GetUser(id).Unwrap();

			if (error)
			{
				return error.Wrap();
			}

			User updatedUser = existingUser with
			{
				FirstName = returnDefaultIfNull(userDto.FirstName, existingUser.FirstName).ToString(),
				LastName = returnDefaultIfNull(userDto.LastName, existingUser.LastName).ToString(),
				Email = returnDefaultIfNull(userDto.Email, existingUser.Email).ToString(),
				Password = returnDefaultIfNull(userDto.Password?.Hash(), existingUser.Password).ToString()
			};

			return await usersRepository.UpdateUser(updatedUser)
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
