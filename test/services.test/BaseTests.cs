using AutoFixture;

namespace services.test;

public abstract class BaseTests
{
    internal static readonly Fixture _fixture = new Fixture();

    static BaseTests()
    {
        _fixture.Register<DateTime, DateOnly>(DateOnly.FromDateTime);
    }
}