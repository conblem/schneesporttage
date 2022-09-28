using api.Entities;
using Microsoft.EntityFrameworkCore;

namespace api;

public class SchneesporttageContext : DbContext
{
    public SchneesporttageContext(DbContextOptions<SchneesporttageContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; } = null!;
}