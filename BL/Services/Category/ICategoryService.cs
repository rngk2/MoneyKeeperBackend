using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using MoneyKeeper.BL.Dtos.Category;
using MoneyKeeper.Utils.Results;
using MoneyKeeper.DAL.Models;

namespace MoneyKeeper.BL.Services
{
	public interface ICategoryService
	{
		static readonly IReadOnlyCollection<string> DEFAULT_CATEGORIES_NAMES = new[] {
				"Earnings"
		};

		Task<Result<IEnumerable<Category>>> GetCategoriesOfUser(int userId);

		Task<Result<Category>> GetCategory(int id, int userId);

		Task<Result<Category>> GetCategory(int userId, string name);

		Task<Result<IEnumerable<CategoryOverview>>> GetCategoriesOverview(int userId, Range range);

		Task<Result<CategoryOverview>> GetCategoryOverview(int userId, int categoryId);

		Task<Result<CategoryOverview>> GetEarningsOverview(int userId);

		Task<Result<Category>> AddCategoryToUser(CreateCategory categoryDto);

		Task<Result<Category>> UpdateCategoryToUser(Category existingCategory, UpdateCategory categoryDto);

		Task<Result<Category>> DeleteCategoryToUser(string categoryName, int userId);

		Task<Result<Category>> DeleteCategoryToUser(int categoryId, int userId);
	}
}