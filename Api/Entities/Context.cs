using Microsoft.EntityFrameworkCore;

namespace Api.Entities
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Route>().HasMany(r => r.Checkpoints)
                .WithOne(cp => cp.Route)
                .HasForeignKey(cp => cp.RouteId);
            modelBuilder.Entity<Route>().OwnsOne(r => r.Properties);
            modelBuilder.Entity<Route>().HasMany(r => r.Ranking).WithOne(rr => rr.Route)
                .HasForeignKey(rr => rr.RouteId);

            modelBuilder.Entity<RankingRecord>().HasOne(rr => rr.User);
            modelBuilder.Entity<RankingRecord>().HasKey(rr => new { rr.RouteId, rr.UserId });

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Route> Routes { get; set; }
    }
}