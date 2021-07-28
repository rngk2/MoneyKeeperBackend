using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MoneyKeeper.Dtos.Category;
using MoneyKeeper.Entities;
using MoneyKeeper.Repositories.Categories;
using MoneyKeeper.Extensions;

namespace MoneyKeeper.Controllers
{
	[Route("[controller]")]
	[ApiController]
	public class CategoriesController : ControllerBase
	{
		private readonly ICategoriesRepository repository;

		public CategoriesController(ICategoriesRepository repository)
		{
			this.repository = repository;
		}

		public async Task<ActionResult<CategoryDto>> GetCategory(int id)
		{
			var category = await repository.GetCategory(id);
			return category is null ? NotFound() : category.AsDto();
		}

		public ActionResult<IEnumerable<CategoryDto>> GetCategoriesOfUser(int userId)
		{
			throw new NotImplementedException();
		}

		public ActionResult AddCategoryToUser(CreateCategoryDto categoryDto)
		{
			throw new NotImplementedException();
		}

		public ActionResult UpdateCategoryToUser(int id, UpdateCategoryDto categoryDto)
		{
			throw new NotImplementedException();
		}

		public ActionResult DeleteCategoryToUser(int id, UpdateCategoryDto categoryDto)
		{
			throw new NotImplementedException();
		}
	}
}
