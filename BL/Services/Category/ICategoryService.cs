using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using BL.Dtos.Category;
using DAL.Entities;
using MoneyKeeper.Api.Results;

namespace BL.Services
{
	public interface ICategoryService
	{
		Task<Result<IEnumerable<Category>>> GetCategoriesOfUser(int userId);

		Task<Result<Category>> GetCategory(int id, int userId);

		Task<Result<Category>> GetCategory(int userId, string name);

		Task<Result<Category>> AddCategoryToUser([NotNull] CreateCategoryDto categoryDto);

		Task<Result<Category>> UpdateCategoryToUser(Category existingCategory, UpdateCategoryDto categoryDto);

		Task<Result<Category>> DeleteCategoryToUser(string categoryName, int userId);

		Task<Result<Category>> DeleteCategoryToUser(int categoryId, int userId);
	}
}