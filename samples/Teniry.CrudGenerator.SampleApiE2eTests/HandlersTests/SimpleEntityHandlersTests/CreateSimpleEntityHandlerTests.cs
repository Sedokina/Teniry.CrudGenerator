using Teniry.CrudGenerator.SampleApi;
using Teniry.CrudGenerator.SampleApi.Application.SimpleEntityFeature.CreateSimpleEntity;
using Moq;
using Teniry.CrudGenerator.SampleApi.CrudConfigurations.SimpleEntityGenerator;

namespace Teniry.CrudGenerator.SampleApiE2eTests.HandlersTests.SimpleEntityHandlersTests;

public class CreateSimpleEntityHandlerTests {
    private readonly CreateSimpleEntityCommand _command;
    private readonly Mock<SampleMongoDb> _db;
    private readonly CreateSimpleEntityHandler _sut;

    public CreateSimpleEntityHandlerTests() {
        _db = new();
        _sut = new(_db.Object);
        _command = new() {
            Name = "My test entity"
        };
    }

    [Fact]
    public async Task Should_ReturnCorrectValue() {
        // Arrange
        _db.Setup(x => x.AddAsync(It.IsAny<SimpleEntity>(), It.IsAny<CancellationToken>()))
            .Callback((SimpleEntity entity, CancellationToken _) => entity.Id = Guid.NewGuid());

        // Act
        var createdEntityDto = await _sut.HandleAsync(_command, new());

        // Assert
        createdEntityDto.Id.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Should_MapCommandToEntityCorrectly() {
        // Act
        await _sut.HandleAsync(_command, new());

        // Assert
        _db.Verify(
            x => x.AddAsync(
                It.Is<SimpleEntity>(c => c.Name.Equals("My test entity")),
                It.IsAny<CancellationToken>()
            )
        );
    }

    [Fact]
    public async Task Should_AddToDbSetAndSave() {
        // Act
        await _sut.HandleAsync(_command, new());

        // Assert
        _db.Verify(x => x.AddAsync(It.IsAny<SimpleEntity>(), It.IsAny<CancellationToken>()));
        _db.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));
        _db.VerifyNoOtherCalls();
    }
}