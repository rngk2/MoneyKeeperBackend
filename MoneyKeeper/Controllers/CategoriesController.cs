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
using Globals.Errors;
using MoneyKeeper.Attributes;
using MoneyKeeper.Providers;

namespace MoneyKeeper.Controllers
{
	[Route("[controller]")]
	[ApiController]
	public class CategoriesController : ControllerBase
	{
		private readonly ICategoryService categoryService;
		private readonly ICurrentUserProvider currentUserProvider;

		public CategoriesController(ICategoryService categoryService, ICurrentUserProvider currentUserProvider)
		{
			this.categoryService = categoryService;
			this.currentUserProvider = currentUserProvider;
		}


		[HttpGet("{id}")]
		public async Task<ActionResult<CategoryDto>> GetCategory(int id)
		{
			var category = await categoryService.GetCategory(id);
			return category is null ? NotFound() : category.AsDto();
		}

		[HttpGet]
		public async Task<IEnumerable<CategoryDto>> GetCategoriesOfUser()
		{
			return (await categoryService.GetCategoriesOfUser(currentUserProvider.GetCurrentUser().Id))
				.Select(category => category.AsDto());
		}

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

		[HttpPut("{id}")]
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
				if (e.Number == ((int)SqlErrorCodes.DUBLICATE_KEY_ERROR))  
				{
					return StatusCode(409, $"User#{existingCategory.UserId} alredy has category with name={categoryDto.Name}");
				}

				return StatusCode(500);
			}
		}

		[HttpDelete("{id}")]
		public async Task<ActionResult> DeleteCategory(int id)
		{
			await categoryService.DeleteCategory(id);

			return NoContent();
		}

		[HttpDelete("byname/{categoryName}")]	
		public async Task<ActionResult> DeleteCategory(string categoryName)
		{
			await categoryService.DeleteCategoryToUser(currentUserProvider.GetCurrentUser().Id, categoryName);

			return NoContent();
		}
	}
}
