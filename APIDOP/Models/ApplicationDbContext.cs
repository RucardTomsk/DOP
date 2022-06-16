using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using APIDOP.Models.DB;

namespace APIDOP.Models
{
    public class ApplicationDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid, IdentityUserClaim<Guid>, IdentityUserRole<Guid>, IdentityUserLogin<Guid>, IdentityRoleClaim<Guid>, IdentityUserToken<Guid>>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public override DbSet<User> Users { get; set; }
        public DbSet<ForumSection> ForumSections { get; set; }
        public DbSet<Topic> Topics { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Attachment> Attachments {get;set;}
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<User>(o =>
            {
                o.ToTable("Users");
                o.HasMany(x => x.Topics).WithOne(x => x.User).OnDelete(DeleteBehavior.Cascade);
              
            });
            builder.Entity<ForumSection>(o =>
            {
                o.ToTable("ForumSections");
                o.HasMany(x => x.Topics).WithOne(x => x.ForumSection).OnDelete(DeleteBehavior.Cascade);
            });
            builder.Entity<Topic>(o =>
            {
                o.ToTable("Topics");
                o.HasMany(x => x.Messages).WithOne(x => x.Topic).OnDelete(DeleteBehavior.Cascade);
            });
            builder.Entity<Message>(o =>
            {
                o.ToTable("Messages");
                o.HasMany(x => x.Attachments).WithOne(x => x.Message).OnDelete(DeleteBehavior.Cascade);
            });
            builder.Entity<Attachment>(o =>
            {
                o.ToTable("Attachments");
            });
        }
    }
}
