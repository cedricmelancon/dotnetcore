using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UserApplication.Data;
using UserApplication.Data.Models;

namespace UserApplication.Services
{
    public class UserService : IUserService
    {
        private readonly ILogger<IUserService> _logger;
        private readonly UserContext _dbContext;

        public UserService(ILogger<IUserService> logger, DbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext as UserContext;
        }

        public IEnumerable<UserModel> GetUsers()
        {
            _logger.LogInformation("Getting all users.");
            return _dbContext.Users.Include(u => u.Addresses);
        }

        public async Task AddUserAsync(UserModel user)
        {
            _logger.LogInformation("Adding user {first} {last}", user.FirstName, user.LastName);
            _dbContext.Add(user);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateUserAsync(UserModel user)
        {
            _logger.LogInformation("Modifying the user {id}", user.Id);
            _dbContext.Update(user);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteUserAsync(Guid id)
        {
            var user = _dbContext.Users.SingleOrDefault(u => u.Id == id);

            if (user != null)
            {
                _logger.LogInformation("Deleting user {first} {last}.", user.FirstName, user.LastName);
                _dbContext.Remove(user);

                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
