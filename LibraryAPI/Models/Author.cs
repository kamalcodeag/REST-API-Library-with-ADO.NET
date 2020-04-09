using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryAPI.Models
{
    public class Author
    {
        public Guid ID { get; set; }
        [Required]
        public string FullName { get; set; }
        [Required]
        public string Nationality { get; set; }
        [Required, DataType(DataType.DateTime)]
        public DateTime BirthDate { get; set; }
    }
}
