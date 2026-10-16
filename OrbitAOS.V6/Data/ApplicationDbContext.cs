using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace OrbitAOS.V6.Data
{
    // ApplicationDbContext upgraded for EF Core 8.0 compatibility.
    // Migration note: IdentityDbContext<IdentityUser> is compatible with EF Core 8.0.
    // The generic parameter ensures proper Identity table generation.
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    }
}
