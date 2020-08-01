using Microsoft.EntityFrameworkCore;

namespace ArkEcho
{
    public class MusicFileContext : DbContext
    {
        public MusicFileContext(DbContextOptions<MusicFileContext> options)
            : base(options)
        {
        }

        public DbSet<MusicFile> TodoItems { get; set; }
    }
}