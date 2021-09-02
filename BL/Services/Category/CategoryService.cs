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

		public async Task<Result<Category>>GetCategory(int id)
		{
			var category = await repository.GetCategory(id);
			return category is not null
				? category
				: new Error(ApiResultErrorCodes.NOT_FOUND.ToString(), $"Cannot find category: #{id}");
		}

		/*public async Task<Result<IEnumerable<Category>>> GetCategories()
		{
			return await repository.GetCategories();
		}*/

		public async Task<Result<IEnumerable<Category>>> GetCategoriesOfUser(int userId)
		{
			return new SuccessResult<IEnumerable<Category>>(await repository.GetCategoriesOfUser(userId));
		}

		public async Task<Result<Category>> AddCategoryToUser(CreateCategoryDto categoryDto)
		{
			if (await GetCategory(categoryDto.UserId, categoryDto.Name) is not null)
			{
				return new Error(ApiResultErrorCodes.ALREADY_EXISTS.ToString(), 
					$"User: {categoryDto.UserId} already has category with name: {categoryDto.Name}");
			}

			Category category = new()
			{
				Name = categoryDto.Name,
				UserId = categoryDto.UserId
			};

			int createdId = await repository.AddCategoryToUser(category);

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

			return await repository.UpdateCategoryToUser(updatedCategory)
				? updatedCategory
				: new Error(ApiResultErrorCodes.CANNOT_UPDATE.ToString(), $"Error occured while updating category: #{existingCategory.Id}");
		}

		/*
		public async Task DeleteCategory(int id)
		{
			await repository.DeleteCategory(id);
		}
		*/

		public async Task<Category> GetCategory(int userId, string name)
		{
			return await repository.GetCategory(userId, name);
		}

		public async Task<Result<Category>> DeleteCategoryToUser(int userId, string name)
		{
			Category toDelete = await GetCategory(userId, name);

			if (toDelete is null) 
			{
				return new Error(ApiResultErrorCodes.NOT_FOUND.ToString(), $"Cannot find category: {name} of user: {userId} to delete");
			}

			return await repository.DeleteCategoryToUser(userId, name)
				? toDelete
				: new Error(ApiResultErrorCodes.CANNOT_DELETE.ToString(), "Error occured while deleting");
		}
	}
}
