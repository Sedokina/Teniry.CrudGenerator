using ITech.CrudGenerator.TestApi;
using ITech.CrudGenerator.TestApi.Application.SimpleEntityFeature.CreateSimpleEntity;
using ITech.CrudGenerator.TestApi.Generators.SimpleEntityGenerator;
using Moq;

namespace ITech.CrudGenerator.Tests.HandlersTests;

public class CreateHandlerTests
{
    private readonly CreateSimpleEntityCommand _command;
    private readonly Mock<TestMongoDb> _db;
    private readonly CreateSimpleEntityHandler _sut;

    public CreateHandlerTests()
    {
        _db = new Mock<TestMongoDb>();
        _sut = new CreateSimpleEntityHandler(_db.Object);
        _command = new CreateSimpleEntityCommand
        {
            Name = "My test entity"
        };
    }

    [Fact]
    public async Task Should_ReturnCorrectValue()
    {
        // Arrange
        _db.Setup(x => x.AddAsync(It.IsAny<SimpleEntity>(), It.IsAny<CancellationToken>()))
            .Callback((SimpleEntity entity, CancellationToken _) => entity.Id = Guid.NewGuid());

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
            x => x.AddAsync(It.Is<SimpleEntity>(c => c.Name.Equals("My test entity")),
                It.IsAny<CancellationToken>())
        );
    }

    [Fact]
    public async Task Should_AddToDbSetAndSave()
    {
        // Act
        await _sut.HandleAsync(_command, new CancellationToken());

        // Assert
        _db.Verify(x => x.AddAsync(It.IsAny<SimpleEntity>(), It.IsAny<CancellationToken>()));
        _db.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));
        _db.VerifyNoOtherCalls();
    }
}