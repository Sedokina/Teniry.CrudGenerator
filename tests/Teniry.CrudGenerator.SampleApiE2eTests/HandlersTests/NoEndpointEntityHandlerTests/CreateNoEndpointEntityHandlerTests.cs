using Teniry.CrudGenerator.SampleApi;
using Teniry.CrudGenerator.SampleApi.Application.NoEndpointEntityFeature.CreateNoEndpointEntity;
using Moq;
using Teniry.CrudGenerator.SampleApi.CrudConfigurations.NoEndpointEntityGenerator;

namespace Teniry.CrudGenerator.SampleApiE2eTests.HandlersTests.NoEndpointEntityHandlerTests;

public class CreateNoEndpointEntityHandlerTests {
    private readonly CreateNoEndpointEntityCommand _command;
    private readonly Mock<SampleMongoDb> _db;
    private readonly CreateNoEndpointEntityHandler _sut;

    public CreateNoEndpointEntityHandlerTests() {
        _db = new();
        _sut = new(_db.Object);
        _command = new() {
            Name = "My test entity"
        };
    }

    [Fact]
    public async Task Should_ReturnCorrectValue() {
        // Arrange
        _db.Setup(x => x.AddAsync(It.IsAny<NoEndpointEntity>(), It.IsAny<CancellationToken>()))
            .Callback((NoEndpointEntity entity, CancellationToken _) => entity.Id = Guid.NewGuid());

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
                It.Is<NoEndpointEntity>(c => c.Name.Equals("My test entity")),
                It.IsAny<CancellationToken>()
            )
        );
    }

    [Fact]
    public async Task Should_AddToDbSetAndSave() {
        // Act
        await _sut.HandleAsync(_command, new());

        // Assert
        _db.Verify(x => x.AddAsync(It.IsAny<NoEndpointEntity>(), It.IsAny<CancellationToken>()));
        _db.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));
        _db.VerifyNoOtherCalls();
    }
}