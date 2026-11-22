namespace services.suppliers.Models;

/// <summary>
/// The data that defines a guest.
/// </summary>
public sealed record GuestData
{
    public required string FirstName { get;  init; }
    public required string LastName { get;  init; }
}