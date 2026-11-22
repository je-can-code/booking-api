using AutoFixture;
using data;
using data.Repositories;
using FakeItEasy;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using services.suppliers;
using services.suppliers.Infrastructure.LuxurioSupremeSupplier;
using services.suppliers.Mappers;
using services.suppliers.Models;
using services.suppliers.Validators;

namespace services.test;

public class BookingServiceTests : BaseTests
{
    private readonly IBookingService _sut;

    private readonly IBookingRepository _fakeRepository;
    private readonly IBookingEntityMapper _fakeMapper;
    private readonly ILuxurioSupremeApi _fakeApiClient;
    private readonly ILuxurioMapper _fakeLuxurioMapper;
    private readonly IStatusTransitionValidator _fakeValidator;

    public BookingServiceTests()
    {
        _fakeRepository = A.Fake<IBookingRepository>();
        _fakeMapper = A.Fake<IBookingEntityMapper>();
        _fakeApiClient = A.Fake<ILuxurioSupremeApi>();
        _fakeLuxurioMapper = A.Fake<ILuxurioMapper>();
        _fakeValidator = A.Fake<IStatusTransitionValidator>();
        _sut = new BookingService(
            A.Fake<ILogger<BookingService>>(),
            _fakeRepository,
            _fakeMapper,
            _fakeApiClient,
            _fakeLuxurioMapper,
            _fakeValidator);
    }

    [Theory]
    [InlineData(BookingStatus.CONFIRMED)]
    [InlineData(BookingStatus.REJECTED)]
    public async Task Create_WhenSupplierRespondsImmediately_ReturnsCreatedBooking(BookingStatus immediateStatus)
    {
        // arrange
        var fakeCreateRequest = _fixture.Create<CreateBookingRequest>();
        var fakeMappedRequest = _fixture.Create<BookingEntity>();
        A.CallTo(() => _fakeMapper.ToEntity(fakeCreateRequest))
            .Returns(fakeMappedRequest);

        var fakeCreatedResponse = _fixture.Create<BookingEntity>();
        A.CallTo(() => _fakeRepository.Create(fakeMappedRequest))
            .Returns(fakeCreatedResponse);

        var fakeLuxurioRequest = _fixture.Create<LuxurioSupremeRequest>();
        A.CallTo(() => _fakeLuxurioMapper.ToRequest(fakeCreatedResponse.BookingId, fakeCreateRequest.StayInfo))
            .Returns(fakeLuxurioRequest);

        var fakeLuxurioResponse = _fixture.Build<LuxurioSupremeResponse>()
            .With(response => response.Status, immediateStatus)
            .Create();
        A.CallTo(() => _fakeApiClient.PostAsync(fakeLuxurioRequest))
            .Returns(fakeLuxurioResponse);

        var fakeStatusResponse = _fixture.Create<BookingEntity>();
        A.CallTo(() => _fakeRepository.UpdateStatus(
                fakeMappedRequest.BookingId,
                nameof(BookingStatus.RECEIVED).ToUpperInvariant(),
                fakeLuxurioResponse.Status.ToString().ToUpperInvariant(),
                fakeLuxurioResponse.SupplierReferenceId))
            .Returns(fakeStatusResponse);

        var expected = _fixture.Create<RawBookingResponse>();
        A.CallTo(() => _fakeMapper.ToRawResponse(fakeStatusResponse))
            .Returns(expected);

        // act
        var response = await _sut.Create(fakeCreateRequest);

        // assert
        response.Should().Be(expected);
        A.CallTo(() => _fakeRepository.GetByIndex(fakeCreateRequest.DistributorId, fakeCreateRequest.IdempotencyKey))
            .MustNotHaveHappened();
        A.CallTo(() => _fakeRepository.UpdateStatus(
                fakeMappedRequest.BookingId,
                nameof(BookingStatus.RECEIVED).ToUpperInvariant(),
                nameof(BookingStatus.PENDING).ToUpperInvariant(),
                string.Empty))
            .MustNotHaveHappened();
    }

    [Fact]
    public async Task Create_WhenDuplicateBookingDetected_ReturnsOriginalBooking()
    {
        // arrange
        var fakeCreateRequest = _fixture.Create<CreateBookingRequest>();
        var fakeMappedRequest = _fixture.Create<BookingEntity>();
        A.CallTo(() => _fakeMapper.ToEntity(fakeCreateRequest))
            .Returns(fakeMappedRequest);

        // this particular exception is what triggers the lookup by index!
        var fakeCreateException = new DbUpdateException("borked", new Exception("SQLite Error 19"));
        A.CallTo(() => _fakeRepository.Create(fakeMappedRequest))
            .Throws(fakeCreateException);

        var fakeBooking = _fixture.Create<BookingEntity>();
        A.CallTo(() => _fakeRepository.GetByIndex(fakeCreateRequest.DistributorId, fakeCreateRequest.IdempotencyKey))
            .Returns(fakeBooking);

        var expected = _fixture.Create<RawBookingResponse>();
        A.CallTo(() => _fakeMapper.ToRawResponse(fakeBooking))
            .Returns(expected);

        // act
        var response = await _sut.Create(fakeCreateRequest);

        // assert
        A.CallTo(() => _fakeLuxurioMapper.ToRequest(A<Guid>._, A<StayInfo>._))
            .MustNotHaveHappened();
        response.Should().Be(expected);
    }
}