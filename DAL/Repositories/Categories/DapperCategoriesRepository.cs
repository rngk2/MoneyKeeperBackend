﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.Entities;
using MoneyKeeper.Globals;

namespace DAL.Repositories.Categories
{
	public class DapperCategoriesRepository : ICategoriesRepository
	{
		private readonly IDapperRepository dapperRepository;

		private const string CATEGORIES_TABLE_NAME = "Categories";

		public DapperCategoriesRepository(IDapperRepository dapperRepository)
		{
			this.dapperRepository = dapperRepository;
		}

		public async Task<IEnumerable<Category>> GetCategories(int userId)
		{
			string sql = $"select * from {CATEGORIES_TABLE_NAME} where UserId = @userId";

			return await dapperRepository.QueryAny<Category>(sql, new { userId });
		}

		public async Task<IEnumerable<Category>> GetCategories()
		{
			string sql = $"select * from {CATEGORIES_TABLE_NAME}";

			return await dapperRepository.QueryAny<Category>(sql);
		}

		public async Task<Category> GetCategory(int id)
		{
			string sql = $@"
					select * from {CATEGORIES_TABLE_NAME} 
					where 
						Id = @id
			";

			return (await dapperRepository.QueryAny<Category>(sql, new { id })).FirstOrDefault()!;
		}

		public async Task<Category> GetCategory(int userId, string categoryName)
		{
			string sql = $@"
					select * from {CATEGORIES_TABLE_NAME} 
					where 
						UserId = @userId and Name = @categoryName
			";

			return (await dapperRepository.QueryAny<Category>(sql, new { userId, categoryName })).FirstOrDefault()!;
		}

		public async Task<int> CreateCategory(Category category)
		{
			string sql = $@"
					insert into {CATEGORIES_TABLE_NAME}
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
			string sql = $@"
					update 
						{CATEGORIES_TABLE_NAME}
					set
						Name = @Name
					where
						Id = @Id
			";

			return await dapperRepository.ExecuteAny(sql, category) == (int)UtilConstants.SQL_SINGLE_ROW_AFFECTED;
		}

		public async Task<bool> DeleteCategory(int userId, string categoryName)
		{
			string sql = $@"
					delete from {CATEGORIES_TABLE_NAME}
					where 
						UserId = @userId and Name = @categoryName
			";

			return await dapperRepository.ExecuteAny(sql, new { userId, categoryName }) == (int)UtilConstants.SQL_SINGLE_ROW_AFFECTED;
		}

		public async Task<bool> DeleteCategory(int id)
		{
			string sql = $@"
					delete from {CATEGORIES_TABLE_NAME}
					where 
						Id = @id
			";

			return await dapperRepository.ExecuteAny(sql, new { id }) == (int)UtilConstants.SQL_SINGLE_ROW_AFFECTED;
		}
	}
}
