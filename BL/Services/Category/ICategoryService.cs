using System.Collections.Generic;
using System.Threading.Tasks;
using BL.Dtos.Category;
using DAL.Entities;

namespace BL.Services
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