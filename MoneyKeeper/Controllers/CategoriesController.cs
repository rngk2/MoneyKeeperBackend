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
using MoneyKeeper.Api.Results;
using MoneyKeeper.Globals.Errors;

namespace MoneyKeeper.Controllers
{
	[Route("[controller]")]
	[ApiController]
	[Authorize]
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
		public async Task<ApiResult<CategoryDto>> GetCategory(int id)
		{
			var (contextUser, provider_error) = currentUserProvider.GetCurrentUser().Unwrap();

			if (provider_error)
			{
				return provider_error.Wrap();
			}

			var (category, service_error) = await categoryService.GetCategory(id, contextUser.Id).Unwrap();

			return service_error
				? service_error.Wrap()
				: category.AsDto();
		}

		[HttpGet]
		public async Task<ApiResult<IEnumerable<CategoryDto>>> GetCategoriesOfUser()
		{
			var (contextUser, provider_error) = currentUserProvider.GetCurrentUser().Unwrap();

			if (provider_error)
			{
				return provider_error.Wrap();
			}

			var (categories, service_error) = await categoryService.GetCategoriesOfUser(contextUser.Id).Unwrap();

			return service_error
				? service_error.Wrap()
				: categories.Select(c => c.AsDto()).ToList();
		}

		[HttpPost]
		public async Task<ApiResult<CategoryDto>> AddCategoryToUser(CreateCategoryDto categoryDto)
		{
			var (contextUser, provider_error) = currentUserProvider.GetCurrentUser().Unwrap();

			if (provider_error)
			{
				return provider_error.Wrap();
			}

			if (categoryDto.UserId != contextUser.Id)
			{
				return new Error(ApiResultErrorCodes.PROHIBITED, $"User: #{contextUser.Id} cannot add category to user: #{categoryDto.UserId}");
			}

			var (created, service_error) = await categoryService.AddCategoryToUser(categoryDto).Unwrap();

			return service_error
				? service_error.Wrap()
				: created.AsDto();
		}

		[HttpPut("{id}")]
		public async Task<ApiResult<CategoryDto>> UpdateCategoryToUser(int categoryId, UpdateCategoryDto categoryDto)
		{
			var (contextUser, provider_error) = currentUserProvider.GetCurrentUser().Unwrap();

			if (provider_error)
			{
				return provider_error.Wrap();
			}

			var (existingCategory, service_getError) = await categoryService.GetCategory(categoryId, contextUser.Id).Unwrap();

			if (service_getError)
			{
				return service_getError.Wrap();
			}

			var (updated, service_updateError) = await categoryService.UpdateCategoryToUser(existingCategory, categoryDto).Unwrap();

			return service_updateError
				? service_updateError.Wrap()
				: updated.AsDto();
		}

		[HttpDelete("{id}")]
		public async Task<ApiResult<CategoryDto>> DeleteCategory(int id)
		{
			var (contextUser, provider_error) = currentUserProvider.GetCurrentUser().Unwrap();

			if (provider_error)
			{
				return provider_error.Wrap();
			}

			var (deleted, error) = await categoryService.DeleteCategoryToUser(id, contextUser.Id).Unwrap();

			return error
				? error.Wrap()
				: deleted.AsDto();
		}

		[HttpDelete]
		public async Task<ApiResult<CategoryDto>> DeleteCategory(string categoryName)
		{
			var (contextUser, provider_error) = currentUserProvider.GetCurrentUser().Unwrap();

			if (provider_error)
			{
				return provider_error.Wrap();
			}

			var (deleted, service_error) = await categoryService.DeleteCategoryToUser(categoryName, contextUser.Id).Unwrap();

			return service_error
				? service_error.Wrap()
				: deleted.AsDto();
		}
	}
}
