using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using BL.Dtos.Transaction;
using BL.Extensions;
using BL.Services;
using Globals.Errors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MoneyKeeper.Attributes;
using MoneyKeeper.Providers;

namespace MoneyKeeper.Controllers
{
	[Route("[controller]")]
	[ApiController]
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
		public async Task<TransactionDto> GetTransaction(int id)
		{
			return (await transactionService.GetTransaction(id)).AsDto();
		}

		[HttpGet]
		public async Task<IEnumerable<TransactionDto>> GetTransactions()
		{
			return (await transactionService.GetTransactions()).Select(t => t.AsDto());
		}

		[HttpGet("{from}/{to}/{like?}/{when?}")]
		public async Task<IEnumerable<TransactionDto>> GetTransactionsOfUser(int from, int to, string like = null, DateTimeOffset when = default)
		{
			return (await transactionService.GetTransactionsOfUser(currentUserProvider.GetCurrentUser().Id, new Range(from, to), like, when))
				.Select(t => t.AsDto());
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
