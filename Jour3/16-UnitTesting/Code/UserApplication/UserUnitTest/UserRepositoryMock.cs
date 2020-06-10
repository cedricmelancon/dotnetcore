using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using UserApplication.Data;
using UserApplication.Data.Models;

namespace UserUnitTest
{
    public class UserRepositoryMock : IRepository<UserModel>
    {
        private IList<UserModel> _users = new List<UserModel>
        {
            new UserModel
            {
                Id = Guid.NewGuid(),
                FirstName = "Cedric",
                LastName = "Melancon",
                Addresses = new List<AddressModel>
                {
                    new AddressModel
                    {
                        Line1 = "1425, boul. René-Lévesques Ouest",
                        Line2 = "Bureau 240",
                        City = "Montréal",
                        Province = "Québec",
                        ZipCode = "H3C 1T7",
                        Description = "Office"
                    }
                },
                PhoneNumber = "514-437-9018"
            }
        };

        public UserRepositoryMock()
        {

        }

        public IEnumerable<UserModel> GetAll()
        {
            return _users;
        }

        public int Count(Expression<Func<UserModel, bool>> predicate = null)
        {
            throw new NotImplementedException();
        }

        public UserModel GetById(object id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<UserModel> Get(Expression<Func<UserModel, bool>> filter = null, Func<IQueryable<UserModel>, IOrderedQueryable<UserModel>> orderBy = null, string includeProperties = "", bool asNoTracking = true)
        {
            return _users;
        }

        public async Task InsertAsync(UserModel entity, CancellationToken cancellationToken = default(CancellationToken))
        {
            _users.Add(entity);
        }

        public void Update(UserModel entity)
        {
            var index = _users.IndexOf(entity);
            _users[index] = entity;
        }

        public void Delete(UserModel entity)
        {
            _users.Remove(entity);
        }

        public void DeleteById(object id)
        {
            var user = _users.SingleOrDefault(u => u.Id == (Guid)id);

            if (user != null)
            {
                var index = _users.IndexOf(user);
                _users.RemoveAt(index);
            }
        }
    }
}
