using Microsoft.AspNetCore.Identity;

namespace VERSUS.Infrastructure.Models
{
    public class SiteUser : IdentityUser
    {
        public int UserNewColumn { get; set; }
    }
}