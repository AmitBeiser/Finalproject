using Microsoft.EntityFrameworkCore;

namespace ParkinServer.Models;

public class ParkinDbContext : DbContext
{
    public ParkinDbContext(DbContextOptions<ParkinDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Vehicle> Vehicles { get; set; }
    public DbSet<UserVehicle> UserVehicles { get; set; }
    public DbSet<Post> Posts { get; set; }
}