using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.Entities;
using MoneyKeeper.Globals;

namespace DAL.Repositories.Categories
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

			return await dapperRepository.ExecuteAny(sql, category) == (int)UtilConstants.SQL_SINGLE_ROW_AFFECTED;
		}

		public async Task<bool> DeleteCategory(int id)
		{
			string sql = @"
					delete from Categories
					where 
						Id = @id
			";

			return await dapperRepository.ExecuteAny(sql, new { id }) == (int)UtilConstants.SQL_SINGLE_ROW_AFFECTED;
		}
	}
}
