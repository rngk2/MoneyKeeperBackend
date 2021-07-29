using System.Collections.Generic;
using System.Threading.Tasks;
using MoneyKeeper.Dtos.Category;
using MoneyKeeper.Entities;

namespace MoneyKeeper.Services
{
	public interface ICategoryService
	{
		Task<Category> AddCategoryToUser(CreateCategoryDto categoryDto);

		Task DeleteCategory(int id);

		Task<IEnumerable<Category>> GetCategoriesOfUser(int userId);

		Task<IEnumerable<Category>> GetCategories();

		Task<Category> GetCategory(int id);

		Task UpdateCategoryToUser(Category existingCategory, UpdateCategoryDto categoryDto);

		Task<Category> GetCategory(int userId, string name);
	}
}