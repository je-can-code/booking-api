namespace services.suppliers.Models;

/// <summary>
/// The shape of the raw data that a booking has.
/// </summary>
public sealed record RawBookingResponse
{
    public Guid BookingId { get; set; }
    public required string SupplierReferenceId { get; init; }
    public required string DistributorId { get; init; }
    public required string IdempotencyKey { get; init; }
    public required BookingStatus Status { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
    public required DateTimeOffset UpdatedAt { get; init; }
    
}