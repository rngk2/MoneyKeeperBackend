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

		Task<IEnumerable<SummaryUnit>> GetSummaryForUser(int id);

		Task<IEnumerable<SummaryUnit>> GetSummaryForUserForMonth(int id);

		Task<IEnumerable<SummaryUnit>> GetSummaryForUserForYear(int id);

		Task<int> CreateUser(User user);

		Task<bool> UpdateUser(User user);

		Task<bool> DeleteUser(int id);
	}
}
