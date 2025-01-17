using ITech.CrudGenerator.TestApi;
using ITech.CrudGenerator.TestApi.Application.SimpleTypeEntityFeature.CreateSimpleTypeEntity;
using ITech.CrudGenerator.TestApi.Generators.SimpleTypeEntityGenerator;
using ITech.CrudGenerator.TestApiTests.E2eTests.Core;
using Moq;

namespace ITech.CrudGenerator.TestApiTests.HandlersTests.SimpleTypeEntityHandlersTests;

public class CreateSimpleTypeEntityHandlerTests {
    private readonly CreateSimpleTypeEntityCommand _command;
    private readonly Mock<TestMongoDb> _db;
    private readonly CreateSimpleTypeEntityHandler _sut;

    public CreateSimpleTypeEntityHandlerTests() {
        _db = new();
        _sut = new(_db.Object);
        _command = new() {
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
        };
    }

    [Fact]
    public async Task Should_ReturnCorrectValue() {
        // Arrange
        _db.Setup(x => x.AddAsync(It.IsAny<SimpleTypeEntity>(), It.IsAny<CancellationToken>()))
            .Callback((SimpleTypeEntity entity, CancellationToken _) => entity.Id = Guid.NewGuid());

        // Act
        var createdEntityDto = await _sut.HandleAsync(_command, new());

        // Assert
        createdEntityDto.Id.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Should_MapCommandToEntityCorrectly() {
        // Act
        await _sut.HandleAsync(_command, new());

        var entityMapAssert = Should.Assert<SimpleTypeEntity>(
            x => {
                x.Id.Should().BeEmpty();
                x.Name.Should().Be("Test Entity");
                x.Code.Should().Be('a');
                x.IsActive.Should().Be(true);
                x.RegistrationDate.Should().NotBeBefore(DateTime.Today.Date.ToUniversalTime());
                x.LastSignInDate.Should().NotBeBefore(DateTime.Today.Date.ToUniversalTime());
                x.ByteRating.Should().BeGreaterThan(0);
                x.ShortRating.Should().BeLessThan(0);
                x.IntRating.Should().BeLessThan(0);
                x.LongRating.Should().BeLessThan(0);
                x.SByteRating.Should().BeLessThan(0);
                x.UShortRating.Should().BeGreaterThan(0);
                x.UIntRating.Should().BeGreaterThan(0);
                x.ULongRating.Should().BeGreaterThan(0);
                x.FloatRating.Should().BeGreaterThan(0);
                x.DoubleRating.Should().BeGreaterThan(0);
                x.DecimalRating.Should().BeGreaterThan(0);
                x.NotIdGuid.Should().NotBeEmpty();
            }
        );

        // Assert
        _db.Verify(x => x.AddAsync(Should.Assert(entityMapAssert), It.IsAny<CancellationToken>()));
    }

    [Fact]
    public async Task Should_AddToDbSetAndSave() {
        // Act
        await _sut.HandleAsync(_command, new());

        // Assert
        _db.Verify(x => x.AddAsync(It.IsAny<SimpleTypeEntity>(), It.IsAny<CancellationToken>()));
        _db.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));
        _db.VerifyNoOtherCalls();
    }
}