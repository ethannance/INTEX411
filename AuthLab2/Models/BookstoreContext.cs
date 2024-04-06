using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace AuthLab2.Models;

public partial class BookstoreContext : DbContext
{
    public BookstoreContext() //Constructor
    {
    }

    public BookstoreContext(DbContextOptions<BookstoreContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Book> Books { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlite("Data Source=Bookstore.sqlite");

    // Configures the model for the 'Book' entity, including indices and property mappings.
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasIndex(e => e.BookId, "IX_Books_BookID").IsUnique();
            entity.Property(e => e.BookId).HasColumnName("BookID");
            entity.Property(e => e.Isbn).HasColumnName("ISBN");
        });

        OnModelCreatingPartial(modelBuilder);
    }
    // Partial method to allow additional model configuration in a separate file.
    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
