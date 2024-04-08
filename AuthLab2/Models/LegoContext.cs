using System;
using System.Collections.Generic;
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

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlite("Data Source=LEGO.sqlite");

    // Configures the model for the 'Book' entity, including indices and property mappings.
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
}
