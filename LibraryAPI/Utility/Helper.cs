using LibraryAPI.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace LibraryAPI.Utility
{
    public class Helper
    {
        public static T ADONetToClass<T>(SqlDataReader reader) where T : new()
        {
            var entity = new T();
            var type = typeof(T);
            var properties = type.GetProperties();

            foreach (var property in properties)
            {
                if (property.Name == "ID")
                {
                    property.SetValue(entity, Guid.Parse(reader[property.Name].ToString()));
                }
                else
                {
                    property.SetValue(entity, Convert.ChangeType(reader[property.Name], property.PropertyType));
                }
            }

            return entity;
        }

        public static User AuthenticateUser(User login)
        {
            User user = null;

            if (login.UserName == "kamalguliyev06" && login.Password == "123")
            {
                user = new User
                {
                    UserName = "kamalguliyev06",
                    Password = "123",
                    Email = "kamalguliyev06@gmail.com"
                };
            }

            return user;
        }

        public static string GenerateJSONWebToken(User user, IConfiguration _configuration)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Issuer"],
                claims,
                expires: DateTime.Now.AddMinutes(120),
                signingCredentials: credentials);

            var encodeToken = new JwtSecurityTokenHandler().WriteToken(token);
            return encodeToken;
        }
    }
}
