using Microsoft.EntityFrameworkCore;

using VERSUS.Infrastructure.Models;

namespace VERSUS.Infrastructure.Services
{
    public class SiteDbContext : DbContext
    {
        public DbSet<Review> Reviews { get; set; }

        public SiteDbContext(DbContextOptions<SiteDbContext> options)
            : base(options)
        { }
    }
}