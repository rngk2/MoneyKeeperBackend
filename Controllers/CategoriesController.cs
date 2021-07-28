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

		// GET /categories/{id}
		[HttpGet("{id}")]
		public async Task<ActionResult<CategoryDto>> GetCategory(int id)
		{
			var category = await repository.GetCategory(id);
			return category is null ? NotFound() : category.AsDto();
		}

		// GET /categories
		[HttpGet]
		public async Task<IEnumerable<CategoryDto>> GetCategoriesOfUser(int userId)
		{
			return (await repository.GetCategoriesOfUser(userId)).Select(category => category.AsDto());
		}

		// POST /categories
		[HttpPost]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status409Conflict)]
		public async Task<ActionResult<CategoryDto>> AddCategoryToUser(CreateCategoryDto categoryDto)
		{
			Category category = new()
			{
				Name = categoryDto.Name,
				UserId = categoryDto.UserId
			};

			if ((await repository.GetCategory(category.UserId, category.Name)) is not null)
			{
				return StatusCode(409, $"User#{category.UserId} alredy has category with name={category.Name}");
			}

			await repository.AddCategoryToUser(category);

			return CreatedAtAction(
					nameof(GetCategory),
					new { id = category.Id },
					category.AsDto()
				);
		}

		// PUT /categories/{id}
		[HttpPut("{id}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status409Conflict)]
		public async Task<ActionResult> UpdateCategoryToUser(int id, UpdateCategoryDto categoryDto)
		{
			var existingCategory = await repository.GetCategory(id);

			if (existingCategory is null) 
			{
				return NotFound();
			}

			else if ((await repository.GetCategory(existingCategory.UserId, categoryDto.Name)) is not null)
			{
				return StatusCode(409, $"User#{existingCategory.UserId} alredy has category with name={categoryDto.Name}");
			}

			return NoContent();
		}

		// DELETE /categories/{id}
		[HttpDelete("{id}")]
		public async Task<ActionResult> DeleteCategoryToUser(int id)
		{
			await repository.DeleteCategoryToUser(id);

			return NoContent();
		}
	}
}
