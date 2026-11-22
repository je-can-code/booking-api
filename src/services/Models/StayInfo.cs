namespace services.suppliers.Models;

public sealed record StayInfo
{
    public required string PropertyId { get; init; }
    public required string RoomTypeId { get; init; }
    public required DateOnly CheckIn { get; init; }
    public required DateOnly CheckOut { get; init; }
    public required GuestData Guest { get; init; }
}