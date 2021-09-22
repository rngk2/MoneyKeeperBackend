using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.Entities;
using MoneyKeeper.DAL;
using MoneyKeeper.DAL.Models;

namespace DAL.Repositories
{
	internal class DapperCategoriesRepository : ICategoriesRepository
	{
		private readonly IDapperRepository dapperRepository;

		public DapperCategoriesRepository(IDapperRepository dapperRepository)
		{
			this.dapperRepository = dapperRepository;
		}

		public async Task<IEnumerable<Category>> GetCategories(int userId)
		{
			string sql = "select * from Categories where UserId = @userId";

			return await dapperRepository.QueryAny<Category>(sql, new { userId });
		}

		public async Task<IEnumerable<Category>> GetCategories()
		{
			string sql = "select * from Categories";

			return await dapperRepository.QueryAny<Category>(sql);
		}

		public async Task<Category> GetCategory(int id)
		{
			string sql = @"
					select * from Categories 
					where 
						Id = @id
			";

			return (await dapperRepository.QueryAny<Category>(sql, new { id })).FirstOrDefault()!;
		}

		public async Task<Category> GetCategory(int userId, string categoryName)
		{
			string sql = @"
					select * from Categories 
					where 
						UserId = @userId and Name = @categoryName
			";

			return (await dapperRepository.QueryAny<Category>(sql, new { userId, categoryName })).FirstOrDefault()!;
		}

		public async Task<IEnumerable<CategoryOverview>> GetCategoriesOverview(int userId, Range range)
		{	
			string sql = @"
				select
					Categories.Id as CategoryId, Categories.Name as CategoryName, 
					(select sum(Amount) from Transactions where CategoryId = Categories.Id and [Name] != 'Earnings') as SpentThisMonth
				from
					Categories
				where
					Categories.UserId = @userId and [Name] != 'Earnings' 
				order by
					Categories.Name asc
				offset 
					@start rows
				fetch 
					next @next rows only
			";

			return await dapperRepository.QueryAny<CategoryOverview>(sql, new 
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

			return (await dapperRepository.QueryAny<CategoryOverview>(sql, new { categoryId })).FirstOrDefault()!;
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

			return await dapperRepository.QuerySingleWithOutput<int>(sql, category);
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

			return await dapperRepository.ExecuteAny(sql, category) == UtilConstants.SQL_SINGLE_ROW_AFFECTED;
		}

		public async Task<bool> DeleteCategory(int id)
		{
			string sql = @"
					delete from Categories
					where 
						Id = @id
			";

			return await dapperRepository.ExecuteAny(sql, new { id }) == UtilConstants.SQL_SINGLE_ROW_AFFECTED;
		}
	}
}
