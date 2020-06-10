using Microsoft.EntityFrameworkCore;
using UserApplication.Data.Models;

namespace UserApplication.Data
{
    public class UserContext : DbContext
    {
        public DbSet<UserModel> Users { get; set; }
        public DbSet<AddressModel> Addresses { get; set; }

        public UserContext(DbContextOptions<UserContext> options) : base(options)
        {
            
        }

        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserModel>()
                .HasKey(u => u.Id);

            modelBuilder.Entity<UserModel>()
                .Property(u => u.FirstName)
                .IsRequired();

            modelBuilder.Entity<UserModel>()
                .Property(u => u.LastName)
                .IsRequired();

            modelBuilder.Entity<UserModel>()
                .Property(u => u.PhoneNumber)
                .IsRequired();

            modelBuilder.Entity<UserModel>()
                .Property(u => u.Description)
                .IsRequired(false);

            modelBuilder.Entity<UserModel>()
                .HasMany(u => u.Addresses)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AddressModel>()
                .HasKey(a => a.Id);

            modelBuilder.Entity<AddressModel>()
                .Property(a => a.Line1)
                .IsRequired();

            modelBuilder.Entity<AddressModel>()
                .Property(a => a.Line2)
                .IsRequired(false);

            modelBuilder.Entity<AddressModel>()
                .Property(a => a.City)
                .IsRequired();

            modelBuilder.Entity<AddressModel>()
                .Property(a => a.Province)
                .IsRequired();

            modelBuilder.Entity<AddressModel>()
                .Property(a => a.Country)
                .IsRequired();

            modelBuilder.Entity<AddressModel>()
                .Property(a => a.ZipCode)
                .IsRequired();

            modelBuilder.Entity<AddressModel>()
                .Property(a => a.Description)
                .IsRequired(false);
        }
    }
}
