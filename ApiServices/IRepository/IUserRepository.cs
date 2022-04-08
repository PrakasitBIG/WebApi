using ApiServices.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiServices.IRepository
{
    public interface IUserRepository
    {
        Task<User> Save(User obj);

        Task<User> GetByUsernamePassword(User user);

    }
}
