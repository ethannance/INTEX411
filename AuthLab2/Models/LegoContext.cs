using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;


namespace AuthLab2.Models;

public partial class LegoContext : DbContext
{
    public LegoContext() //Constructor
    {
    }

    public LegoContext(DbContextOptions<LegoContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Product> Products { get; set; }
    public DbSet<content_recs> content_recs { get; set; }
    public DbSet<user_recommendations> user_recommendations { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Server=tcp:myfreesqldbservergroup411.database.windows.net,1433;Initial Catalog=Group411IntexDB;Persist Security Info=False;User ID=admin411;Password=Fireplot411;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");

    // Configures the model for the 'Product' entity, including indices and property mappings.
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasIndex(e => e.product_ID, "IX_Products_ProductID").IsUnique();
            entity.Property(e => e.product_ID).HasColumnName("product_ID");
        });

        OnModelCreatingPartial(modelBuilder);
    }
    // Partial method to allow additional model configuration in a separate file.
    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

    

    public class RegisterModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

}
