using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using BL.Dtos.Category;
using BL.Extensions;
using BL.Services;
using Globals.Errors;
using MoneyKeeper.Providers;
using MoneyKeeper.Api.Results;
using MoneyKeeper.Globals.Errors;
using Microsoft.AspNetCore.Authorization;
using System;
using MoneyKeeper.DAL.Models;

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
		public async Task<ApiResult<Category>> GetCategory(int id)
		{
			var (contextUser, provider_error) = await currentUserProvider.GetCurrentUser().Unwrap();

			if (provider_error)
			{
				return provider_error.Wrap();
			}

			var (category, service_error) = await categoryService.GetCategory(id, contextUser.Id).Unwrap();

			return service_error
				? service_error.Wrap()
				: category;
		}

		[HttpGet("overview")]
		public async Task<ApiResult<IEnumerable<CategoryOverview>>> GetCategoriesOverview(int from, int to)
		{
			var (contextUser, provider_error) = await currentUserProvider.GetCurrentUser().Unwrap();

			if (provider_error)
			{
				return provider_error.Wrap();
			}

			var (categories, service_error) = await categoryService.GetCategoriesOverview(contextUser.Id, new(from, to)).Unwrap();

			return service_error
				? service_error.Wrap()
				: categories.ToList();
		}	
		
		[HttpGet("overview/{categoryId}")]
		public async Task<ApiResult<CategoryOverview>> GetCategoriesOverview(int categoryId)
		{
			var (contextUser, provider_error) = await currentUserProvider.GetCurrentUser().Unwrap();

			if (provider_error)
			{
				return provider_error.Wrap();
			}

			var (category, service_error) = await categoryService.GetCategoryOverview(contextUser.Id, categoryId).Unwrap();

			return service_error
				? service_error.Wrap()
				: category;
		}

		[HttpGet("earnings/overview")]
		public async Task<ApiResult<CategoryOverview>> GetEarningsOverview()
		{
			var (contextUser, provider_error) = await currentUserProvider.GetCurrentUser().Unwrap();

			if (provider_error)
			{
				return provider_error.Wrap();
			}

			return (await categoryService.GetEarningsOverview(contextUser.Id).Unwrap()).Value!;
		}

		[HttpGet]
		public async Task<ApiResult<IEnumerable<Category>>> GetCategoriesOfUser()
		{
			var (contextUser, provider_error) = await currentUserProvider.GetCurrentUser().Unwrap();

			if (provider_error)
			{
				return provider_error.Wrap();
			}

			var (categories, service_error) = await categoryService.GetCategoriesOfUser(contextUser.Id).Unwrap();

			return service_error
				? service_error.Wrap()
				: categories.ToList();
		}

		[HttpPost]
		public async Task<ApiResult<Category>> AddCategoryToUser(CreateCategory categoryDto)
		{
			var (contextUser, provider_error) = await currentUserProvider.GetCurrentUser().Unwrap();

			if (provider_error)
			{
				return provider_error.Wrap();
			}

			var (created, service_error) = await categoryService.AddCategoryToUser(categoryDto with
			{
				UserId = contextUser.Id
			}).Unwrap();

			return service_error
				? service_error.Wrap()
				: created;
		}

		[HttpPut("{categoryId}")]
		public async Task<ApiResult<Category>> UpdateCategoryToUser(int categoryId, UpdateCategory categoryDto)
		{
			var (contextUser, provider_error) = await currentUserProvider.GetCurrentUser().Unwrap();

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
				: updated;
		}

		[HttpDelete("{id}")]
		public async Task<ApiResult<Category>> DeleteCategory(int id)
		{
			var (contextUser, provider_error) = await currentUserProvider.GetCurrentUser().Unwrap();

			if (provider_error)
			{
				return provider_error.Wrap();
			}

			var (deleted, error) = await categoryService.DeleteCategoryToUser(id, contextUser.Id).Unwrap();

			return error
				? error.Wrap()
				: deleted;
		}

		[HttpDelete]
		public async Task<ApiResult<Category>> DeleteCategory([FromQuery] string categoryName)
		{
			var (contextUser, provider_error) = await currentUserProvider.GetCurrentUser().Unwrap();

			if (provider_error)
			{
				return provider_error.Wrap();
			}

			var (deleted, service_error) = await categoryService.DeleteCategoryToUser(categoryName, contextUser.Id).Unwrap();

			return service_error
				? service_error.Wrap()
				: deleted;
		}
	}
}
