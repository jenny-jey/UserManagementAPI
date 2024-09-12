using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserAPI.DataAccess.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _dataContext;
        private readonly ILogger<UserRepository> _logger;
        public UserRepository(DataContext dataContext, ILogger<UserRepository> logger)
        {
            _dataContext = dataContext;
            _logger = logger;
        }

        public async Task AddUserAsync(User user)
        {
            try
            {
                await _dataContext.Users.AddAsync(user);
                await _dataContext.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding the user.");
                throw new Exception("An unexpected error occurred while adding the user.", ex);
            }
        }

        public async Task DeleteUserAsync(int id)
        {
            try
            {
                var user = await _dataContext.Users.FindAsync(id);
                if (user == null)
                {
                    throw new Exception("User not found");
                }
                _dataContext.Users.Remove(user);
                await _dataContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting user with ID {Id}", id);
                throw new Exception("An unexpected error occurred while deleting the user.", ex);

            }
        }

        public async Task<User> GetUserAsync(int id)
        {
            //var user = await _dataContext.Users.FindAsync(id);
            try
            {
                var user = await _dataContext.Users.Where(u => u.Id == id).FirstOrDefaultAsync();
                if (user == null)
                {
                    throw new KeyNotFoundException($"User not found.");
                }
                return user;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving user with ID {Id}", id);
                throw new Exception("An unexpected error occurred while retrieving the user.", ex); 

            }
        }

        public async  Task<IEnumerable<User>> GetUsersAsync()
        {
            try
            {
                return await _dataContext.Users.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while retrieving users.");
                throw new Exception("An unexpected error occurred while retrieving the list of users.", ex);
            }
        }

        public async Task UpdateUserAsync(User user)
        {
            try
            {
                _dataContext.Users.Update(user);
                await _dataContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the user");
                throw new Exception("An unexpected error occurred while updating the user.", ex);

            }
        }
    }
}
