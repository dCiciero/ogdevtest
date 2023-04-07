using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HalogenPreTestAPI.Models;
using HalogenPreTestAPI.Data;
using Microsoft.AspNetCore.Cors;

namespace HalogenPreTestAPI.Controllers
{
    [Route("api/[controller]")]
    [EnableCors("policyHalogenPreTest")]
    [ApiController]
    public class FileDBController : ControllerBase
    {
        private readonly FileDBContext _context;
        List<int> divBy3 = new List<int>();
        List<int> divBy5 = new List<int>();
        List<int> divBy7 = new List<int>();
        List<int> evenNums = new List<int>();
        List<int> oddNums = new List<int>();
        int modeOfNums = 0;
        double medianOfNums = 0;
        private readonly string AppDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        private static List<FileData> DbFile = new List<FileData>();
        // private IWebHostingEnvironment webHostingEnvironment; IWebHostingEnvironment _webHostingEnvironment

        public FileDBController(FileDBContext context)
        {
            _context = context;
            // webHostingEnvironment = _webHostingEnvironment;
        }

        [HttpPost("/sendfile")]
        // [EnableCors("policyHalogenPreTest")]
        [Consumes("multipart/form-data")]
        public async Task<HttpResponseMessage> PostAsync([FromForm] FileDB model)
        {
            try
            {
                // return new HttpResponseMessage(HttpStatusCode.OK)
                // {
                //     Content = new StringContent("Successful")
                // };
                FileData file = await SaveFileAsync(model.File2Process);
                if (!string.IsNullOrEmpty(file.FilePath))
                {
                    file.Description = model.Description;
                    file.AltText = model.AltText;
                    DbFile.Add(file);
                    // SaveToDB(file);
                    Console.WriteLine(file.FileName + "\n" + file.FilePath);
                    processFileContent();
                    return new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(divBy3.ToString())
                    };
                }
                else
                {
                    // return new List<int> { 0 };
                    return new HttpResponseMessage(HttpStatusCode.BadRequest)
                    {
                        Content = new StringContent("Error Occured Here!!!!!!")
                    };
                }
            }
            catch (Exception ex)
            {
                // return new List<int> { 0 };
                return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(ex.Message),
                };
            }
        }

        private int processFileContent()
        {


            string[] filePaths = Directory.GetFiles(AppDirectory);
            if (filePaths.Length == 0)
                return -1;
            var lookUpFile = Path.Combine(AppDirectory, filePaths[filePaths.Length - 1]);
            if (Path.Exists(lookUpFile))
            {
                // new FileStream(path, FileMode.Open))
                using (var reader = new StreamReader(new FileStream(lookUpFile, FileMode.Open)))
                {
                    List<int> numbersList = new List<int>();
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        var values = line.Split(',');
                        var numbaItem = 0;
                        foreach (var item in values)
                        {
                            if (Int32.TryParse(item, out numbaItem))
                                numbersList.Add(numbaItem);
                        }
                        foreach (var numba in numbersList)
                        {
                            if (numba % 3 == 0)
                                divBy3.Add(numba);
                            if (numba % 5 == 0)
                                divBy5.Add(numba);
                            if (numba % 7 == 0)
                                divBy7.Add(numba);
                            if (numba % 2 == 0)
                                evenNums.Add(numba);
                            else
                                oddNums.Add(numba);
                            // Console.WriteLine(numba);
                        }

                        modeOfNums = numbersList
                                        .GroupBy(x => x)
                                        .OrderByDescending(x => x.Count())
                                        .Select(x => x.Key)
                                        .First();

                        medianOfNums = numbersList
                                        .OrderBy(item => item)
                                        .Skip((numbersList.ToArray().Length - 1) / 2)
                                        .Take(2 - numbersList.ToArray().Length % 2)
                                        .Average();
                        // var findMode = numbersList.ToLookup(x => x);
                        // var allModes = findMode.Max(x => x.Count());
                        // modeOfNums = findMode.Where(x => x.Count() == allModes).Select(x => x.Key).First();
                        Console.WriteLine($"Numbers div by 3: {string.Join(",", divBy3)}\nNumbers div by 5: {string.Join(",", divBy5)}\nNumbers div by 7: {string.Join(",", divBy7)}");
                    }

                }
                // var reader = new StreamReader(new FileStream(lookUpFile, FileMode.Open));
            }
            return 0;
        }

        private async Task<FileData> SaveFileAsync(IFormFile fileData)
        {
            FileData file = new FileData();
            if (fileData != null)
            {
                if (!Directory.Exists(AppDirectory))
                    Directory.CreateDirectory(AppDirectory);

                var fileName = DateTime.Now.Ticks.ToString() + Path.GetExtension(fileData.FileName);
                var path = Path.Combine(AppDirectory, fileName);

                file.Id = Guid.NewGuid(); //fileDB.Count()+1; //
                file.FilePath = fileName;
                file.FileExtention = Path.GetExtension(fileData.FileName);
                file.DateUploaded = DateTime.Now;
                file.FileName = Path.GetFileName(fileData.FileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await fileData.CopyToAsync(stream);
                }
                return file;
            }
            return file;
        }

        private void SaveToDB(FileData record)
        {
            if (record == null)
                throw new ArgumentNullException($"{nameof(record)}");

            FileData fileData = new FileData();
            fileData.FilePath = record.FilePath;
            fileData.FileName = record.FileName;
            fileData.FileExtention = record.FileExtention;

            _context.FileData.Add(fileData);
            _context.SaveChanges();
        }

        // [HttpGet] LATER
        // public List<FileData> GetAllFiles()
        // {

        // }

        /* [HttpGet("/downloadFile/{id}")]
        public async Task<IActionResult> DownLoadFile(Guid id)
        {
            if (!Directory.Exists(AppDirectory))
                Directory.CreateDirectory(AppDirectory);

            var file = _context.FileData.Where(n => n.Id == id).FirstOrDefault();
            var path = Path.Combine(AppDirectory, file?.FilePath);

            var memory = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            var contentType = "Application/0ctet-stream";
            var fileName = Path.GetFileName(path);

            return File(memory, contentType, fileName);
        } */

        [HttpGet("/processOutput")]
        public async Task<IActionResult> ProcessData()
        {
            var success = processFileContent();
            if (success == -1)
                return BadRequest("File not uploaded");


            var calculatedValues = new List<ObjectModel>{
                new ObjectModel
                {
                    DivBy3 = divBy3,
                    DivBy5 = divBy5,
                    DivBy7 = divBy7,
                    EvenNums = evenNums,
                    OddNums = oddNums,
                    Mode = modeOfNums,
                    Median = medianOfNums

                }
            };

            return Ok(calculatedValues);

            // if (!Directory.Exists(AppDirectory))
            //     Directory.CreateDirectory(AppDirectory);

            // var file = _context.FileData.Where(n => n.Id == id).FirstOrDefault();
            // var path = Path.Combine(AppDirectory, file?.FilePath);

            // var memory = new MemoryStream();
            // using (var stream = new FileStream(path, FileMode.Open))
            // {
            //     await stream.CopyToAsync(memory);
            // }
            // memory.Position = 0;
            // var contentType = "Application/0ctet-stream";
            // var fileName = Path.GetFileName(path);

            // return File(memory, contentType, fileName);
        }

        private bool FileDBExists(Guid id)
        {
            return (_context.FileData?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
