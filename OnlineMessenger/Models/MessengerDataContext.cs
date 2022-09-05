using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace OnlineMessenger.Models
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public partial class MessengerDataContext : IdentityDbContext<User>
    {

        public MessengerDataContext()
        {

        }

        public MessengerDataContext(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public MessengerDataContext(DbContextOptions<MessengerDataContext> options, IConfiguration configuration)
            : base(options)
        {
            this.configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(configuration["DefaultConnection"]);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Group> Groups { get; set; }

        public DbSet<GroupMember> GroupMembers { get; set; }
        
        public DbSet<Chat> Chats { get; set; }

        public DbSet<Message> Messages { get; set; }

        public IConfiguration configuration;
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}
