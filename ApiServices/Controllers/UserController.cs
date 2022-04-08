using ApiServices.IRepository;
using ApiServices.Models; 
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Text.RegularExpressions;

namespace ApiServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IConfiguration _config;
        IUserRepository _oUserRepository = null; 
        public static IWebHostEnvironment _webHostEnvironment;
        
        public UserController(IConfiguration config,IUserRepository oUserRepository, IWebHostEnvironment webHostEnvironment)
        {
            _config = config;
            _oUserRepository = oUserRepository;
            _webHostEnvironment = webHostEnvironment;
        }
         

        [HttpPost]
        [Route("SignUp")]
        public async Task<IActionResult> SignUp([FromBody] User model)
        {
            try
            {
                Regex rgx = new Regex("[^A-Za-z0-9_]");

                bool hasSpecialChars = rgx.IsMatch(model.Username.ToString());

                if (model.Username.Count() > 12 || model.Username == null) 
                    return Ok("Fail username <= 12"); 
                else if( hasSpecialChars == true) 
                    return Ok("Fail requitement A-Za-z0-9_ ");  
                else if(model.Password != null)
                {
                    if (Regex.Matches(model.Password, @"[a-zA-Z]").Count != 0) //is not string
                        return Ok("Password requitement number");
                    if (model.Password.Count() >= 6)
                    {
                        model.Password = Common.EnDeCrypet.ConvertToEncrypt(model.Password);
                        model = await _oUserRepository.Save(model);
                        return Ok(model);
                    } 
                    else return Ok(" Password >= 6");
                }
                else
                {
                    return Ok("Fail");
                }
                 
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }

        }
         

        [HttpGet]
        [Route("Signin/{username}/{password}")]
        public async Task<IActionResult> Signin(string username, string password)
        {
            try
            {
               
                User model = new User()
                {
                    Username = username, 
                    Password = password
                };
                var user = await AuthenticationUser(model);
                if(user.UserId == 0) return StatusCode((int)HttpStatusCode.NotFound,"Invalid user");
                user.Token = GenereteToken(model);
                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }

        }

        [HttpPost("[action]")]
        public IActionResult UploadFiles(List<IFormFile> files)
        {
            if (files.Count == 0) return BadRequest();
                string directoryPath = Path.Combine(_webHostEnvironment.ContentRootPath, "Photos");

                foreach (var file in files)
                {
                    string filePath = Path.Combine(directoryPath, file.FileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                }
                return Ok("Upload Successful"); 
        }

        private string GenereteToken(User model)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_config["Jwt:Key"],
                _config["Jwt:Issuer"],
                null,
                expires: DateTime.Now.AddMinutes(120),
                signingCredentials:credentials
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private async Task<User> AuthenticationUser(User user)
        {
            return await _oUserRepository.GetByUsernamePassword(user);
        }
         


    }
}
