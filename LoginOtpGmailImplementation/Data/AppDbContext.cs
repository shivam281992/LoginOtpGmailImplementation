using LoginOtpGmailImplementation.Models;
using Microsoft.EntityFrameworkCore;

namespace LoginOtpGmailImplementation.Data
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<OtpEntry> OtpEntries { get; set; }
    }
}
