using ITech.Cqrs.Domain.Exceptions;
using ITech.CrudGenerator.TestApi;
using ITech.CrudGenerator.TestApi.Application.SimpleTypeEntityFeature.GetSimpleTypeEntity;
using ITech.CrudGenerator.TestApi.Generators.SimpleTypeEntityGenerator;
using Moq;

namespace ITech.CrudGenerator.TestApiTests.HandlersTests.SimpleTypeEntityHandlersTests;

public class GetSimpleTypeEntityHandlerTests {
    private readonly Mock<TestMongoDb> _db;
    private readonly GetSimpleTypeEntityQuery _query;
    private readonly GetSimpleTypeEntityHandler _sut;

    public GetSimpleTypeEntityHandlerTests() {
        _db = new();
        _sut = new(_db.Object);
        _query = new(Guid.NewGuid());
    }

    [Fact]
    public async Task Should_ThrowEntityNotFoundException_When_GettingNotExistingEntity() {
        // Arrange
        _db.Setup(x => x.FindAsync<SimpleTypeEntity>(new object[] { _query.Id }, It.IsAny<CancellationToken>()))
            .ReturnsAsync((SimpleTypeEntity?)null);

        // Act
        var act = async () => await _sut.HandleAsync(_query, new());

        // Assert
        await act.Should().ThrowAsync<EfEntityNotFoundException>()
            .Where(x => x.TypeName.Equals(nameof(SimpleTypeEntity)));
    }

    [Fact]
    public async Task Should_GetEntityWithCorrectData() {
        // Arrange
        _db.Setup(x => x.FindAsync<SimpleTypeEntity>(new object[] { _query.Id }, It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                new SimpleTypeEntity {
                    Id = _query.Id,
                    Name = "Test Entity",
                    Code = 'a',
                    IsActive = true,
                    RegistrationDate = DateTime.Today,
                    LastSignInDate = DateTimeOffset.UtcNow,
                    ByteRating = 1,
                    ShortRating = -83,
                    IntRating = -19876718,
                    LongRating = -971652637891,
                    SByteRating = -4,
                    UShortRating = 83,
                    UIntRating = 19876718,
                    ULongRating = 971652637891,
                    FloatRating = 18.13f,
                    DoubleRating = 91873.862378,
                    DecimalRating = 867.97716829m,
                    NotIdGuid = new("63c4e04c-77d3-4e27-b490-8f6e4fc635bd")
                }
            );

        // Act
        var entity = await _sut.HandleAsync(_query, new());

        // Assert
        entity.Id.Should().Be(_query.Id);
        entity.Name.Should().Be("Test Entity");
        entity.Code.Should().Be('a');
        entity.IsActive.Should().Be(true);
        entity.RegistrationDate.Should().NotBeBefore(DateTime.Today.Date.ToUniversalTime());
        entity.LastSignInDate.Should().NotBeBefore(DateTime.Today.Date.ToUniversalTime());
        entity.ByteRating.Should().BeGreaterThan(0);
        entity.ShortRating.Should().BeLessThan(0);
        entity.IntRating.Should().BeLessThan(0);
        entity.LongRating.Should().BeLessThan(0);
        entity.SByteRating.Should().BeLessThan(0);
        entity.UShortRating.Should().BeGreaterThan(0);
        entity.UIntRating.Should().BeGreaterThan(0);
        entity.ULongRating.Should().BeGreaterThan(0);
        entity.FloatRating.Should().BeGreaterThan(0);
        entity.DoubleRating.Should().BeGreaterThan(0);
        entity.DecimalRating.Should().BeGreaterThan(0);
        entity.NotIdGuid.Should().NotBeEmpty();
        _db.Verify(
            x => x.FindAsync<SimpleTypeEntity>(new object[] { _query.Id }, It.IsAny<CancellationToken>()),
            Times.Once
        );
        _db.VerifyNoOtherCalls();
    }
}