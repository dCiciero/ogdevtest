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
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.StaticFiles;
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
        List<double> divBy3 = new List<double>();
        List<double> divBy5 = new List<double>();
        List<double> divBy7 = new List<double>();
        List<double> evenNums = new List<double>();
        List<double> oddNums = new List<double>();
        double modeOfNums = 0;
        double medianOfNums = 0;
        private readonly string AppDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        private static List<FileData> DbFile = new List<FileData>();

        public FileDBController(FileDBContext context)
        {
            _context = context;
        }

        [HttpPost("/sendfile")]
        [Consumes("multipart/form-data")]
        public async Task<HttpResponseMessage> PostAsync([FromForm] FileDB model)
        {
            try
            {

                FileData file = await SaveFileAsync(model.File2Process);
                if (!string.IsNullOrEmpty(file.FilePath))
                {
                    file.Description = model.Description;
                    // file.AltText = model.AltText;
                    DbFile.Add(file);
                    SaveToDB(file);
                    processFileContent();
                    return new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(divBy3.ToString())
                    };
                }
                else
                {

                    return new HttpResponseMessage(HttpStatusCode.BadRequest)
                    {
                        Content = new StringContent("Error Occured Here!!!!!!")
                    };
                }
            }
            catch (Exception ex)
            {

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
                using (var reader = new StreamReader(new FileStream(lookUpFile, FileMode.Open)))
                {
                    List<double> numbersList = new List<double>();
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        var values = line.Split(',');
                        var numbaItem = 0.0;
                        foreach (var item in values)
                        {
                            if (Double.TryParse(item, out numbaItem))
                                numbersList.Add(numbaItem);
                        }
                        numbersList.Sort();
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
                        }


                        var groups = numbersList.GroupBy(v => v);
                        int maxCount = groups.Max(g => g.Count());
                        // System.Console.WriteLine($"Max Count: {maxCount}");
                        modeOfNums = maxCount > 1 ? groups.First(g => g.Count() == maxCount).Key : 0;
                        // modeOfNums = numbersList
                        //                 .GroupBy(x => x)
                        //                 .OrderByDescending(x => x.Count())
                        //                 // .Select(x => x.Key)
                        //                 .First().Key;

                        medianOfNums = numbersList
                                        .OrderBy(item => item)
                                        .Skip((numbersList.ToArray().Length - 1) / 2)
                                        .Take(2 - numbersList.ToArray().Length % 2)
                                        .Average();

                    }

                }
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
                file.ContentType = fileData.ContentType;

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
            fileData.ContentType = record.ContentType;

            _context.FileData.Add(fileData);
            _context.SaveChanges();
        }


        [HttpGet("/showfile")]
        public async Task<IActionResult> ShowUploadedFile()
        {
            string[] filePaths = Directory.GetFiles(AppDirectory);
            var path = "";
            string? contentType = "";
            if (filePaths.Length != 0)
            {
                var lookUpFile = filePaths[filePaths.Length - 1];
                path = Path.Combine(AppDirectory, lookUpFile);

                new FileExtensionContentTypeProvider().TryGetContentType(path, out contentType);
            }
            var retData = new Dictionary<string, string>(){
                {"fileName", Path.GetFileName(path)},
                {"CntentType", contentType}
            };

            return Ok(retData.ToList());

        }

        [HttpGet("/downloadFile")]
        public async Task<IActionResult> DownLoadFile()
        {
            if (!Directory.Exists(AppDirectory))
                Directory.CreateDirectory(AppDirectory);

            string[] filePaths = Directory.GetFiles(AppDirectory);
            var lookUpFile = Path.Combine(AppDirectory, filePaths[filePaths.Length - 1]);
            var path = Path.Combine(AppDirectory, lookUpFile);

            var memory = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            var contentType = "Application/octet-stream";
            var fileName = Path.GetFileName(path);

            return File(memory, contentType, fileName);
        }

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
        }

        private bool FileDBExists(Guid id)
        {
            return (_context.FileData?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}