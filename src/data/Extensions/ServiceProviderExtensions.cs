using Microsoft.Extensions.DependencyInjection;

namespace data.Extensions;

/// <summary>
/// Extensions for the <see cref="IServiceProvider"/> class.
/// </summary>
public static class ServiceProviderExtensions
{
    /// <summary>
    /// Set up the database for the first time and verify its ready2go.
    /// </summary>
    /// <param name="serviceProvider">The built service provider.</param>
    public static void InitializeDatabase(this IServiceProvider serviceProvider)
    {
        using var serviceScope = serviceProvider
            .GetRequiredService<IServiceScopeFactory>()
            .CreateScope();
        serviceScope
            .ServiceProvider
            .GetRequiredService<BookingDbContext>()
            .Database
            .EnsureCreated();
    }
}