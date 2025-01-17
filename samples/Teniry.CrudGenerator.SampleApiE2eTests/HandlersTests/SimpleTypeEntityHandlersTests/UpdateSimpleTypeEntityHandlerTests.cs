using ITech.Cqrs.Domain.Exceptions;
using Teniry.CrudGenerator.SampleApi;
using Teniry.CrudGenerator.SampleApi.Application.SimpleTypeEntityFeature.UpdateSimpleTypeEntity;
using Teniry.CrudGenerator.SampleApi.Generators.SimpleTypeEntityGenerator;
using Moq;

namespace Teniry.CrudGenerator.SampleApiE2eTests.HandlersTests.SimpleTypeEntityHandlersTests;

public class UpdateSimpleTypeEntityHandlerTests {
    private readonly UpdateSimpleTypeEntityCommand _command;
    private readonly Mock<TestMongoDb> _db;
    private readonly UpdateSimpleTypeEntityHandler _sut;

    public UpdateSimpleTypeEntityHandlerTests() {
        _db = new();
        _sut = new(_db.Object);
        _command = new(Guid.NewGuid()) {
            Name = "New Test Entity",
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
        };
    }

    [Fact]
    public async Task Should_ThrowEntityNotFoundException_When_UpdatingNotExistingEntity() {
        // Arrange
        _db.Setup(x => x.FindAsync<SimpleTypeEntity>(new object[] { _command.Id }, It.IsAny<CancellationToken>()))
            .ReturnsAsync((SimpleTypeEntity?)null);

        // Act
        var act = async () => await _sut.HandleAsync(_command, new());

        // Assert
        await act.Should().ThrowAsync<EfEntityNotFoundException>()
            .Where(x => x.TypeName.Equals(nameof(SimpleTypeEntity)));
    }

    [Fact]
    public async Task Should_ChangeEntityDataAndSave() {
        // Arrange
        var entity = new SimpleTypeEntity {
            Id = _command.Id,
            Name = "Old Test Entity",
            Code = 'b',
            IsActive = false,
            RegistrationDate = DateTime.Today.AddDays(2),
            LastSignInDate = DateTimeOffset.UtcNow.AddDays(2),
            ByteRating = 3,
            ShortRating = -76,
            IntRating = -86154145,
            LongRating = -5871263515364,
            SByteRating = -7,
            UShortRating = 76,
            UIntRating = 86154145,
            ULongRating = 5871263515364,
            FloatRating = 99.91f,
            DoubleRating = 123432.16536,
            DecimalRating = 0871.11137816562m,
            NotIdGuid = new("63c4e04c-77d3-4e27-b490-8f6e4fc635bd")
        };
        _db.Setup(x => x.FindAsync<SimpleTypeEntity>(new object[] { _command.Id }, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        // Act
        await _sut.HandleAsync(_command, new());

        // Assert
        entity.Id.Should().Be(_command.Id);
        entity.Name.Should().Be("New Test Entity");
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
        _db.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));
        _db.Verify(
            x => x.FindAsync<SimpleTypeEntity>(new object[] { _command.Id }, It.IsAny<CancellationToken>()),
            Times.Once
        );
        _db.VerifyNoOtherCalls();
    }
}