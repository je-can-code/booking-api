using api.Mappers;
using FluentAssertions;
using services.suppliers.Models;

namespace api.test.Mappers;

public class SupplierMapperTests
{
    private readonly ISupplierMapper _sut;

    public SupplierMapperTests()
    {
        _sut = new SupplierMapper();
    }

    [Fact]
    public void ToRequest_WhenGivenData_MapsObject()
    {
        // arrange
        var fakeBookingId = Guid.NewGuid();
        var fakeSupplierReferenceId = "luxurio-supreme-approves-this-message-123";
        var fakeBookingStatus = BookingStatus.CONFIRMED;

        var expected = new UpdateBookingRequest
        {
            BookingId =  fakeBookingId,
            SupplierReferenceId = fakeSupplierReferenceId,
            NewStatus =  fakeBookingStatus
        };

        // act
        var actual = _sut.ToRequest(fakeBookingId, fakeSupplierReferenceId, fakeBookingStatus);

        // assert
        actual.Should().Be(expected);
    }
}