using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using LibraryAPI.Models;
using LibraryAPI.Utility;
using Newtonsoft.Json;

namespace LibraryAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public BookController(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("BookAPIDatabase");
        }

        [HttpGet]
        public ActionResult<IEnumerable<Book>> GetListOfBooks()
        {
            #region XML Converter
            //XmlSerializer xsSubmit = new XmlSerializer(typeof(Person));
            //var xml = "";

            //using (var sww = new StringWriter())
            //{
            //    using (XmlWriter writer = XmlWriter.Create(sww))
            //    {
            //        xsSubmit.Serialize(writer, person);
            //        xml = sww.ToString(); // Your XML
            //    }
            //}

            //return xml;
            #endregion

            //By manual
            //var books = new List<Book>();

            var books = new List<Book>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                //SqlCommand command = new SqlCommand("select * from Books", connection);
                SqlCommand command = new SqlCommand(@"select Books.*, Authors.FullName as AuthorName from Books
                                                      join BookAuthors on Books.ID = BookAuthors.BooksID
                                                      join Authors on Authors.ID = BookAuthors.AuthorsID", connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    #region By manual
                    //Book book = new Book();

                    //book.ID = Guid.Parse(reader["ID"].ToString());
                    //book.BookName = reader["BookName"].ToString();
                    //book.Genre = reader["Genre"].ToString();
                    //book.ReleaseDate = Convert.ToDateTime(reader["ReleaseDate"]);

                    //books.Add(book);
                    #endregion

                    books.Add(Helper.ADONetToClass<Book>(reader));
                }
            }

            return Ok(books);
        }

        [HttpGet("{id}")]
        public ActionResult<Book> GetBook(string id)
        {
            Book book = new Book();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                //SqlCommand command = new SqlCommand("select * from Books where ID = @id", connection);
                SqlCommand command = new SqlCommand(@"select Books.*, Authors.FullName as AuthorName from Books
                                                        join BookAuthors on Books.ID = BookAuthors.BooksID
                                                        join Authors on Authors.ID = BookAuthors.AuthorsID
                                                        where BookAuthors.BooksID = @BooksId", connection);
                //command.Parameters.AddWithValue("@id", id);
                command.Parameters.AddWithValue("@BooksId", id);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    book = Helper.ADONetToClass<Book>(reader);
                }
            }

            return Ok(book);
        }

        [HttpGet("byauthor/{authorName}")]
        public ActionResult<IEnumerable<Book>> GetBookByAuthorName(string authorName)
        {
            Author author = new Author();
            string authorId = null;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand("select * from Authors where FullName = @FullName", connection);
                command.Parameters.AddWithValue("@FullName", authorName);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    author = Helper.ADONetToClass<Author>(reader);
                    authorId = author.ID.ToString();
                }
            }

            List<Book> books = new List<Book>();
            Book book = null;

            if (authorId != null)
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    SqlCommand command = new SqlCommand(@"select Books.*, Authors.FullName as AuthorName from Books
                                                        join BookAuthors on Books.ID = BookAuthors.BooksID
                                                        join Authors on Authors.ID = BookAuthors.AuthorsID
                                                        where BookAuthors.AuthorsID = @AuthorId", connection);
                    command.Parameters.AddWithValue("@AuthorId", authorId);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        book = Helper.ADONetToClass<Book>(reader);
                        books.Add(book);
                    }
                }
            }
            else
            {
                return NotFound();
            }

            return Ok(books);
        }

        [HttpPost]
        public ActionResult<Book> PublishBook(Book book)
        {
            #region By manual
            //var ID = Guid.NewGuid().ToString();
            //var BookName = book.BookName;
            //var Genre = book.Genre;
            //var ReleaseDate = book.ReleaseDate;

            //using (SqlConnection connection = new SqlConnection(_connectionString))
            //{
            //    SqlCommand command = new SqlCommand("insert into Books values(@ID, @BookName, @Genre, @ReleaseDate)", connection);
            //    command.Parameters.AddWithValue("@ID", ID);
            //    command.Parameters.AddWithValue("@BookName", BookName);
            //    command.Parameters.AddWithValue("@Genre", Genre);
            //    command.Parameters.AddWithValue("@ReleaseDate", ReleaseDate);
            //    connection.Open();
            //    command.ExecuteNonQuery();
            //}
            #endregion

            var type = typeof(Book);
            var properties = type.GetProperties();
            var ID = Guid.NewGuid().ToString();
            StringBuilder commandText = new StringBuilder("insert into Books values(@ID, ");

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
                    command.Parameters.AddWithValue($"{@properties[i].Name}", properties[i].GetValue(book));
                }
            }

            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();

            return CreatedAtAction("GetBook", new { id = ID }, book);
        }

        [HttpPut("{id}")]
        public ActionResult UpdateBook(string id, Book book)
        {
            var type = typeof(Book);
            var properties = type.GetProperties();

            StringBuilder commandText = new StringBuilder("update Books set ");
            //update Books set BookName = @BookName, Genre = @Genre, ReleaseDate = @ReleaseDate where ID = @id

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
                    command.Parameters.AddWithValue($"{@properties[i].Name}", properties[i].GetValue(book));
                }
            }

            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteBook(string id)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand("delete from Books where ID = @id", connection);
                command.Parameters.AddWithValue("@id", id);
                connection.Open();
                command.ExecuteNonQuery();
            }

            return NoContent();
        }
    }
}
