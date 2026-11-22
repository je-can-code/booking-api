using AutoFixture;

namespace api.test;

public abstract class BaseTests
{
    internal static readonly Fixture _fixture = new();

    static BaseTests()
    {
        _fixture.Register<DateTime, DateOnly>(DateOnly.FromDateTime);
    }
}