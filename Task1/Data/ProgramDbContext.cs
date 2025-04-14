using Microsoft.EntityFrameworkCore;
using Task1.Models;

namespace Task1.Data
{
    public class ProgramDbContext : DbContext
    {
        public ProgramDbContext() : base() { }
        public ProgramDbContext(DbContextOptions<ProgramDbContext> options) : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //Cluster Index
            modelBuilder.Entity<Vehicles>()
                .HasKey(v => v.Id)
                .IsClustered();

            // one to many relationship with vehicles and Brands 
            modelBuilder.Entity<Vehicles>()
                .HasOne(v => v.Brands)
                .WithMany(b => b.Vehicle)
                .HasForeignKey(v => v.BrandID);

            // one to many relationship with vehicles and CAtegories  
            modelBuilder.Entity<Vehicles>()
                .HasOne(v => v.Categories)
                .WithMany(c => c.Vehicles)
                .HasForeignKey(v => v.CategoryId);

            // Non clustered Index For the Vehicles 
            modelBuilder.Entity<Vehicles>()
                .HasIndex(v => v.Name)
                .IsUnique();

            // Required field name of Vehicles 
            modelBuilder.Entity<Vehicles>()
                .Property(v => v.Name)
                .IsRequired();

            //Check Constraints for the Ratings 
            modelBuilder.Entity<Vehicles>()
                .ToTable(t => t.HasCheckConstraint("CK_Vehicles_Rating", "[Rating] >=0 AND [Rating] <=5"));


            //Check Constraints for the Price 
            modelBuilder.Entity<Vehicles>()
                .ToTable(t => t.HasCheckConstraint("CK_Vehicles_Price", "[Price] >= 200000 AND [Price] <= 200000000"));

            modelBuilder.Entity<Vehicles>()
                .Property(v => v.Price)
                .HasPrecision(18, 2);



            modelBuilder.Entity<Brands>()
                .HasKey(b => b.Id)
                .IsClustered();

            modelBuilder.Entity<Brands>()
                .HasIndex(b => b.Name)
                .IsUnique();

            modelBuilder.Entity<Categories>()
                .HasKey(b => b.Id)
                .IsClustered();

            modelBuilder.Entity<Categories>()
                .HasIndex(b => b.Name)
                .IsUnique();

            modelBuilder.Entity<Stocks>()
                .HasKey(stk => stk.Id);

            modelBuilder.Entity<Stocks>()
                .HasOne(stk => stk.Vehicle)
                .WithOne(vhc => vhc.Stocks)
                .HasForeignKey<Stocks>(stk => stk.VehicleId);

        }

        DbSet<Vehicles> Vehicles { get; set; }
        DbSet<Brands> Brands { get; set; }
        DbSet<Categories> Categories { get; set; }
        DbSet<Stocks> Stocks { get; set; }

    }
}
