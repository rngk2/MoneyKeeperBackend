using System.Collections.Generic;
using System.Threading.Tasks;
using DAL.Entities;

namespace DAL.Repositories.Categories
{
	public interface ICategoriesRepository
	{
		Task<Category> GetCategory(int id);

		Task<Category> GetCategory(int userId, string categoryName);

		Task<IEnumerable<Category>> GetCategoriesOfUser(int userId);

		Task<IEnumerable<Category>> GetCategories();

		Task<int> AddCategoryToUser(Category category);
		
		Task UpdateCategoryToUser(Category category);
		
		Task DeleteCategoryToUser(int id);

	}
}
