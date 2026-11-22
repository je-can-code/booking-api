namespace services.suppliers.Infrastructure.LuxurioSupremeSupplier;

/// <summary>
/// The contract shape for a service used to communicate with the Luxurio Supreme API.
/// </summary>
public interface ILuxurioSupremeApi
{
    /// <summary>
    /// Sends a request to the Luxurio Supreme supplier API to see if they can accommodate the request.
    /// </summary>
    /// <param name="request">The booking information to check for.</param>
    /// <returns>The response from the Luxurio Supreme supplier API.</returns>
    Task<LuxurioSupremeResponse> PostAsync(LuxurioSupremeRequest request);
}