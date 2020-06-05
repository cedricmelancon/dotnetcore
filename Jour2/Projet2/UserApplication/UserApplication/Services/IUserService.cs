using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UserApplication.Data.Models;

namespace UserApplication.Services
{
    public interface IUserService
    {
        IEnumerable<UserModel> GetUsers();
        Task AddUserAsync(UserModel user);
        Task UpdateUserAsync(UserModel user);
        Task DeleteUserAsync(Guid id);
    }
}
