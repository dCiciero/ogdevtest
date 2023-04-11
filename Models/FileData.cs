using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HalogenPreTestAPI.Models;

public class FileData
{
    public Guid Id { get; set; }
    public string? UploadedBy { get; set; }
    public string? FileName { get; set; }
    public string? FileExtention { get; set; }
    public string? FilePath { get; set; }
    public DateTime DateUploaded { get; set; }
    public string? Description { get; set; }
    public string? ContentType { get; set; }
}