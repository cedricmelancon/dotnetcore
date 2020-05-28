using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using UserApplication.Models;

namespace UserApplication.Services
{
    public class UserService : IUserService
    {
        private readonly ILogger<IUserService> _logger;

        public UserService(ILogger<IUserService> logger)
        {
            _logger = logger;
        }

        public IEnumerable<UserModel> GetUsers()
        {
            var users = new List<UserModel>
            { new UserModel
                {
                    FirstName = "Cedric",
                    LastName = "Melancon",
                    Address = "1425, boul. René-Lévesques Ouest, bureau 240",
                    Description = "Cool Guy",
                    PhoneNumber = "1-514-123-4567"
                },
                new UserModel
                {
                    FirstName = "John",
                    LastName = "Doe",
                    Address = "38, rue des Perdus",
                    Description = "Lost Guy",
                    PhoneNumber = "911"
                }
            };

            _logger.LogInformation("Returning {count} elements", users.Count);
            return users;
        }
    }
}
