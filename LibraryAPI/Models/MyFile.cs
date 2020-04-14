using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryAPI.Models
{
    public class MyFile
    {
        public IFormFile UploadFile { get; set; }
    }
}
