using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using BL.Dtos.Transaction;
using BL.Extensions;
using BL.Services;
using Globals.Errors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MoneyKeeper.Api.Results;
using MoneyKeeper.Attributes;
using MoneyKeeper.Providers;

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
			var (contextUser, provider_error) = currentUserProvider.GetCurrentUser().Unwrap();

			if (provider_error)
			{
				return provider_error.Wrap();
			}

			var (transaction, service_error) = await transactionService.GetTransaction(id, contextUser.Id).Unwrap();

			return service_error
				? service_error.Wrap()
				: transaction.AsDto();
		}

		/*[HttpGet]
		public async Task<ApiResult<IEnumerable<TransactionDto>>> GetTransactions()
		{
			var (contextUser, provider_error) = currentUserProvider.GetCurrentUser().Unwrap();

			if (provider_error)
			{
				return provider_error.Wrap();
			}

			return await transactionService.GetTransactions
		}*/

		[HttpGet("ofUser")]
		public async Task<ApiResult<IEnumerable<TransactionDto>>> GetTransactionsOfUser(
			[Required] int from, [Required] int to, string? like = null, DateTimeOffset? when = null)
		{
			var (contextUser, provider_error) = currentUserProvider.GetCurrentUser().Unwrap();

			if (provider_error)
			{
				return provider_error.Wrap();
			}

			var (transactions, service_error) = await transactionService
				.GetTransactionsOfUser(contextUser.Id, new Range(from, to), like, when)
				.Unwrap();

			return service_error
				? service_error.Wrap()
				: transactions.Select(t => t.AsDto()).ToList();
		}

		[HttpPost]
		public async Task<ActionResult<TransactionDto>> CreateTransaction(CreateTransactionDto transactionDto)
		{
			try
			{
				var createdTransaction = await transactionService.CreateTransaction(transactionDto);

				return CreatedAtAction(
					nameof(GetTransaction),
					new { id = createdTransaction.Id },
					createdTransaction.AsDto());
			}
			catch (SqlException e)
			{
				if (e.Number == (int)SqlErrorCodes.FK_CONFLICT_ERROR)
				{
					return new ConflictObjectResult(e.Message);
				}

				return new StatusCodeResult(500);
			}
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteTransaction(int id)
		{
			await transactionService.DeleteTransaction(id);
			return NoContent();
		}

	}
}
