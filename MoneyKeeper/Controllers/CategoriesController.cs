using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using DAL.Entities;
using BL.Dtos.Category;
using BL.Extensions;
using BL.Services;

namespace MoneyKeeper.Controllers
{
	[Route("[controller]")]
	[ApiController]
	public class CategoriesController : ControllerBase
	{
		private readonly ICategoryService categoryService;

		public CategoriesController(ICategoryService categoryService)
		{
			this.categoryService = categoryService;
		}

		// GET /categories
		[HttpGet]
		public async Task<IEnumerable<CategoryDto>> GetCategories()
		{
			return (await categoryService.GetCategories()).Select(category => category.AsDto());
		}

		// GET /categories/{id}
		[HttpGet("{id}")]
		public async Task<ActionResult<CategoryDto>> GetCategory(int id)
		{
			var category = await categoryService.GetCategory(id);
			return category is null ? NotFound() : category.AsDto();
		}

		// GET /categories/user/{userId}
		[HttpGet("user/{userId}")]
		public async Task<IEnumerable<CategoryDto>> GetCategoriesOfUser(int userId)
		{
			return (await categoryService.GetCategoriesOfUser(userId)).Select(category => category.AsDto());
		}

		// POST /categories
		[HttpPost]
		public async Task<ActionResult<CategoryDto>> AddCategoryToUser(CreateCategoryDto categoryDto)
		{
			if ((await categoryService.GetCategory(categoryDto.UserId, categoryDto.Name)) is not null)
			{
				return StatusCode(409, $"User#{categoryDto.UserId} alredy has category with name={categoryDto.Name}");
			}

			Category createdCategory = await categoryService.AddCategoryToUser(categoryDto);

			return CreatedAtAction(
					nameof(GetCategory),
					new { id = createdCategory.Id },
					createdCategory.AsDto()
				);
		}

		// PUT /categories/{id}
		[HttpPut("{id}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status409Conflict)]
		public async Task<ActionResult> UpdateCategoryToUser(int id, UpdateCategoryDto categoryDto)
		{
			var existingCategory = await categoryService.GetCategory(id);

			if (existingCategory is null) 
			{
				return NotFound();
			}

			try
			{
				await categoryService.UpdateCategoryToUser(existingCategory, categoryDto);
				return NoContent();
			}
			catch (SqlException e)
			{
				if (e.Number == 2627) // dublicate key error number
				{
					return StatusCode(409, $"User#{existingCategory.UserId} alredy has category with name={categoryDto.Name}");
				}

				return StatusCode(500);
			}
		}

		// DELETE /categories/{id}
		[HttpDelete("{id}")]
		public async Task<ActionResult> DeleteCategory(int id)
		{
			await categoryService.DeleteCategory(id);

			return NoContent();
		}
	}
}
