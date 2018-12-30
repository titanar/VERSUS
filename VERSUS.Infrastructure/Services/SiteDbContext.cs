using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using VERSUS.Infrastructure.Models;

namespace VERSUS.Infrastructure.Services
{
    public class SiteDbContext : IdentityDbContext<SiteUser, SiteRole, string>
    {
        public DbSet<Review> Reviews { get; set; }

        public SiteDbContext(DbContextOptions<SiteDbContext> options)
            : base(options)
        { }
    }
}