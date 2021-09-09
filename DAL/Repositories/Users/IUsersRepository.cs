using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.Entities;
using DAL.Models;

namespace DAL.Repositories
{
	public interface IUsersRepository
	{
		Task<User> GetUser(int id);

		Task<User> GetUser(string email);

		Task<IEnumerable<Transaction>> GetSummaryForUser(int id);

		Task<IEnumerable<Transaction>> GetSummaryForUser_ForMonth(int id);

		Task<IEnumerable<Transaction>> GetSummaryForUser_ForYear(int id);

		Task<int> CreateUser(User user);

		Task<bool> UpdateUser(User user);

		Task<bool> DeleteUser(int id);
	}
}
