using System.Collections.Generic;
using System.Threading.Tasks;
using DAL.Entities;

namespace DAL.Repositories.Categories
{
	public interface ICategoriesRepository
	{
		Task<Category> GetCategory(int id);

		Task<Category> GetCategory(int userId, string categoryName);

		Task<IEnumerable<Category>> GetCategories(int userId);

		Task<int> CreateCategory(Category category);

		Task<bool> UpdateCategory(Category category);

		Task<bool> DeleteCategory(int id);
	}
}
