using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApp.AccountManager;
using WebApp.DB;
using WebApp.DB.Model;
using WebApp.DB.Repo;

namespace WebApp
{
    public class ApplicationDbContext: IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base (options)
        {
            Database.Migrate();
        }

        DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfiguration(new FriendConfiguration());
            builder.ApplyConfiguration(new MessageConfuiguration());
            
        }
    }
}
