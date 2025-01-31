using Microsoft.EntityFrameworkCore;

namespace HobbyCom.Infrastructure.src.Databases
{
    public class SupabaseContext : DbContext
    {
        public SupabaseContext(DbContextOptions<SupabaseContext> options)
                    : base(options)
        {
        }
    }
}