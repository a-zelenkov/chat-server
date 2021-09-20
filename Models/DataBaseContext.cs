using Microsoft.EntityFrameworkCore;

namespace chat.sssu.Models
{
    public class DataBaseContext : DbContext
    {
        public DbSet<User> Users { get; set; }
   
        public DbSet<Chat> Chats { get; set; }

        public DbSet<Message> Messages { get; set; }

        public DbSet<File> Files { get; set; }

        public DataBaseContext(DbContextOptions<DataBaseContext> options)
            : base(options)
        {
/*            Database.EnsureDeleted();
            Database.EnsureCreated();*/
        }
    }
}
