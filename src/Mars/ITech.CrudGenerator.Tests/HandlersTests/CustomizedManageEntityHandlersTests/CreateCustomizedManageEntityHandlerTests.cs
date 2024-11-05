using ITech.CrudGenerator.TestApi;
using ITech.CrudGenerator.TestApi.Application.CustomizedManageEntityFeature.CreateCustomizedManageEntity;
using ITech.CrudGenerator.TestApi.Generators.CustomizedManageEntity;
using Moq;

namespace ITech.CrudGenerator.Tests.HandlersTests.CustomizedManageEntityHandlersTests;

public class CreateCustomizedManageEntityHandlerTests
{
    private readonly CustomizedNameCreateManageEntityCommand _command;
    private readonly Mock<TestMongoDb> _db;
    private readonly CustomizedNameCreateManageEntityHandler _sut;

    public CreateCustomizedManageEntityHandlerTests()
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
        _db.Setup(x => x.AddAsync(It.IsAny<CustomizedManageEntity>(), It.IsAny<CancellationToken>()))
            .Callback((CustomizedManageEntity entity, CancellationToken _) => entity.Id = Guid.NewGuid());

        // Act
        var createdEntityDto = await _sut.HandleAsync(_command, new CancellationToken());

        // Assert
        createdEntityDto.Id.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Should_HasCorrectReturnModelTypeName()
    {
        // Arrange
        _db.Setup(x => x.AddAsync(It.IsAny<CustomizedManageEntity>(), It.IsAny<CancellationToken>()))
            .Callback((CustomizedManageEntity entity, CancellationToken _) => entity.Id = Guid.NewGuid());

        // Act
        var createdEntityDto = await _sut.HandleAsync(_command, new CancellationToken());

        createdEntityDto.GetType().Name.Should().Be("CustomizedNameCreatedManageEntityDto");
    }

    [Fact]
    public async Task Should_MapCommandToEntityCorrectly()
    {
        // Act
        await _sut.HandleAsync(_command, new CancellationToken());

        // Assert
        _db.Verify(
            x => x.AddAsync(It.Is<CustomizedManageEntity>(c => c.Name.Equals("My test entity")),
                It.IsAny<CancellationToken>())
        );
    }

    [Fact]
    public async Task Should_AddToDbSetAndSave()
    {
        // Act
        await _sut.HandleAsync(_command, new CancellationToken());

        // Assert
        _db.Verify(x => x.AddAsync(It.IsAny<CustomizedManageEntity>(), It.IsAny<CancellationToken>()));
        _db.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));
        _db.VerifyNoOtherCalls();
    }
}