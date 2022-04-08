using ApiServices.IRepository;
using ApiServices.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ApiServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PassSetController : ControllerBase
    {
        private IConfiguration _config;
        IPassSetRepository _oPassSetRepository = null;

        public PassSetController(IConfiguration config, IPassSetRepository oPassSetRepository)
        {
            _config = config;
            _oPassSetRepository = oPassSetRepository;
        }

        [HttpPost]
        [Route("ResetPassword/{userid}/{password}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> ResetPassword(int userid, string password)
        {
            try
            {
                PassSet model = new PassSet()
                {
                    UserId = userid,
                };

                var user = await ResetPasswordUser(model, password);
                if (user == null)
                    return StatusCode((int)HttpStatusCode.NotFound, "Password Not Use");

                return Ok("Success");
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }

        }

        private async Task<List<PassSet>> ResetPasswordUser(PassSet passset, string Newpass)
        {
            return await _oPassSetRepository.ReSetPass(passset, Newpass);
        }


    }
}
