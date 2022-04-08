using ApiServices.Common;
using ApiServices.IRepository;
using ApiServices.Models;
using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace ApiServices.Repository
{
    public class UserRepository : IUserRepository
    {
        string _connectionString = "";
        User _oUser = new User();
        List<User> _oUsers = new List<User>();
        public UserRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("SQLDatabase");
        }
        public async Task<string> Delete(User obj)
        {
            string message = "";
            try
            {
                using (IDbConnection con = new SqlConnection(_connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();
                    var Users = await con.QueryAsync<User>("SP_User",
                        this.SetParameters(obj, (int)OperationType.Delete),
                        commandType: CommandType.StoredProcedure);
                    message = "Deleted";
                }

            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return message;
        }

        public async Task<User> Get(int objId)
        {
            _oUser = new User();
            using (IDbConnection con = new SqlConnection(_connectionString))
            {
                if (con.State == ConnectionState.Closed) con.Open();
                var Users = await con.QueryAsync<User>(string.Format(@"Select * from User Where UserId={0}", objId));
                if (Users != null && Users.Count() > 0)
                {
                    _oUser = Users.SingleOrDefault();
                }
            }

            return _oUser;
        }



        public async Task<List<User>> Gets()
        {
            _oUsers = new List<User>();
            using (IDbConnection con = new SqlConnection(_connectionString))
            {
                if (con.State == ConnectionState.Closed) con.Open();
                var Users = await con.QueryAsync<User>("Select * from [User]");
                if (Users != null && Users.Count() > 0)
                {
                    _oUsers = Users.ToList();
                }
            }
            return _oUsers;
        }



        public async Task<User> Save(User obj)
        {
            _oUser = new User();
            try
            {
                using (IDbConnection con = new SqlConnection(_connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();

                    var Users = await con.QueryAsync<User>("SP_User",
                        this.SetParameters(obj, (int)OperationType.Insert),
                        commandType: CommandType.StoredProcedure);
                    if (Users != null && Users.Count() > 0)
                    {
                        _oUser = Users.SingleOrDefault();
                        _oUser.Password = Common.EnDeCrypet.ConvertToDecrypt(_oUser.Password);
                    }
                }

            }
            catch (Exception ex)
            {
                _oUser = new User();
                _oUser.Message = ex.Message;
            }
            return _oUser;
        }

        private DynamicParameters SetParameters(User oUser, int nOperationType)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@UserId", oUser.UserId);
            parameters.Add("@Username", oUser.Username);
            parameters.Add("@Email", oUser.Email);
            parameters.Add("@Password", oUser.Password);
            parameters.Add("@Fname", oUser.Fname);
            parameters.Add("@LName", oUser.Lname);
            //parameters.Add("@Phofile", oUser.Profile);
            parameters.Add("@OperationType", nOperationType);
            return parameters;
        }

        public async Task<User> GetByUsernamePassword(User user)
        {
            _oUser = new User();
            using (IDbConnection con = new SqlConnection(_connectionString))
            {
                if (con.State == ConnectionState.Closed) con.Open();
                string query = string.Format(@"Select * from [User] where Username='{0}' and password ='{1}'", user.Username, Common.EnDeCrypet.ConvertToEncrypt(user.Password));

                var Users = await con.QueryAsync<User>(query);
                if (Users != null && Users.Count() > 0)
                {
                    _oUser = Users.SingleOrDefault();
                    _oUser.Password = Common.EnDeCrypet.ConvertToDecrypt(_oUser.Password);
                }

            }

            return _oUser;
        }


    }
}
