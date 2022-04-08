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
    public class PassSetRepository : IPassSetRepository
    {
        string _connectionString = "";
        PassSet _oPassSet = new PassSet();
        List<PassSet> _oPassSets = new List<PassSet>();
        public PassSetRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("SQLDatabase");
        }
        public async Task<List<PassSet>> ReSetPass(PassSet pass, string newpassword)
        {
            string Newpass = newpassword;
            _oPassSet = new PassSet();

            using (IDbConnection con = new SqlConnection(_connectionString))
            {
                if (con.State == ConnectionState.Closed) con.Open();
                string query = string.Format(@"select top 5 * from logs_password where UserId='{0}' order by logId desc ", pass.UserId);
                var PassSets = await con.QueryAsync<PassSet>(query);

                if (PassSets != null && PassSets.Count() > 0)
                {
                    foreach (var ps in PassSets)
                    {
                        if (Newpass == Common.EnDeCrypet.ConvertToDecrypt(ps.passwordOld))
                        {
                            return null;
                        }

                    }
                }

                var SaveThis = await con.QueryAsync<PassSet>("SP_ResetPass",
                        this.SetParameters(pass, newpassword),
                        commandType: CommandType.StoredProcedure);
            }

            return _oPassSets;
        }

        private DynamicParameters SetParameters(PassSet oPassSet, string newpassword)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@UserId", oPassSet.UserId);
            parameters.Add("@Password", Common.EnDeCrypet.ConvertToEncrypt(newpassword));
            return parameters;

        }

    }
}
