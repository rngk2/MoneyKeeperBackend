using MoneyKeeper.BL.Dtos.Transaction;
using Entity = MoneyKeeper.DAL.Entities;

namespace MoneyKeeper.BL.Extensions
{
	public static class TransactionExtensions
	{
		public static Transaction AsDto(this Entity.Transaction transaction) => new Transaction
		{
			Id = transaction.Id,
			UserId = transaction.UserId,
			CategoryId = transaction.CategoryId,
			CategoryName = transaction.CategoryName,
			Amount = transaction.Amount,
			Timestamp = transaction.Timestamp,
			Comment = transaction.Comment
		};

		public static Entity.Transaction AsEntity(this Transaction transaction) => new Entity.Transaction
		{
			Id = transaction.Id,
			UserId = transaction.UserId,
			CategoryId = transaction.CategoryId,
			CategoryName = transaction.CategoryName,
			Amount = transaction.Amount,
			Timestamp = transaction.Timestamp,
			Comment = transaction.Comment
		};
	}
}
