using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using LibraryAPI.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LibraryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        //[HttpPost]
        //public async Task<ActionResult> Save([FromForm] MyFile myFile)
        //{
        //    #region Uploading file by FileStream
        //    //using (FileStream fileStream = new FileStream("D:\\" + myFile.UploadFile.FileName, FileMode.Create))
        //    //{
        //    //    await myFile.UploadFile.CopyToAsync(fileStream);
        //    //}
        //    #endregion

        //    return Ok();
        //}

        //[HttpPost]
        //public async Task<ActionResult> Save()
        //{
        //    #region Creating and appending .txt file by FileStream
        //    //using (FileStream fileStream = new FileStream("D:\\testtt.txt", FileMode.Create))
        //    //{
        //    //    byte[] text = Encoding.UTF8.GetBytes("test");
        //    //    await fileStream.WriteAsync(text, 0, text.Length);
        //    //}

        //    //using (FileStream fileStream = new FileStream("D:\\testtt.txt", FileMode.Append))
        //    //{
        //    //    byte[] text = Encoding.UTF8.GetBytes(" oooooooooooooooo \r\n");
        //    //    await fileStream.WriteAsync(text, 0, text.Length);
        //    //}
        //    #endregion

        //    #region Creating, appending and reading .txt file by StreamWriter, StreamReader
        //    //using (StreamWriter sw = System.IO.File.CreateText("D:\\abcd.txt"))
        //    //{
        //    //    await sw.WriteLineAsync("abcd");
        //    //}

        //    //using (StreamWriter sw = System.IO.File.AppendText("D:\\abcd.txt"))
        //    //{
        //    //    await sw.WriteAsync("1234");
        //    //}

        //    //using (StreamReader sr = System.IO.File.OpenText("D:\\abcd.txt"))
        //    //{
        //    //    string text = "";
        //    //    while ((text = await sr.ReadLineAsync()) != null)
        //    //    {
        //    //        Debug.WriteLine(text);
        //    //    }
        //    //}
        //    #endregion

        //    return Ok();
        //}

        //[HttpPost]
        //public ActionResult Download()
        //{
        //    #region Downloading file by WebClient
        //    using (WebClient wc = new WebClient())
        //    {
        //        wc.DownloadFileAsync(new System.Uri("http://www.kirayemlak.az/MainPageFiles/images/announcements/estate93.jpg"), "D:\\estate93.jpg");
        //    }
        //    #endregion

        //    return Ok();
        //}
    }
}