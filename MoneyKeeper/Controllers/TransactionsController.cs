using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BL.Dtos.Transaction;
using BL.Extensions;
using BL.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MoneyKeeper.Controllers
{
	[Route("[controller]")]
	[ApiController]
	public class TransactionsController : ControllerBase
	{

		private ITransactionService transactionService;

		public TransactionsController(ITransactionService transactionService)
		{
			this.transactionService = transactionService;
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
		
		[HttpGet("{userId}/{from}/{to}")]
		public async Task<IEnumerable<TransactionDto>> GetTransactionsOfUser(int userId, int from, int to)
		{
			return (await transactionService.GetTransactionsOfUser(userId, new Range(from, to))).Select(t => t.AsDto());
		}



		[HttpPost]
		public async Task<ActionResult<TransactionDto>> CreateTransaction(CreateTransactionDto transactionDto)
		{
			var createdTransaction = await transactionService.CreateTransaction(transactionDto);
			return CreatedAtAction(
				nameof(GetTransaction),
				new { id = createdTransaction.Id },
				createdTransaction.AsDto());
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteTransaction(int id)
		{
			await transactionService.DeleteTransaction(id);
			return NoContent();
		}

	}
}
