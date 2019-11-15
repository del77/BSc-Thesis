using Microsoft.EntityFrameworkCore;

namespace Api.Entities
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Route> Routes { get; set; }
    }
}