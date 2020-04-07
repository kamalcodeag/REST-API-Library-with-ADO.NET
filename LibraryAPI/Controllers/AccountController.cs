using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
            ActionResult response = Unauthorized();

            bool userExists = Helper.AuthenticateUser(username, Helper.GetHash(password), _connectionString);

            if (userExists)
            {
                var token = Helper.GenerateJSONWebToken(username, _configuration);
                response = Ok(new { token = token });
            }

            return response;
        }

        [HttpPost]
        public ActionResult<User> Register(User user)
        {
            bool userExists = Helper.CheckUserExists(user.UserName, _connectionString);

            if(userExists)
            {
                return Conflict();
            }

            string ID = Guid.NewGuid().ToString();
            string FullName = user.FullName;
            string UserName = user.UserName;
            string HashedPassword = Helper.GetHash(user.Password);
            bool IsLocked = false;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand("insert into Users values(@ID, @FullName, @UserName, @HashedPassword, @IsLocked)", connection);
                command.Parameters.AddWithValue("@ID", ID);
                command.Parameters.AddWithValue("@FullName", FullName);
                command.Parameters.AddWithValue("@UserName", UserName);
                command.Parameters.AddWithValue("@HashedPassword", HashedPassword);
                command.Parameters.AddWithValue("@IsLocked", IsLocked);
                connection.Open();
                command.ExecuteNonQuery();
            }

            return CreatedAtAction("Login", new { username = UserName }, user);
        }
    }
}
