using System.Collections.Generic;
using System.Threading.Tasks;
using BL.Dtos.Category;
using DAL.Entities;
using MoneyKeeper.Api.Results;

namespace BL.Services
{
	public interface ICategoryService
	{
		Task<Result<Category>> AddCategoryToUser(CreateCategoryDto categoryDto);

		//Task<Result<Category>> DeleteCategory(int id);

		Task<Result<IEnumerable<Category>>> GetCategoriesOfUser(int userId);

		Task<Result<IEnumerable<Category>>> GetCategories();

		Task<Result<Category>> GetCategory(int id);

		Task<Result<Category>> UpdateCategoryToUser(Category existingCategory, UpdateCategoryDto categoryDto);

		Task<Result<Category>> GetCategory(int userId, string name);

		Task<Result<Category>> DeleteCategoryToUser(string categoryName);
	}
}