using GestionComplejo2C.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GestionComplejo2C.Infrastructure.Persistence
{
    public class SportsComplexDbContext : DbContext
    {
        public SportsComplexDbContext(DbContextOptions<SportsComplexDbContext> options)
            : base(options)
        {
        }

        public DbSet<Court> Courts => Set<Court>();

        public DbSet<Booking> Bookings => Set<Booking>();

        public DbSet<LockerRoom> LockerRooms => Set<LockerRoom>();

        public DbSet<Amenity> Amenities => Set<Amenity>();

        public DbSet<User> Users => Set<User>();

        public DbSet<Administrator> Administrators => Set<Administrator>();

        public DbSet<Customer> Customers => Set<Customer>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().UseTpcMappingStrategy();
        }
    }
}
