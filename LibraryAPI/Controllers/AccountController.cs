using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using LibraryAPI.Models;
using LibraryAPI.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace LibraryAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public AccountController(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("BookAPIDatabase");
        }

        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            User login = new User();
            login.UserName = username;
            login.Password = password;
            ActionResult response = Unauthorized();

            var user = Helper.AuthenticateUser(login);

            if(user != null)
            {
                var token = Helper.GenerateJSONWebToken(user, _configuration);
                response = Ok(new { token = token });
            }

            return response;
        }
    }
}
