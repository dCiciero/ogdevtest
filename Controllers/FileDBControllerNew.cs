/* using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HalogenPreTestAPI.Models;

namespace HalogenPreTestAPI.Controllers
{
    [Route("api/[controllers]")]
    [ApiController]
    public class FileDBController : ControllerBase
    {
        private readonly FileDBContext _context;

        public FileDBController(FileDBContext context)
        {
            _context = context;
        }

        // GET: api/FileDB
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FileData>>> GetFileData()
        {
          if (_context.FileData == null)
          {
              return NotFound();
          }
            return await _context.FileData.ToListAsync();
        }

        // GET: api/FileDB/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FileData>> GetFileData(Guid id)
        {
          if (_context.FileData == null)
          {
              return NotFound();
          }
            var fileData = await _context.FileData.FindAsync(id);

            if (fileData == null)
            {
                return NotFound();
            }

            return fileData;
        }

        // PUT: api/FileDB/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFileData(Guid id, FileData fileData)
        {
            if (id != fileData.Id)
            {
                return BadRequest();
            }

            _context.Entry(fileData).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FileDataExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/FileDB
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<FileData>> PostFileData(FileData fileData)
        {
          if (_context.FileData == null)
          {
              return Problem("Entity set 'FileDBContext.FileData'  is null.");
          }
            _context.FileData.Add(fileData);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetFileData", new { id = fileData.Id }, fileData);
        }

        // DELETE: api/FileDB/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFileData(Guid id)
        {
            if (_context.FileData == null)
            {
                return NotFound();
            }
            var fileData = await _context.FileData.FindAsync(id);
            if (fileData == null)
            {
                return NotFound();
            }

            _context.FileData.Remove(fileData);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool FileDataExists(Guid id)
        {
            return (_context.FileData?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
 */