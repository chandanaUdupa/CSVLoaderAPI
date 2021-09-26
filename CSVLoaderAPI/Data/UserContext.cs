using CSVLoaderAPI.Entities;
using Microsoft.EntityFrameworkCore;


namespace CSVLoaderAPI.Data
{
    public class UserContext : DbContext
    {
        public UserContext(DbContextOptions<UserContext> options)
            : base(options)
        {
        }

        public System.Data.Entity.DbSet<User> Users { get; set; }
    }
}
