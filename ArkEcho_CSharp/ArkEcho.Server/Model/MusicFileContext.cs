using ArkEcho.Core;
using Microsoft.EntityFrameworkCore;

namespace ArkEcho.Server
{
    public class MusicFileContext : DbContext
    {
        public MusicFileContext(DbContextOptions<MusicFileContext> options)
            : base(options)
        {
        }

        public DbSet<MusicFile> MusicFiles { get; set; }
    }
}