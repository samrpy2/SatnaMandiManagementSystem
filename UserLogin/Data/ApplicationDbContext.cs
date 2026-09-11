using UserLogin.Models;
using Microsoft.EntityFrameworkCore;
namespace UserLogin.Data
{
    public class ApplicationDbContext: DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<UserLogin.Models.User> Users { get; set; } = null!;
        public DbSet<UserLogin.Models.MandiItem> MandiItems { get; set; } = null!;
        public DbSet<AuditLog> AuditLogs { get; set; }


    }
}
