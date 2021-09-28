using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using BL.Dtos.Category;
using BL.Extensions;
using DAL.Repositories;
using MoneyKeeper.Api.Results;
using MoneyKeeper.DAL.Models;
using MoneyKeeper.Globals.Errors;

namespace BL.Services
{
	internal class CategoryService : ICategoryService
	{
		private readonly ICategoriesRepository repository;

		public CategoryService(ICategoriesRepository repository)
		{
			this.repository = repository;
		}

		public async Task<Result<Category>> GetCategory(int userId, string categoryName)
		{
			var category = await repository.GetCategory(userId, categoryName);
			return category is not null
				? category.AsDto()
				: new Error(ApiResultErrorCodes.NOT_FOUND, $"Cannot find category: {categoryName} of user: #{userId}");
		}

		public async Task<Result<Category>> GetCategory(int categoryId, int userId)
		{
			var category = await repository.GetCategory(categoryId);

			return category is not null && category.UserId == userId
				? category.AsDto()
				: new Error(ApiResultErrorCodes.NOT_FOUND, $"Cannot find category: #{categoryId}");
		}

		public async Task<Result<IEnumerable<CategoryOverview>>> GetCategoriesOverview(int userId, Range range)
		{
			return new SuccessResult<IEnumerable<CategoryOverview>>(await repository.GetCategoriesOverview(userId, range));
		}

		public async Task<Result<CategoryOverview>> GetCategoryOverview(int userId, int categoryId)
		{
			var (category, error) = await GetCategory(categoryId, userId).Unwrap();

			if (error)
			{
				return error.Wrap();
			}
			else if (category.UserId != userId)
			{
				return new Error(ApiResultErrorCodes.NOT_FOUND, $"User {userId} has no category with id: {categoryId}");
			}

			return await repository.GetCategoryOverview(categoryId);
		}

		public async Task<Result<IEnumerable<Category>>> GetCategoriesOfUser(int userId)
		{
			return (await repository.GetCategories(userId))
				.Select(c => c.AsDto()).ToList();
		}

		public async Task<Result<CategoryOverview>> GetEarningsOverview(int userId)
		{
			return await repository.GetEarningsOverview(userId);
		}

		public async Task<Result<Category>> AddCategoryToUser(CreateCategory categoryDto)
		{
			if ((await GetCategory(categoryDto.UserId, categoryDto.Name).Unwrap()).Value is not null)
			{
				return new Error(ApiResultErrorCodes.ALREADY_EXISTS,
					$"User: {categoryDto.UserId} already has category with name: {categoryDto.Name}");
			}

			Category category = new()
			{
				Name = categoryDto.Name,
				UserId = categoryDto.UserId
			};

			int createdId = await repository.CreateCategory(category.AsEntity());

			Category created = category with
			{
				Id = createdId
			};

			return created;
		}

		public async Task<Result<Category>> UpdateCategoryToUser(Category existingCategory, UpdateCategory categoryDto)
		{
			Category updatedCategory = existingCategory with
			{
				Name = categoryDto.Name is null ? existingCategory.Name : categoryDto.Name
			};

			return await repository.UpdateCategory(updatedCategory.AsEntity())
				? updatedCategory
				: new Error(ApiResultErrorCodes.CANNOT_UPDATE, $"Error occured while updating category: #{existingCategory.Id}");
		}

		public async Task<Result<Category>> DeleteCategoryToUser(string categoryName, int userId)
		{
			var (toDelete, error) = await GetCategory(userId, categoryName).Unwrap();

			if (error)
			{
				return error.Wrap();
			}

			return await repository.DeleteCategory(toDelete.Id)
				? toDelete
				: new Error(ApiResultErrorCodes.CANNOT_DELETE, "Error occured while deleting");
		}

		public async Task<Result<Category>> DeleteCategoryToUser(int categoryId, int userId)
		{
			var (toDelete, error) = await GetCategory(categoryId, userId).Unwrap();

			if (error)
			{
				return error.Wrap();
			}
			else if (toDelete.UserId != userId)
			{
				return new Error(ApiResultErrorCodes.NOT_FOUND, $"User {userId} has no category with id: {categoryId}");
			}

			return await repository.DeleteCategory(categoryId)
				? toDelete
				: new Error(ApiResultErrorCodes.CANNOT_DELETE, "Error occured while deleting");
		}

	}
}
