using Teniry.Cqrs.Extended.Exceptions;
using Teniry.CrudGenerator.SampleApi;
using Moq;
using Teniry.Cqrs.Extended.Types.PatchOperationType;
using Teniry.CrudGenerator.SampleApi.Application.SimpleTypeEntityFeature.PatchSimpleTypeEntity;
using Teniry.CrudGenerator.SampleApi.CrudConfigurations.SimpleTypeEntityGenerator;

namespace Teniry.CrudGenerator.SampleApiE2eTests.HandlersTests.SimpleTypeEntityHandlersTests;

public class PatchSimpleTypeEntityHandlerTests {
    private readonly PatchSimpleTypeEntityCommand _command;
    private readonly Mock<SampleMongoDb> _db;
    private readonly PatchSimpleTypeEntityHandler _sut;

    public PatchSimpleTypeEntityHandlerTests() {
        _db = new();
        _sut = new(_db.Object);
        _command = new(Guid.NewGuid()) {
            Name = new("New Test Entity", PatchOpType.Update),
            Code = new('a', PatchOpType.Update),
            IsActive = new(true, PatchOpType.Update),
            RegistrationDate = new(DateTime.Today, PatchOpType.Update),
            LastSignInDate = new(DateTimeOffset.UtcNow, PatchOpType.Update),
            ByteRating = new(1, PatchOpType.Update),
            ShortRating = new(-83, PatchOpType.Update),
            IntRating = new(-19876718, PatchOpType.Update),
            LongRating = new(-971652637891, PatchOpType.Update),
            SByteRating = new(-4, PatchOpType.Update),
            UShortRating = new(83, PatchOpType.Update),
            UIntRating = new(19876718, PatchOpType.Update),
            ULongRating = new(971652637891, PatchOpType.Update),
            FloatRating = new(18.13f, PatchOpType.Update),
            DoubleRating = new(91873.862378, PatchOpType.Update),
            DecimalRating = new(867.97716829m, PatchOpType.Update),
            NotIdGuid = new(new("63c4e04c-77d3-4e27-b490-8f6e4fc635bd"), PatchOpType.Update)
        };
    }

    [Fact]
    public async Task Should_ThrowEntityNotFoundException_When_UpdatingNotExistingEntity() {
        // Arrange
        _db.Setup(x => x.FindAsync<SimpleTypeEntity>(new object[] { _command.Id }, It.IsAny<CancellationToken>()))
            .ReturnsAsync((SimpleTypeEntity?)null);

        // Act
        var act = async () => await _sut.HandleAsync(_command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<EntityNotFoundException>()
            .Where(x => x.NotFoundType == typeof(SimpleTypeEntity));
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
        await _sut.HandleAsync(_command, CancellationToken.None);

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