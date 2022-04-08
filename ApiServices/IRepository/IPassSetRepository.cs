using ApiServices.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiServices.IRepository
{
    public interface IPassSetRepository
    {
        Task<List<PassSet>> ReSetPass(PassSet obj, string newpassword); 
    }
}
