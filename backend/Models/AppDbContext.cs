using Microsoft.EntityFrameworkCore;
using backend.Entities;

namespace backend.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<WebSite> WebSites { get; set; }
        public DbSet<WebSiteData> WebSiteData { get; set; }
        public DbSet<TestScenario> TestScenarios { get; set; }
        public DbSet<ScenarioResult> ScenarioResults { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                        .HasMany(u => u.Sites)
                        .WithOne(s => s.User)
                        .HasForeignKey(s => s.UserId);

            modelBuilder.Entity<WebSite>()
                        .HasMany(w => w.WebSiteData)
                        .WithOne(d => d.WebSite)
                        .HasForeignKey(d => d.WebSiteId);

            modelBuilder.Entity<WebSite>()
                        .HasMany(w => w.TestScenarios)
                        .WithOne(t => t.WebSite)
                        .HasForeignKey(t => t.WebSiteId);

            modelBuilder.Entity<WebSite>()
                        .HasMany(w => w.TestsData)
                        .WithOne(r => r.WebSite)
                        .HasForeignKey(r => r.WebSiteId);
        }
    }
}
