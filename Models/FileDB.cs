using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace HalogenPreTestAPI.Models;

public class FileDB
{
    // public long Id { get; set; }
    // [NotMapped]
    public IFormFile? File2Process { get; set; }
    public string? Description { get; set; }
    public string? AltText { get; set; }
    // public string FileName { get; set; }
    // public string FileExtention { get; set; }
    // public string FilePath { get; set; }
    // public DateTime DateUploaded { get; set; }
}