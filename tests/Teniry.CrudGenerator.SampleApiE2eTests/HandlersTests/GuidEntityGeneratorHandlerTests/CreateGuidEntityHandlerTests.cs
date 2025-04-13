using Teniry.CrudGenerator.SampleApi;
using Teniry.CrudGenerator.SampleApi.Application.GuidEntityFeature.CreateGuidEntity;
using Moq;
using Teniry.CrudGenerator.SampleApi.CrudConfigurations.CustomIds.GuidEntityGenerator;

namespace Teniry.CrudGenerator.SampleApiE2eTests.HandlersTests.GuidEntityGeneratorHandlerTests;

public class CreateGuidEntityHandlerTests {
    private readonly CreateGuidEntityCommand _command;
    private readonly Mock<SampleMongoDb> _db;
    private readonly CreateGuidEntityHandler _sut;

    public CreateGuidEntityHandlerTests() {
        _db = new();
        _sut = new(_db.Object);
        _command = new() {
            Name = "My test entity"
        };
    }

    [Fact]
    public async Task Should_ReturnCorrectValue() {
        // Arrange
        _db.Setup(x => x.AddAsync(It.IsAny<GuidEntity>(), It.IsAny<CancellationToken>()))
            .Callback((GuidEntity entity, CancellationToken _) => entity.GuidEntityId = Guid.NewGuid());

        // Act
        var createdEntityDto = await _sut.HandleAsync(_command, new());

        // Assert
        createdEntityDto.GuidEntityId.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Should_MapCommandToEntityCorrectly() {
        // Act
        await _sut.HandleAsync(_command, new());

        // Assert
        _db.Verify(
            x => x.AddAsync(
                It.Is<GuidEntity>(c => c.Name.Equals("My test entity")),
                It.IsAny<CancellationToken>()
            )
        );
    }

    [Fact]
    public async Task Should_AddToDbSetAndSave() {
        // Act
        await _sut.HandleAsync(_command, new());

        // Assert
        _db.Verify(x => x.AddAsync(It.IsAny<GuidEntity>(), It.IsAny<CancellationToken>()));
        _db.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));
        _db.VerifyNoOtherCalls();
    }
}