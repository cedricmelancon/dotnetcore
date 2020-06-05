using System.Collections.Generic;
using UserApplication.Models;

namespace UserApplication.Services
{
    public interface IUserService
    {
        IEnumerable<UserModel> GetUsers();
    }
}
