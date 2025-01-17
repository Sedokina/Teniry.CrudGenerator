using Teniry.CrudGenerator.SampleApi;
using Teniry.CrudGenerator.SampleApi.Application.EntityIdNameFeature.CreateEntityIdName;
using Teniry.CrudGenerator.SampleApi.Generators.CustomIds.EntityIdNameGenerator;
using Moq;

namespace Teniry.CrudGenerator.SampleApiE2eTests.HandlersTests.EntityIdNameGeneratorHandlerTests;

public class CreateEntityIdNameHandlerTests {
    private readonly CreateEntityIdNameCommand _command;
    private readonly Mock<TestMongoDb> _db;
    private readonly CreateEntityIdNameHandler _sut;

    public CreateEntityIdNameHandlerTests() {
        _db = new();
        _sut = new(_db.Object);
        _command = new() {
            Name = "My test entity"
        };
    }

    [Fact]
    public async Task Should_ReturnCorrectValue() {
        // Arrange
        _db.Setup(x => x.AddAsync(It.IsAny<EntityIdName>(), It.IsAny<CancellationToken>()))
            .Callback((EntityIdName entity, CancellationToken _) => entity.EntityIdNameId = Guid.NewGuid());

        // Act
        var createdEntityDto = await _sut.HandleAsync(_command, new());

        // Assert
        createdEntityDto.EntityIdNameId.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Should_MapCommandToEntityCorrectly() {
        // Act
        await _sut.HandleAsync(_command, new());

        // Assert
        _db.Verify(
            x => x.AddAsync(
                It.Is<EntityIdName>(c => c.Name.Equals("My test entity")),
                It.IsAny<CancellationToken>()
            )
        );
    }

    [Fact]
    public async Task Should_AddToDbSetAndSave() {
        // Act
        await _sut.HandleAsync(_command, new());

        // Assert
        _db.Verify(x => x.AddAsync(It.IsAny<EntityIdName>(), It.IsAny<CancellationToken>()));
        _db.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));
        _db.VerifyNoOtherCalls();
    }
}