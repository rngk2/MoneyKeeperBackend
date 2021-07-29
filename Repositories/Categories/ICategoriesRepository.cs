using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MoneyKeeper.Dtos.Category;
using MoneyKeeper.Entities;

namespace MoneyKeeper.Repositories.Categories
{
	public interface ICategoriesRepository
	{
		Task<Category> GetCategory(int id);

		Task<Category> GetCategory(int userId, string categoryName);

		Task<IEnumerable<Category>> GetCategoriesOfUser(int userId);

		Task<int> AddCategoryToUser(Category category);
		
		Task UpdateCategoryToUser(Category category);
		
		Task DeleteCategoryToUser(int id);

	}
}
