using Microsoft.EntityFrameworkCore;

namespace Api.Entities
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Route>().HasMany(r => r.Checkpoints).WithOne(cp => cp.Route)
                .HasForeignKey(cp => cp.RouteId);
            modelBuilder.Entity<Route>().HasOne(r => r.Properties).WithOne(p => p.Route);
            modelBuilder.Entity<Route>().HasMany(r => r.Ranking).WithOne(rr => rr.Route)
                .HasForeignKey(rr => rr.RouteId);

            modelBuilder.Entity<RankingRecord>().HasOne(rr => rr.User);

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Route> Routes { get; set; }
    }
}