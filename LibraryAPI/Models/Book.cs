using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryAPI.Models
{
    public class Book
    {
        public Guid ID { get; set; }
        [Required]
        public string BookName { get; set; }
        [Required]
        public string Genre { get; set; }
        [Required, DataType(DataType.DateTime)]
        public DateTime ReleaseDate { get; set; }
        public string AuthorName { get; set; }
    }
}
