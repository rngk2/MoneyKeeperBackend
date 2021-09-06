using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using BL.Dtos.Category;
using DAL.Entities;
using DAL.Repositories.Categories;
using MoneyKeeper.Api.Results;
using MoneyKeeper.Globals.Errors;

namespace BL.Services
{
	public class CategoryService : ICategoryService
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
				? category
				: new Error(ApiResultErrorCodes.NOT_FOUND, $"Cannot find category: {categoryName} of user: #{userId}");
		}

		public async Task<Result<Category>> GetCategory(int categoryId, int userId)
		{
			var category = await repository.GetCategory(categoryId);

			return category is not null && category.Id == userId
				? category
				: new Error(ApiResultErrorCodes.NOT_FOUND, $"Cannot find category: #{categoryId}");
		}

		public async Task<Result<IEnumerable<Category>>> GetCategoriesOfUser(int userId)
		{
			return new SuccessResult<IEnumerable<Category>>(await repository.GetCategories(userId));
		}

		public async Task<Result<Category>> AddCategoryToUser([NotNull] CreateCategoryDto categoryDto)
		{
			if (await GetCategory(categoryDto.UserId, categoryDto.Name) is not null)
			{
				return new Error(ApiResultErrorCodes.ALREADY_EXISTS,
					$"User: {categoryDto.UserId} already has category with name: {categoryDto.Name}");
			}

			Category category = new()
			{
				Name = categoryDto.Name,
				UserId = categoryDto.UserId
			};

			int createdId = await repository.CreateCategory(category);

			Category created = category with
			{
				Id = createdId
			};

			return created;
		}

		public async Task<Result<Category>> UpdateCategoryToUser([NotNull] Category existingCategory, [NotNull] UpdateCategoryDto categoryDto)
		{
			Category updatedCategory = existingCategory with
			{
				Name = categoryDto.Name is null ? existingCategory.Name : categoryDto.Name
			};

			return await repository.UpdateCategory(updatedCategory)
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

			return await repository.DeleteCategory(userId, categoryName)
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
			else if (toDelete.Id != userId)
			{
				return new Error(ApiResultErrorCodes.NOT_FOUND, $"User {userId} has no category with id: {categoryId}");
			}

			return await repository.DeleteCategory(categoryId)
				? toDelete
				: new Error(ApiResultErrorCodes.CANNOT_DELETE, "Error occured while deleting");
		}

	}
}
