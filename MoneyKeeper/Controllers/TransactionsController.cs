using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using BL.Dtos.Transaction;
using BL.Extensions;
using BL.Services;
using DAL.Entities;
using Globals.Errors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using MoneyKeeper.Api.Results;
using MoneyKeeper.Providers;
using static MoneyKeeper.Api.Types;

namespace MoneyKeeper.Controllers
{
	[Route("[controller]")]
	[ApiController]
	[Authorize]
	public class TransactionsController : ControllerBase
	{
		private readonly ITransactionService transactionService;
		private readonly ICurrentUserProvider currentUserProvider;

		public TransactionsController(ITransactionService transactionService, ICurrentUserProvider currentUserProvider)
		{
			this.transactionService = transactionService;
			this.currentUserProvider = currentUserProvider;
		}

		[HttpGet("{id}")]
		public async Task<ApiResult<TransactionDto>> GetTransaction(int id)
		{
			var (contextUser, provider_error) = await currentUserProvider.GetCurrentUser().Unwrap();

			if (provider_error)
			{
				return provider_error.Wrap();
			}

			var (transaction, service_error) = await transactionService.GetTransaction(id, contextUser.Id).Unwrap();

			return service_error
				? service_error.Wrap()
				: transaction.AsDto();
		}

		[HttpGet("category/{categoryId}/transactions")]
		public async Task<ApiResult<IEnumerable<TransactionDto>>> GetTransactionsForCategories(int categoryId, [Required] int from, [Required] int to)
		{
			var (contextUser, provider_error) = await currentUserProvider.GetCurrentUser().Unwrap();

			if (provider_error)
			{
				return provider_error.Wrap();
			}

			var (transactions, service_error) = await transactionService
				.GetTransactionsForCategory(contextUser.Id, categoryId, new Range(from, to))
				.Unwrap();

			return service_error
				? service_error.Wrap()
				: transactions.Select(t => t.AsDto()).ToList();
		}

		[HttpGet("user/transactions")]
		public async Task<ApiResult<IEnumerable<TransactionDto>>> GetTransactions(
			[Required] int from,
			[Required] int to,
			[Required] TransactionField orderByField,
			[Required] Order.OrderType order,
			string? searchPattern = null
		)
		{
			var (contextUser, provider_error) = await currentUserProvider.GetCurrentUser().Unwrap();

			if (provider_error)
			{
				return provider_error.Wrap();
			}

			var (transactions, service_error) = await transactionService
				.GetTransactions(contextUser.Id, new Range(from, to), orderByField.ToString(), order.ToString(), searchPattern)
				.Unwrap();

			return service_error
				? service_error.Wrap()
				: transactions.Select(t => t.AsDto()).ToList();
		}


		[HttpGet("summary")]
		public async Task<ApiResult<IEnumerable<TransactionDto>>> GetSummaryForMonth()
		{
			var (contextUser, provider_error) = await currentUserProvider.GetCurrentUser().Unwrap();

			if (provider_error)
			{
				return provider_error.Wrap();
			}

			return (await transactionService.GetSummaryForUser(contextUser.Id).Unwrap())
				.Value!
				.Select(t => t.AsDto())
				.ToList();
		}

		[HttpGet("total")]
		public async Task<ApiResult<Dictionary<string, decimal>>> GetTotal()
		{
			var (contextUser, provider_error) = await currentUserProvider.GetCurrentUser().Unwrap();

			if (provider_error)
			{
				return provider_error.Wrap();
			}

			return (await transactionService.GetTotalForUser(contextUser.Id).Unwrap()).Value!;
		}

		[HttpGet("total/month")]
		public async Task<ApiResult<Dictionary<string, decimal>>> GetTotalForMonth()
		{
			var (contextUser, provider_error) = await currentUserProvider.GetCurrentUser().Unwrap();

			if (provider_error)
			{
				return provider_error.Wrap();
			}

			return (await transactionService.GetTotalForUserForMonth(contextUser.Id).Unwrap()).Value!;
		}

		[HttpGet("total/year")]
		public async Task<ApiResult<Dictionary<string, decimal>>> GetTotalForYear()
		{
			var (contextUser, provider_error) = await currentUserProvider.GetCurrentUser().Unwrap();

			if (provider_error)
			{
				return provider_error.Wrap();
			}

			return (await transactionService.GetTotalForUserForYear(contextUser.Id).Unwrap()).Value!;
		}

		[HttpPost]
		public async Task<ApiResult<TransactionDto>> CreateTransaction(CreateTransactionDto transactionDto)
		{
			var (contextUser, provider_error) = await currentUserProvider.GetCurrentUser().Unwrap();

			if (provider_error)
			{
				return provider_error.Wrap();
			}

			var (created, service_error) = await transactionService.CreateTransaction(transactionDto with
			{
				UserId = contextUser.Id
			}).Unwrap();

			return service_error
				? service_error.Wrap()
				: created.AsDto();
		}

		[HttpDelete("{id}")]
		public async Task<ApiResult<TransactionDto>> DeleteTransaction(int id)
		{
			var (contextUser, provider_error) = await currentUserProvider.GetCurrentUser().Unwrap();

			if (provider_error)
			{
				return provider_error.Wrap();
			}

			var (deleted, error) = await transactionService.DeleteTransaction(id, contextUser.Id).Unwrap();

			return error
				? error.Wrap()
				: deleted.AsDto();
		}

	}
}
