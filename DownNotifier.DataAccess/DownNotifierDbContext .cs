using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace DownNotifier.DataAccess
{
    public class DownNotifierDbContext : IdentityDbContext<AppUser, AppRole, string>
    {
        private IConfiguration _configuration;
        public DownNotifierDbContext()
        {
        }
        public DownNotifierDbContext(DbContextOptions<DownNotifierDbContext> options, IConfiguration configuration)
            : base(options)
        {
            _configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }
        public DbSet<TargetApplication> TargetApplications { get; set; }
        public DbSet<LogEntry> Logs { get; set; }
    }
}
