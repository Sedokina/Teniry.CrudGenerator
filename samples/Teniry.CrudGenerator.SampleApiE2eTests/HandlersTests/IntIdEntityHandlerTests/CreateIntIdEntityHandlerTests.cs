using Teniry.CrudGenerator.SampleApi;
using Teniry.CrudGenerator.SampleApi.Application.IntIdEntityFeature.CreateIntIdEntity;
using Moq;
using Teniry.CrudGenerator.SampleApi.CrudConfigurations.CustomIds.IntIdEntityGenerator;

namespace Teniry.CrudGenerator.SampleApiE2eTests.HandlersTests.IntIdEntityHandlerTests;

public class CreateIntIdEntityHandlerTests {
    private readonly CreateIntIdEntityCommand _command;
    private readonly Mock<SampleMongoDb> _db;
    private readonly CreateIntIdEntityHandler _sut;

    public CreateIntIdEntityHandlerTests() {
        _db = new();
        _sut = new(_db.Object);
        _command = new() {
            Name = "My test entity"
        };
    }

    [Fact]
    public async Task Should_ReturnCorrectValue() {
        // Arrange
        _db.Setup(x => x.AddAsync(It.IsAny<IntIdEntity>(), It.IsAny<CancellationToken>()))
            .Callback((IntIdEntity entity, CancellationToken _) => entity.Id = 1);

        // Act
        var createdEntityDto = await _sut.HandleAsync(_command, new());

        // Assert
        createdEntityDto.Id.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task Should_MapCommandToEntityCorrectly() {
        // Act
        await _sut.HandleAsync(_command, new());

        // Assert
        _db.Verify(
            x => x.AddAsync(
                It.Is<IntIdEntity>(c => c.Name.Equals("My test entity")),
                It.IsAny<CancellationToken>()
            )
        );
    }

    [Fact]
    public async Task Should_AddToDbSetAndSave() {
        // Act
        await _sut.HandleAsync(_command, new());

        // Assert
        _db.Verify(x => x.AddAsync(It.IsAny<IntIdEntity>(), It.IsAny<CancellationToken>()));
        _db.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));
        _db.VerifyNoOtherCalls();
    }
}