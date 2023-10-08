using Kiselev.UserDbTestTask.Data.Users;

namespace Kiselev.UserDbTestTask.DataBase
{
    public class UserDb : DbContext
    {
        public UserDb(DbContextOptions<UserDb> options) : base(options) { }

        public DbSet<UserDTO> Users => Set<UserDTO>();
    }
}
