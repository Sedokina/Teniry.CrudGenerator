using ITech.CrudGenerator.TestApi;
using ITech.CrudGenerator.TestApi.Application.NoEndpointEntityFeature.CreateNoEndpointEntity;
using ITech.CrudGenerator.TestApi.Generators.NoEndpointEntityGenerator;
using Moq;

namespace ITech.CrudGenerator.TestApiTests.HandlersTests.NoEndpointEntityHandlerTests;

public class CreateNoEndpointEntityHandlerTests
{
    private readonly CreateNoEndpointEntityCommand _command;
    private readonly Mock<TestMongoDb> _db;
    private readonly CreateNoEndpointEntityHandler _sut;

    public CreateNoEndpointEntityHandlerTests()
    {
        _db = new Mock<TestMongoDb>();
        _sut = new(_db.Object);
        _command = new()
        {
            Name = "My test entity"
        };
    }

    [Fact]
    public async Task Should_ReturnCorrectValue()
    {
        // Arrange
        _db.Setup(x => x.AddAsync(It.IsAny<NoEndpointEntity>(), It.IsAny<CancellationToken>()))
            .Callback((NoEndpointEntity entity, CancellationToken _) => entity.Id = Guid.NewGuid());

        // Act
        var createdEntityDto = await _sut.HandleAsync(_command, new CancellationToken());

        // Assert
        createdEntityDto.Id.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Should_MapCommandToEntityCorrectly()
    {
        // Act
        await _sut.HandleAsync(_command, new CancellationToken());

        // Assert
        _db.Verify(
            x => x.AddAsync(It.Is<NoEndpointEntity>(c => c.Name.Equals("My test entity")),
                It.IsAny<CancellationToken>())
        );
    }

    [Fact]
    public async Task Should_AddToDbSetAndSave()
    {
        // Act
        await _sut.HandleAsync(_command, new CancellationToken());

        // Assert
        _db.Verify(x => x.AddAsync(It.IsAny<NoEndpointEntity>(), It.IsAny<CancellationToken>()));
        _db.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));
        _db.VerifyNoOtherCalls();
    }
}