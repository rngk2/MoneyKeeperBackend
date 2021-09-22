using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DAL.Entities;
using MoneyKeeper.DAL.Models;

namespace DAL.Repositories
{
	public interface ICategoriesRepository
	{
		Task<Category> GetCategory(int id);

		Task<Category> GetCategory(int userId, string categoryName);

		Task<IEnumerable<CategoryOverview>> GetCategoriesOverview(int userId, Range range);

		Task<CategoryOverview> GetCategoryOverview(int categoryId);

		Task<IEnumerable<Category>> GetCategories(int userId);

		Task<int> CreateCategory(Category category);

		Task<bool> UpdateCategory(Category category);

		Task<bool> DeleteCategory(int id);
	}
}
