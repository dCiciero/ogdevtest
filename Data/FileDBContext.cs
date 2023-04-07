using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using HalogenPreTestAPI.Models;

namespace HalogenPreTestAPI.Data;
public class FileDBContext : DbContext
{
    public FileDBContext(DbContextOptions<FileDBContext> options)
        : base(options)
    {
    }

    public DbSet<FileData> FileData { get; set; } = default!;
}
