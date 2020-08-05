using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ArkEcho.Core;
using ArkEcho.Server;

namespace ArkEcho.Server
{
    [Route("api/[controller]")]
    [ApiController]
    public class MusicFilesController : ControllerBase
    {
        List<MusicFile> files = new List<MusicFile>();

        public MusicFilesController()
        {
            files.Add(new MusicFile() { ID = 1, Title = "Tick" });
            files.Add(new MusicFile() { ID = 2, Title = "Trick" });
            files.Add(new MusicFile() { ID = 3, Title = "Track" });
        }

        // GET: api/MusicFiles
        [HttpGet]
        public async Task<IEnumerable<MusicFile>> GetMusicFiles()
        {
            return await Task.FromResult(files);
        }

        // GET: api/MusicFiles/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MusicFile>> GetMusicFile(long id)
        {
            MusicFile musicFile = files.Find(x => x.ID == id);
            await verifyRequest(HttpContext.Request);

            if (musicFile == null)            
                return NotFound();
            
            return musicFile;
        }

        public async Task<bool> verifyRequest(HttpRequest request)
        {
            return true;
        }

        // PUT: api/MusicFiles/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutMusicFile(long id, MusicFile musicFile)
        //{
        //    if (id != musicFile.ID)
        //    {
        //        return BadRequest();
        //    }

        //    context.Entry(musicFile).State = EntityState.Modified;

        //    try
        //    {
        //        await context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!MusicFileExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return NoContent();
        //}

        // POST: api/MusicFiles
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        //[HttpPost]
        //public async Task<ActionResult<MusicFile>> PostMusicFile(MusicFile musicFile)
        //{
        //    context.MusicFiles.Add(musicFile);
        //    await context.SaveChangesAsync();

        //    return CreatedAtAction("GetMusicFile", new { id = musicFile.ID }, musicFile);
        //}

        // DELETE: api/MusicFiles/5
        //[HttpDelete("{id}")]
        //public async Task<ActionResult<MusicFile>> DeleteMusicFile(long id)
        //{
        //    var musicFile = await context.MusicFiles.FindAsync(id);
        //    if (musicFile == null)
        //    {
        //        return NotFound();
        //    }

        //    context.MusicFiles.Remove(musicFile);
        //    await context.SaveChangesAsync();

        //    return musicFile;
        //}

        //private bool MusicFileExists(long id)
        //{
        //    return context.MusicFiles.Any(e => e.ID == id);
        //}
    }
}
