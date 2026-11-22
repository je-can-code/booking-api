using AutoFixture;
using data.Repositories;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace data.tests.Repositories;

public class BookingRepositoryTests : BaseTests, IDisposable
{
    private readonly IBookingRepository  _sut;
    
    private readonly SqliteConnection _connection;

    public BookingRepositoryTests()
    {
        _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();
        var dbContextOptions = new DbContextOptionsBuilder<BookingDbContext>()
            .UseSqlite(_connection).Options;
        var dbContext = new BookingDbContext(dbContextOptions);
        dbContext.Database.EnsureCreated();
        _sut = new BookingRepository(dbContext);
    }

    [Fact]
    public async Task Create_WhenDuplicateBookingDetected_ThrowsUniqueIndexException()
    {
        // arrange
        var fakeFirstBooking = _fixture.Create<BookingEntity>();
        var fakeCreatedBooking = await _sut.Create(fakeFirstBooking);
        var fakeSecondBooking = _fixture.Build<BookingEntity>()
            // same as the first means we clash against the unique index!
            .With(entity => entity.DistributorId, fakeCreatedBooking.DistributorId)
            .With(entity => entity.IdempotencyKey, fakeCreatedBooking.IdempotencyKey)
            .Create();

        var actual = async () => await _sut.Create(fakeSecondBooking);
        
        // act & assert
        await actual.Should()
            .ThrowAsync<DbUpdateException>()
            .WithInnerException<DbUpdateException, SqliteException>()
            .WithMessage("*SQLite Error 19*");
    }

    [Fact]
    public async Task Update_WhenStatusAlreadyConfirmed_CannotUpdateIncorrectly()
    {
        // arrange
        var fakeBooking = _fixture.Build<BookingEntity>()
            .With(entity => entity.Status, "CONFIRMED")
            .Create();
        var fakeCreatedBooking = await _sut.Create(fakeBooking);
        
        // act
        var actual = await _sut.UpdateStatus(
            fakeCreatedBooking.BookingId,
            "PENDING",
            "REJECTED", // flipping PENDING -> REJECTED.
            fakeCreatedBooking.SupplierReferenceId);

        // assert
        actual.Should().BeNull();
    }

    public void Dispose()
    {
        _connection.Close();
    }
}