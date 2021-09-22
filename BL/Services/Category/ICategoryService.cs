using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using BL.Dtos.Category;
using DAL.Entities;
using MoneyKeeper.Api.Results;
using MoneyKeeper.DAL.Models;

namespace BL.Services
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

		Task<Result<Category>> AddCategoryToUser([NotNull] CreateCategoryDto categoryDto);

		Task<Result<Category>> UpdateCategoryToUser(Category existingCategory, UpdateCategoryDto categoryDto);

		Task<Result<Category>> DeleteCategoryToUser(string categoryName, int userId);

		Task<Result<Category>> DeleteCategoryToUser(int categoryId, int userId);
	}
}