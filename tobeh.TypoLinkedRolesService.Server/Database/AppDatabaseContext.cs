using Microsoft.EntityFrameworkCore;
using tobeh.TypoLinkedRolesService.Server.Database.Model;

namespace tobeh.TypoLinkedRolesService.Server.Database
{
    public class AppDatabaseContext : DbContext
    {
        private static readonly string PATH = "./data";
        private static string DbPath => Path.Combine(PATH, "app.db");

        public static void EnsureDatabaseExists()
        {
            Directory.CreateDirectory(PATH);
            var ctx = new AppDatabaseContext();
            ctx.Database.EnsureCreated();
            ctx.Dispose();
        }

        public DbSet<DiscordUserToken> DiscordUserTokens { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Use SQLite as the database provider
            optionsBuilder.UseSqlite($"Data Source={DbPath}");
        }
    }
}
