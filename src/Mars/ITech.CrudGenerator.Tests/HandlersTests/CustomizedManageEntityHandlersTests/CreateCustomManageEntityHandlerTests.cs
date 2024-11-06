using ITech.CrudGenerator.TestApi;
using ITech.CrudGenerator.TestApi.Application.CustomManagedEntityFeature.ManagedEntityCreateOperationCustomNs;
using ITech.CrudGenerator.TestApi.Generators.CustomManagedEntity;
using ITech.CrudGenerator.Tests.E2eTests.Core;
using Moq;

namespace ITech.CrudGenerator.Tests.HandlersTests.CustomizedManageEntityHandlersTests;

public class CreateCustomManageEntityHandlerTests
{
    private readonly CustomizedNameCreateManagedEntityCommand _command;
    private readonly Mock<TestMongoDb> _db;
    private readonly CustomizedNameCreateManagedEntityHandler _sut;

    public CreateCustomManageEntityHandlerTests()
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
        _db.Setup(x => x.AddAsync(It.IsAny<CustomManagedEntity>(), It.IsAny<CancellationToken>()))
            .Callback((CustomManagedEntity entity, CancellationToken _) => entity.Id = Guid.NewGuid());

        // Act
        var createdEntityDto = await _sut.HandleAsync(_command, new CancellationToken());

        // Assert
        createdEntityDto.Id.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Should_HasCorrectReturnModelTypeName()
    {
        // Arrange
        _db.Setup(x => x.AddAsync(It.IsAny<CustomManagedEntity>(), It.IsAny<CancellationToken>()))
            .Callback((CustomManagedEntity entity, CancellationToken _) => entity.Id = Guid.NewGuid());

        // Act
        var createdEntityDto = await _sut.HandleAsync(_command, new CancellationToken());

        createdEntityDto.GetType().Name.Should().Be("CustomizedNameCreatedManagedEntityDto");
    }

    [Fact]
    public async Task Should_MapCommandToEntityCorrectly()
    {
        // Act
        await _sut.HandleAsync(_command, new CancellationToken());

        // Assert
        _db.Verify(
            x => x.AddAsync(It.Is<CustomManagedEntity>(c => c.Name.Equals("My test entity")),
                It.IsAny<CancellationToken>())
        );
    }

    [Fact]
    public async Task Should_AddToDbSetAndSave()
    {
        // Act
        await _sut.HandleAsync(_command, new CancellationToken());

        // Assert
        _db.Verify(x => x.AddAsync(It.IsAny<CustomManagedEntity>(), It.IsAny<CancellationToken>()));
        _db.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));
        _db.VerifyNoOtherCalls();
    }

    [Theory]
    [InlineData("CustomizedNameCreateManagedEntityCommand")]
    [InlineData("CustomizedNameCreateManagedEntityHandler")]
    [InlineData("CustomizedNameCreatedManagedEntityDto")]
    public void Should_BeInCustomNamespace(string typeName)
    {
        // Assert
        typeof(Program).Assembly.Should().BeInNamespaceThatEndsWith(typeName, "ManagedEntityCreateOperationCustomNs");
    }
}

// Пофиксить
// System.Reflection.ReflectionTypeLoadException: Unable to load one or more of the requested types.
//
// System.Reflection.ReflectionTypeLoadException
// Unable to load one or more of the requested types.
// Could not load type 'Castle.Proxies.DbSet`1Proxy_1' from assembly 'DynamicProxyGenAssembly2, Version=0.0.0.0, Culture=neutral, PublicKeyToken=a621a9e7e5c32e69'.
// at System.Reflection.RuntimeModule.GetTypes(RuntimeModule module)
// at System.Reflection.RuntimeModule.GetTypes()
// at ITech.CrudGenerator.Tests.HandlersTests.CustomizedManageEntityHandlersTests.CreateCustomManageEntityHandlerTests.<>c.<Should_BeInCustomNamespace>b__8_0(Assembly x) in /home/anks/projects/mars/src/Mars/ITech.CrudGenerator.Tests/HandlersTests/CustomizedManageEntityHandlersTests/CreateCustomManageEntityHandlerTests.cs:line 85
// at System.Linq.Enumerable.SelectManySingleSelectorIterator`2.MoveNext()
// at System.Linq.Enumerable.WhereEnumerableIterator`1.MoveNext()
// at System.Linq.Enumerable.ZipIterator[TFirst,TSecond,TResult](IEnumerable`1 first, IEnumerable`1 second, Func`3 resultSelector)+MoveNext()
// at FluentAssertions.Collections.GenericCollectionAssertions`3.CollectFailuresFromInspectors(IEnumerable`1 elementInspectors)
// at FluentAssertions.Collections.GenericCollectionAssertions`3.AllSatisfy(Action`1 expected, String because, Object[] becauseArgs)
// at ITech.CrudGenerator.Tests.HandlersTests.CustomizedManageEntityHandlersTests.CreateCustomManageEntityHandlerTests.Should_BeInCustomNamespace(String typeName) in /home/anks/projects/mars/src/Mars/ITech.CrudGenerator.Tests/HandlersTests/CustomizedManageEntityHandlersTests/CreateCustomManageEntityHandlerTests.cs:line 89
// at InvokeStub_CreateCustomManageEntityHandlerTests.Should_BeInCustomNamespace(Object, Span`1)
// at System.Reflection.MethodBaseInvoker.InvokeWithOneArg(Object obj, BindingFlags invokeAttr, Binder binder, Object[] parameters, CultureInfo culture)
