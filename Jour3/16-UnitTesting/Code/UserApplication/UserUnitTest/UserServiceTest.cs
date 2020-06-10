using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserApplication.Data;
using UserApplication.Data.Models;
using UserApplication.Services;
using Xunit;

namespace UserUnitTest
{
    public class UserServiceTest
    {
        private readonly IUserService _service;
        private Mock<ILogger<IUserService>> _loggerMock;
        private Mock<IUnitOfWork> _unitOfWorkMock;

        public UserServiceTest()
        {
            _loggerMock = new Mock<ILogger<IUserService>>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();

            _unitOfWorkMock.Setup(u => u.GetRepository<UserModel>()).Returns(new UserRepositoryMock());

            _service = new UserService(_loggerMock.Object, _unitOfWorkMock.Object);
        }

        [Fact]
        public void TestGetUser()
        {
            var users = _service.GetUsers().ToList();

            Assert.True(users.Count() == 1);
            Assert.True(users[0].FirstName == "Cedric");
        }

        [Fact]
        public async Task TestAddUser()
        {
            var newUser = new UserModel
            {
                Id = Guid.NewGuid(),
                FirstName = "John",
                LastName = "Doe",
                Addresses = new List<AddressModel>(),
                Description = "Homeless",
                PhoneNumber = ""
            };

            await _service.AddUserAsync(newUser);

            var users = _service.GetUsers().ToList();

            Assert.True(users.Count() == 2);
            Assert.NotNull(users.SingleOrDefault(u => u.Id == newUser.Id));
            Assert.Equal(users.SingleOrDefault(u => u.Id == newUser.Id), newUser);
        }
    }
}
