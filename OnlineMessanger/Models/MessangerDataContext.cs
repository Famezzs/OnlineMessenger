using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace OnlineMessanger.Models
{
    public partial class MessangerDataContext : IdentityDbContext<User>
    {
        public MessangerDataContext(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public MessangerDataContext(IConfiguration configuration, DbContextOptions<MessangerDataContext> options)
            : base(options)
        {
            this.configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Group>? Groups { get; set; }

        public DbSet<GroupMember>? GroupMembers { get; set; }
        
        public DbSet<Chat>? Chats { get; set; }

        public DbSet<Message>? Messages { get; set; }

        private IConfiguration configuration;
    }
}
