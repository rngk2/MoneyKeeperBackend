using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MoneyKeeper.Entities;

namespace MoneyKeeper.Repositories.Categories
{
	public class DapperCategoriesRepository : ICategoriesRepository
	{
		private readonly IDapperRepository dapperRepository;

		private const string TABLE_NAME = "dbo.Categories";

		public DapperCategoriesRepository(IDapperRepository dapperRepository)
		{
			this.dapperRepository = dapperRepository;
		}

		public async Task<IEnumerable<Category>> GetCategoriesOfUser(int userId)
		{
			string getCategoryQuery = $"select * from {TABLE_NAME} where UserId = @userId";
			return await dapperRepository.QueryAny<Category>(getCategoryQuery, new { userId });
		}

		public async Task<Category> GetCategory(int id)
		{
			string getCategoryQuery = $"select * from {TABLE_NAME} where Id = @id";
			return (await dapperRepository.QueryAny<Category>(getCategoryQuery, new { id })).FirstOrDefault();
		}

		public async Task<Category> GetCategory(int userId, string categoryName)
		{
			string getCategoryQuery = $"select * from {TABLE_NAME} where UserId = @userId and Name = @categoryName";
			return (await dapperRepository.QueryAny<Category>(getCategoryQuery, new { userId, categoryName })).FirstOrDefault();
		}

		public async Task<int> AddCategoryToUser(Category category)
		{
			string addCategoryQuery = @$"
				insert into {TABLE_NAME}
					(Name, UserId)
				output inserted.Id
				values
					(@Name, @UserId)
			";
			return await dapperRepository.QuerySingle<Category>(addCategoryQuery, category);
		}

		public async Task DeleteCategoryToUser(int id)
		{
			string deleteUserQuery = @$"
					delete from {TABLE_NAME}
						where
							Id = @id
							
			";
			await dapperRepository.ExecuteAny<User>(deleteUserQuery, new { id });
		}

		public async Task UpdateCategoryToUser(Category category)
		{
			string updateCategoryQuery = @$"
				update {TABLE_NAME}
					set
						Name = @Name
					where
						Id = @Id
			";
			await dapperRepository.ExecuteAny<Category>(updateCategoryQuery, category);
		}
	}
}
