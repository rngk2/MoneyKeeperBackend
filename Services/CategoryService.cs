using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MoneyKeeper.Dtos.Category;
using MoneyKeeper.Entities;
using MoneyKeeper.Extensions;
using MoneyKeeper.Repositories.Categories;

namespace MoneyKeeper.Services
{
	public class CategoryService : ICategoryService
	{
		private readonly ICategoriesRepository repository;

		public CategoryService(ICategoriesRepository repository)
		{
			this.repository = repository;
		}

		public async Task<Category> GetCategory(int id)
		{
			return await repository.GetCategory(id);
		}

		public async Task<IEnumerable<Category>> GetCategories()
		{
			return await repository.GetCategories();
		}

		public async Task<IEnumerable<Category>> GetCategoriesOfUser(int userId)
		{
			return await repository.GetCategoriesOfUser(userId);//(await repository.GetCategoriesOfUser(userId)).Select(category => category.AsDto())
		}

		public async Task<Category> AddCategoryToUser(CreateCategoryDto categoryDto)
		{
			Category category = new()
			{
				Name = categoryDto.Name,
				UserId = categoryDto.UserId
			};

			/*if ((await repository.GetCategory(category.UserId, category.Name)) is not null)
			{
				return 
			}*/

			int createdId = await repository.AddCategoryToUser(category);

			Category created = category with
			{
				Id = createdId
			};

			return created;
		}

		public async Task UpdateCategoryToUser(Category existingCategory, UpdateCategoryDto categoryDto)
		{
		
			/*if ((await repository.GetCategory(existingCategory.UserId, categoryDto.Name)) is not null)
			{
				return StatusCode(409, $"User#{existingCategory.UserId} alredy has category with name={categoryDto.Name}");
			}*/

			Category updatedCategory = existingCategory with
			{
				Name = categoryDto.Name is null ? existingCategory.Name : categoryDto.Name
			};

			await repository.UpdateCategoryToUser(updatedCategory);

		}

		public async Task DeleteCategory(int id)
		{
			await repository.DeleteCategoryToUser(id);

		}

		public async Task<Category> GetCategory(int userId, string name)
		{
			return await repository.GetCategory(userId, name);
		}
	}
}
