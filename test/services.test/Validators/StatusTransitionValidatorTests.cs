using FluentAssertions;
using services.suppliers.Models;
using services.suppliers.Validators;

namespace services.test.Validators;

public class StatusTransitionValidatorTests : BaseTests
{
    private readonly IStatusTransitionValidator _sut;

    public StatusTransitionValidatorTests()
    {
        _sut = new StatusTransitionValidator();
    }

    [Theory]
    [InlineData(BookingStatus.RECEIVED, BookingStatus.CONFIRMED, true)]
    [InlineData(BookingStatus.RECEIVED, BookingStatus.REJECTED, true)]
    [InlineData(BookingStatus.PENDING, BookingStatus.CONFIRMED, true)]
    [InlineData(BookingStatus.PENDING, BookingStatus.REJECTED, true)]
    [InlineData(BookingStatus.RECEIVED, BookingStatus.CANCELLED, true)]
    [InlineData(BookingStatus.PENDING, BookingStatus.CANCELLED, true)]
    [InlineData(BookingStatus.CONFIRMED, BookingStatus.CANCELLED, true)]
    [InlineData(BookingStatus.PENDING, BookingStatus.RECEIVED, false)]
    [InlineData(BookingStatus.CONFIRMED, BookingStatus.RECEIVED, false)]
    [InlineData(BookingStatus.CONFIRMED, BookingStatus.PENDING, false)]
    [InlineData(BookingStatus.REJECTED, BookingStatus.RECEIVED, false)]
    [InlineData(BookingStatus.REJECTED, BookingStatus.PENDING, false)]
    [InlineData(BookingStatus.CONFIRMED, BookingStatus.REJECTED, false)]
    [InlineData(BookingStatus.REJECTED, BookingStatus.CONFIRMED, false)]
    [InlineData(BookingStatus.REJECTED, BookingStatus.CANCELLED, false)]
    [InlineData(BookingStatus.CANCELLED, BookingStatus.RECEIVED, false)]
    [InlineData(BookingStatus.CANCELLED, BookingStatus.PENDING, false)]
    [InlineData(BookingStatus.CANCELLED, BookingStatus.CONFIRMED, false)]
    [InlineData(BookingStatus.CANCELLED, BookingStatus.REJECTED, false)]
    [InlineData(BookingStatus.RECEIVED, BookingStatus.RECEIVED, false)]
    [InlineData(BookingStatus.PENDING, BookingStatus.PENDING, false)]
    [InlineData(BookingStatus.CONFIRMED, BookingStatus.CONFIRMED, false)]
    [InlineData(BookingStatus.REJECTED, BookingStatus.REJECTED, false)]
    [InlineData(BookingStatus.CANCELLED, BookingStatus.CANCELLED, false)]
    [InlineData(BookingStatus.UNKNOWN, BookingStatus.RECEIVED, true)]
    [InlineData(BookingStatus.UNKNOWN, BookingStatus.PENDING, true)]
    [InlineData(BookingStatus.UNKNOWN, BookingStatus.CONFIRMED, true)]
    [InlineData(BookingStatus.UNKNOWN, BookingStatus.REJECTED, true)]
    [InlineData(BookingStatus.UNKNOWN, BookingStatus.CANCELLED, true)]
    [InlineData(BookingStatus.RECEIVED, BookingStatus.UNKNOWN, true)]
    [InlineData(BookingStatus.PENDING, BookingStatus.UNKNOWN, true)]
    [InlineData(BookingStatus.CONFIRMED, BookingStatus.UNKNOWN, true)]
    [InlineData(BookingStatus.REJECTED, BookingStatus.UNKNOWN, true)]
    [InlineData(BookingStatus.CANCELLED, BookingStatus.UNKNOWN, true)]
    public void CanTransitionTo_WhenTransitionValid_ReturnsAllowed(
        BookingStatus current,
        BookingStatus newStatus,
        bool allowed)
    {
        // arrange & act
        var actual = _sut.CanTransitionTo(current, newStatus);

        // assert
        actual.Should().Be(allowed);
    }
}