using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UserApplication.Data;
using UserApplication.Data.Models;

namespace UserApplication.Services
{
    public class UserService : IUserService
    {
        private readonly ILogger<IUserService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<UserModel> _userRepository;

        public UserService(ILogger<IUserService> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _userRepository = _unitOfWork.GetRepository<UserModel>();
        }

        public IEnumerable<UserModel> GetUsers()
        {
            _logger.LogInformation("Getting all users.");
            return _userRepository.Get(includeProperties: "Addresses");
        }

        public async Task AddUserAsync(UserModel user)
        {
            _logger.LogInformation("Adding user {first} {last}", user.FirstName, user.LastName);
            await _userRepository.InsertAsync(user);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateUserAsync(UserModel user)
        {
            _logger.LogInformation("Modifying the user {id}", user.Id);
            _userRepository.Update(user);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteUserAsync(Guid id)
        {
            _userRepository.DeleteById(id);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
