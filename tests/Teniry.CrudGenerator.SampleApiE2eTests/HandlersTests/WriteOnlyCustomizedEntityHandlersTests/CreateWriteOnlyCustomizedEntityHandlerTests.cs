using Moq;
using Teniry.CrudGenerator.SampleApi;
using Teniry.CrudGenerator.SampleApi.Application.WriteOnlyCustomizedEntityFeature.ManagedEntityCreateOperationCustomNs;
using Teniry.CrudGenerator.SampleApi.CrudConfigurations.WriteOnlyCustomizedGenerator;
using Teniry.CrudGenerator.SampleApiE2eTests.E2eTests.Core;

namespace Teniry.CrudGenerator.SampleApiE2eTests.HandlersTests.WriteOnlyCustomizedEntityHandlersTests;

public class CreateWriteOnlyCustomizedEntityHandlerTests {
    private readonly CustomizedNameCreateManagedEntityCommand _command;
    private readonly Mock<SampleMongoDb> _db;
    private readonly CustomizedNameCreateManagedEntityHandler _sut;

    public CreateWriteOnlyCustomizedEntityHandlerTests() {
        _db = new();
        _sut = new(_db.Object);
        _command = new() {
            Name = "My test entity"
        };
    }

    [Fact]
    public async Task Should_ReturnCorrectValue() {
        // Arrange
        _db.Setup(x => x.AddAsync(It.IsAny<WriteOnlyCustomizedEntity>(), It.IsAny<CancellationToken>()))
            .Callback((WriteOnlyCustomizedEntity entity, CancellationToken _) => entity.Id = Guid.NewGuid());

        // Act
        var createdEntityDto = await _sut.HandleAsync(_command, new());

        // Assert
        createdEntityDto.Id.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Should_HasCorrectReturnModelTypeName() {
        // Arrange
        _db.Setup(x => x.AddAsync(It.IsAny<WriteOnlyCustomizedEntity>(), It.IsAny<CancellationToken>()))
            .Callback((WriteOnlyCustomizedEntity entity, CancellationToken _) => entity.Id = Guid.NewGuid());

        // Act
        var createdEntityDto = await _sut.HandleAsync(_command, new());

        createdEntityDto.GetType().Name.Should().Be("CustomizedNameCreatedManagedEntityDto");
    }

    [Fact]
    public async Task Should_MapCommandToEntityCorrectly() {
        // Act
        await _sut.HandleAsync(_command, new());

        // Assert
        _db.Verify(
            x => x.AddAsync(
                It.Is<WriteOnlyCustomizedEntity>(c => c.Name.Equals("My test entity")),
                It.IsAny<CancellationToken>()
            )
        );
    }

    [Fact]
    public async Task Should_AddToDbSetAndSave() {
        // Act
        await _sut.HandleAsync(_command, new());

        // Assert
        _db.Verify(x => x.AddAsync(It.IsAny<WriteOnlyCustomizedEntity>(), It.IsAny<CancellationToken>()));
        _db.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));
        _db.VerifyNoOtherCalls();
    }

    [Theory]
    [InlineData("CustomizedNameCreateManagedEntityCommand")]
    [InlineData("CustomizedNameCreateManagedEntityHandler")]
    [InlineData("CustomizedNameCreatedManagedEntityDto")]
    public void Should_BeInCustomNamespace(string typeName) {
        // Assert
        typeof(Program).Assembly.Should().BeInNamespaceThatEndsWith(typeName, "ManagedEntityCreateOperationCustomNs");
    }
}