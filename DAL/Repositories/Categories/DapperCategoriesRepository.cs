using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MoneyKeeper.DAL.Entities;
using MoneyKeeper.DAL;
using MoneyKeeper.DAL.Models;
using Microsoft.Extensions.Options;
using MoneyKeeper.DAL.Settings;

namespace MoneyKeeper.DAL.Repositories
{
	internal class DapperCategoriesRepository : DapperRepository, ICategoriesRepository
	{
		public DapperCategoriesRepository(IOptions<DapperSettings> options): base(options) { } 

		public async Task<IEnumerable<Category>> GetCategories(int userId)
		{
			string sql = "select * from Categories where UserId = @userId";

			return await QueryAny<Category>(sql, new { userId });
		}

		public async Task<IEnumerable<Category>> GetCategories()
		{
			string sql = "select * from Categories";

			return await QueryAny<Category>(sql);
		}

		public async Task<Category> GetCategory(int id)
		{
			string sql = @"
					select * from Categories 
					where 
						Id = @id
			";

			return (await QueryAny<Category>(sql, new { id })).FirstOrDefault()!;
		}

		public async Task<Category?> GetCategory(int userId, string categoryName)
		{
			string sql = @"
					select * from Categories 
					where 
						UserId = @userId and Name = @categoryName
			";

			return (await QueryAny<Category>(sql, new { userId, categoryName })).FirstOrDefault();
		}

		public async Task<IEnumerable<CategoryOverview>> GetCategoriesOverview(int userId, Range range)
		{	
			string sql = @"
				select
					Categories.Id as CategoryId, Categories.Name as CategoryName, 
					(select sum(Amount) from Transactions where CategoryId = Categories.Id and Categories.Name != 'Earnings') as SpentThisMonth
				from
					Categories
				where
					Categories.UserId = @userId and Categories.Name != 'Earnings' 
				order by
					Categories.Name asc
				offset 
					@start rows
				fetch 
					next @next rows only
			";

			return await QueryAny<CategoryOverview>(sql, new 
			{
				start = range.Start.Value,
				next = range.End.Value - range.Start.Value,
				userId
			});
		}

		public async Task<CategoryOverview> GetCategoryOverview(int categoryId)
		{
			string sql = @"
				select
					Categories.Id as CategoryId, Categories.Name as CategoryName, 
					(select sum(Amount) from Transactions where CategoryId = Categories.Id) as SpentThisMonth
				from
					Categories
				where
					Categories.Id = @categoryId
			";

			return (await QueryAny<CategoryOverview>(sql, new { categoryId })).FirstOrDefault()!;
		}

		public async Task<CategoryOverview> GetEarningsOverview(int userId)
		{
			string sql = @"
				select
					Categories.Id as CategoryId, Categories.Name as CategoryName, 
					(select sum(Amount) from Transactions where CategoryId = Categories.Id and month([Timestamp]) = month(getdate())) as SpentThisMonth
				from
					Categories
				where
					Categories.Name = 'Earnings' and Categories.UserId = @userId
			";

			return (await QueryAny<CategoryOverview>(sql, new { userId })).FirstOrDefault()!;
		}

		public async Task<int> CreateCategory(Category category)
		{
			string sql = @"
					insert into Categories
						(Name, UserId)
					output 
						inserted.Id
					values
						(@Name, @UserId)
			";

			return await QuerySingleWithOutput<int>(sql, category);
		}

		public async Task<bool> UpdateCategory(Category category)
		{
			string sql = @"
					update 
						Categories
					set
						Name = @Name
					where
						Id = @Id
			";

			return await ExecuteAny(sql, category) == UtilConstants.SQL_SINGLE_ROW_AFFECTED;
		}

		public async Task<bool> DeleteCategory(int id)
		{
			string sql = @"
					delete from Categories
					where 
						Id = @id
			";

			return await ExecuteAny(sql, new { id }) == UtilConstants.SQL_SINGLE_ROW_AFFECTED;
		}
	}
}
