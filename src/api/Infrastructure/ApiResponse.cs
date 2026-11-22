namespace api.Infrastructure;

/// <summary>
/// A generic shape that all responses from this API carry.
/// </summary>
/// <typeparam name="T">The underlying data of the response.</typeparam>
public sealed record ApiResponse<T>
{
    public static ApiResponse<T> From(T response)
    {
        return new ApiResponse<T>
        {
            Data = response,
        };
    }

    public required T Data { get; init; }
    // other observation stuff here.
}