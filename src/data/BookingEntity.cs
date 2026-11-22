namespace data;

/// <summary>
/// A database entity representing a single booking between a customer and a supplier.
/// </summary>
public sealed record BookingEntity
{
    public required Guid BookingId { get; init; }
    public required string SupplierReferenceId { get; set; } = string.Empty;
    public required string DistributorId { get; init; }
    public required string IdempotencyKey { get; init; }
    public required string Status { get; set; }
    public required DateTimeOffset CreatedAt { get; init; }
    public required DateTimeOffset UpdatedAt { get; set; }
}