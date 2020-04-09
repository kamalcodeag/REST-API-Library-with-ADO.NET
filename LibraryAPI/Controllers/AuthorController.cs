using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibraryAPI.Models;
using LibraryAPI.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace LibraryAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthorController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public AuthorController(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("BookAPIDatabase");
        }

        [HttpGet]
        public ActionResult<IEnumerable<Author>> GetListOfAuthors()
        {
            var authors = new List<Author>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand("select * from Authors", connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    authors.Add(Helper.ADONetToClass<Author>(reader));
                }
            }

            return Ok(authors);
        }

        [HttpGet("{id}")]
        public ActionResult<Author> GetAuthor(string id)
        {
            Author author = new Author();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand("select * from Authors where ID = @id", connection);
                command.Parameters.AddWithValue("@id", id);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    author = Helper.ADONetToClass<Author>(reader);
                }
            }

            return Ok(author);
        }

        [HttpPost]
        public ActionResult<Author> PublishAuthor(Author author)
        {
            var type = typeof(Author);
            var properties = type.GetProperties();
            var ID = Guid.NewGuid().ToString();
            StringBuilder commandText = new StringBuilder("insert into Authors values(@ID, ");

            SqlConnection connection = new SqlConnection();
            connection.ConnectionString = _connectionString;
            SqlCommand command = new SqlCommand();
            command.Connection = connection;

            for (int i = 1; i < properties.Length; i++)
            {
                if (i != properties.Length - 1)
                {
                    commandText.Append($"@{properties[i].Name}, ");
                }
                else
                {
                    commandText.Append($"@{properties[i].Name})");
                }
            }

            command.CommandText = commandText.ToString();

            for (int i = 0; i < properties.Length; i++)
            {
                if (i == 0)
                {
                    command.Parameters.AddWithValue($"{@properties[i].Name}", ID);
                }
                else
                {
                    command.Parameters.AddWithValue($"{@properties[i].Name}", properties[i].GetValue(author));
                }
            }

            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();

            return CreatedAtAction("GetAuthor", new { id = ID }, author);
        }

        [HttpPut("{id}")]
        public ActionResult UpdateAuthor(string id, Author author)
        {
            var type = typeof(Author);
            var properties = type.GetProperties();

            StringBuilder commandText = new StringBuilder("update Authors set ");

            SqlConnection connection = new SqlConnection();
            connection.ConnectionString = _connectionString;
            SqlCommand command = new SqlCommand();
            command.Connection = connection;

            for (int i = 1; i < properties.Length; i++)
            {
                if (i != properties.Length - 1)
                {
                    commandText.Append($"{properties[i].Name} = @{properties[i].Name}, ");
                }
                else
                {
                    commandText.Append($"{properties[i].Name} = @{properties[i].Name} where {properties[0].Name} = @id");
                }
            }

            command.CommandText = commandText.ToString();

            for (int i = 0; i < properties.Length; i++)
            {
                if (i == 0)
                {
                    command.Parameters.AddWithValue("@id", id);
                }
                else
                {
                    command.Parameters.AddWithValue($"{@properties[i].Name}", properties[i].GetValue(author));
                }
            }

            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteAuthor(string id)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand("delete from Authors where ID = @id", connection);
                command.Parameters.AddWithValue("@id", id);
                connection.Open();
                command.ExecuteNonQuery();
            }

            return NoContent();
        }
    }
}