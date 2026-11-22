using Microsoft.EntityFrameworkCore;

namespace data;

/// <summary>
/// The configuration for the database context for this service.
/// </summary>
internal sealed class BookingDbContext : DbContext
{
    public DbSet<BookingEntity> Bookings { get; set; }

    public BookingDbContext(DbContextOptions<BookingDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // the primary key is the BookingId.
        modelBuilder
            .Entity<BookingEntity>()
            .HasKey(entity => entity.BookingId);
        
        // we want a unique index on the DistributorId:IdempotencyKey tuple.
        modelBuilder
            .Entity<BookingEntity>()
            .HasIndex(entity => new { entity.DistributorId, entity.IdempotencyKey })
            .IsUnique();
        
        // flag the Status as the concurrency property in the off-chance we have race conditions.
        modelBuilder
            .Entity<BookingEntity>()
            .Property(entity => entity.Status)
            .IsConcurrencyToken();
    }
}